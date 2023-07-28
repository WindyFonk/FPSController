using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FPS.Manager
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;

        public Vector2 Move { get; set; }
        public Vector2 Look { get; set; }
        public bool Run { get; set; }
        public bool Kick { get; set; }
        public bool Equip { get; set; }
        public bool Shoot { get; set; }
        public bool Knife { get; set; }

        private InputActionMap _currentMap;
        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _runAction;
        private InputAction _kickAction;
        private InputAction _equipAction;
        private InputAction _shootAction;
        private InputAction _knifeAction;

        private void Awake()
        {
            _currentMap = playerInput.currentActionMap;
            _moveAction = _currentMap.FindAction("Move");
            _lookAction = _currentMap.FindAction("Look");
            _runAction = _currentMap.FindAction("Run");
            _kickAction = _currentMap.FindAction("Kick");
            _equipAction = _currentMap.FindAction("Equip");
            _shootAction = _currentMap.FindAction("Shoot");
            _knifeAction = _currentMap.FindAction("Knife");

            _moveAction.performed += onMove;
            _lookAction.performed += onLook;
            _runAction.performed += onRun;
            _kickAction.performed += onKick;
            _equipAction.started += onEquip;
            _shootAction.started += onShoot;
            _knifeAction.started += onKnife;

            _moveAction.canceled += onMove;
            _lookAction.canceled += onLook;
            _runAction.canceled += onRun;
            _kickAction.canceled += onKick;
            _equipAction.canceled += onEquip;
            _shootAction.canceled += onShoot;
            _knifeAction.canceled += onKnife;

        }

        private void onMove(InputAction.CallbackContext context)
        {
            Move = context.ReadValue<Vector2>();
        }
        private void onLook(InputAction.CallbackContext context)
        {
            Look = context.ReadValue<Vector2>();
        }
        private void onRun(InputAction.CallbackContext context)
        {
            Run = context.ReadValueAsButton();
        }

        private void onKick(InputAction.CallbackContext context)
        {
            Kick = context.ReadValueAsButton();
        }

        private void onEquip(InputAction.CallbackContext context)
        {
            Equip = context.ReadValueAsButton();
        }
        private void onShoot(InputAction.CallbackContext context)
        {
            Shoot = context.ReadValueAsButton();
        }
        private void onKnife(InputAction.CallbackContext context)
        {
            Knife = context.ReadValueAsButton();
        }



        private void OnEnable()
        {
            _currentMap.Enable();
        }

        private void OnDisable()
        {
            _currentMap.Disable();
        }
    }
}
