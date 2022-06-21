using System;
using Game.Common;
using Game.Input;
using Game.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Game.Controllers
{
    public class MouseController : IDisposable
    {
        private readonly InputActionAsset _inputAsset;
        private readonly PlayerModel _playerModel;
        
        private readonly InputActionMap _mouseMap;
        private readonly InputAction _mousePosition;
        private readonly InputAction _mouseDelta;
        
        private readonly SignalBus _signalBus;
        
        public MouseController(InputActionAsset inputAsset, PlayerModel playerModel, SignalBus signalBus)
        {
            _inputAsset = inputAsset;
            _playerModel = playerModel;
            _signalBus = signalBus;
            
            _mouseMap = _inputAsset.FindActionMap("Mouse");
            _mousePosition = _mouseMap.FindAction("Position");
            _mouseDelta = _mouseMap.FindAction("Delta");
            
            _mouseDelta.performed += OnMouseDeltaPerformed;
            
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnMouseDeltaPerformed(InputAction.CallbackContext context)
        {
            _signalBus.Fire(new MouseSignals.MouseDeltaPerformed() { Value = context.ReadValue<Vector2>() });
        }

        public void Dispose()
        {
            _mouseDelta.performed -= OnMouseDeltaPerformed;
        }
    }
}