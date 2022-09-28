using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.Users;
using UnityEngine.Internal;

namespace EmptySoul.TestQuest._Develop.Scripts.TestScripts
{
    public enum ControllerType
    {
        MouseAndKeyboard = 0,
        XboxGamepad = 1
    }


    public class TestInput : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private CharacterController characterController;

        private TestActions _testActions;
        private static readonly int Horizontal = Animator.StringToHash("Horizontal");
        private static readonly int Vertical = Animator.StringToHash("Vertical");
        [SerializeField] private ControllerType _controllerType = ControllerType.MouseAndKeyboard;


        private void Awake()
        {
            _testActions = new TestActions();
            _testActions.Character.Enable();

            InputUser.onChange += (user, change, device) =>
            {
                if (change == InputUserChange.DevicePaired)
                {
                    Debug.Log(device);

                    switch (device.ToString())
                    {
                        case { } x when x.Contains("Keyboard"):
                            _controllerType = ControllerType.MouseAndKeyboard;
                            break;
                        case { } x when x.Contains("XInputControllerWindows"):
                            _controllerType = ControllerType.XboxGamepad;
                            break;
                        default:
                            _controllerType = ControllerType.MouseAndKeyboard;
                            break;
                    }
                }
            };
        }

        private void Update()
        {
            var velocity = _testActions.Character.Move.ReadValue<Vector2>();
            
            Move(velocity);
        }

        private Vector2 _smoothInputVelocity;
        private Vector2 _currentVelocity;
        
        private void Move(Vector2 velocity)
        {
            
            _currentVelocity = Vector2.SmoothDamp(_currentVelocity, velocity, ref _smoothInputVelocity, .2f);
            
            switch (_controllerType)
            {
                case ControllerType.MouseAndKeyboard:
                    
                    animator.SetFloat(Horizontal, _currentVelocity.x);
                    animator.SetFloat(Vertical, _currentVelocity.y);

                    break;
                case ControllerType.XboxGamepad:
                    animator.SetFloat(Horizontal, _currentVelocity.x);
                    animator.SetFloat(Vertical, _currentVelocity.y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            characterController.Move(new Vector3(_currentVelocity.x, 0, _currentVelocity.y) * Time.deltaTime);
        }
        
        public async void Graduate(Action<float> action, float duration, bool reverse = false)
        {
            for (float time = 0f; time < duration; time += Time.deltaTime)
            {
                float ratio = time / duration;
                ratio = reverse ? 1f - ratio : ratio;

                float progress = ratio;

                action.Invoke(progress);

                await Task.Yield();
            }

            action.Invoke(reverse ? 0f : 1f);
        }
    }
}