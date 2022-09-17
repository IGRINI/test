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
        
        private readonly InputActionMap _keyboardMap;
        private readonly InputAction _moveVector;
        private readonly InputAction _jump;
        private readonly InputAction _sprint;
        private readonly InputAction _interact;
        private readonly InputAction _escape;
        
        private readonly SignalBus _signalBus;
        
        public KeyboardController(InputActionAsset inputAsset, SignalBus signalBus)
        {
            _inputAsset = inputAsset;
            _signalBus = signalBus;
            
            _keyboardMap = _inputAsset.FindActionMap("Keyboard");
            _moveVector = _keyboardMap.FindAction("Move");
            _jump = _keyboardMap.FindAction("Jump");
            _escape = _keyboardMap.FindAction("Escape");
            _sprint = _keyboardMap.FindAction("Sprint");
            _interact = _keyboardMap.FindAction("Interact");
            
            _moveVector.performed += OnMovePerformed;
            _jump.performed += OnJumpPerformed;
            _escape.performed += OnEscapePerformed;
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
            _signalBus.Fire(new KeyboardSignals.IsSprintPerformed() { IsPerformed = true});
        }
        
        private void OnSprintCanceled(InputAction.CallbackContext context)
        {
            _signalBus.Fire(new KeyboardSignals.IsSprintPerformed() { IsPerformed = false});
        }
        
        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            _signalBus.Fire<KeyboardSignals.InteractPerformed>();
        }

        private void OnEscapePerformed(InputAction.CallbackContext context)
        {
            _signalBus.Fire<KeyboardSignals.EscapePerformed>();
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