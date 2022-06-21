using System;
using Game.Common;
using Game.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Game.Controllers
{
    public class KeyboardController : IDisposable
    {
        private readonly InputActionAsset _inputAsset;
        private readonly PlayerModel _playerModel;
        
        private readonly InputActionMap _keyboardMap;
        private readonly InputAction _moveVector;
        private readonly InputAction _jump;
        private readonly InputAction _sprint;
        private readonly InputAction _interact;
        
        private readonly SignalBus _signalBus;
        
        public KeyboardController(InputActionAsset inputAsset, PlayerModel playerModel, SignalBus signalBus)
        {
            _inputAsset = inputAsset;
            _playerModel = playerModel;
            _signalBus = signalBus;
            
            _keyboardMap = _inputAsset.FindActionMap("Keyboard");
            _moveVector = _keyboardMap.FindAction("Move");
            _jump = _keyboardMap.FindAction("Jump");
            _sprint = _keyboardMap.FindAction("Sprint");
            _interact = _keyboardMap.FindAction("Interact");
            
            _moveVector.performed += OnMovePerformed;
            _jump.performed += OnJumpPerformed;
            _sprint.performed += OnSprintPerformed;
            _sprint.canceled += OnSprintCanceled;
            _interact.canceled += OnInteractPerformed;
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            _signalBus.Fire(new KeyboardSignals.MovePerformed() { Value = context.ReadValue<Vector2>() });
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            _signalBus.Fire<KeyboardSignals.JumpPerformed>();
        }

        private void OnSprintPerformed(InputAction.CallbackContext context)
        {
            _signalBus.Fire<KeyboardSignals.SprintPerformed>();
        }
        
        private void OnSprintCanceled(InputAction.CallbackContext context)
        {
            _signalBus.Fire<KeyboardSignals.SprintCanceled>();
        }
        
        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            _signalBus.Fire<KeyboardSignals.InteractPerformed>();
        }

        public void Dispose()
        {
            _moveVector.performed -= OnMovePerformed;
            _jump.performed -= OnJumpPerformed;
            _sprint.performed -= OnSprintPerformed;
            _sprint.canceled -= OnSprintCanceled;
            _interact.canceled -= OnInteractPerformed;
        }
    }
}