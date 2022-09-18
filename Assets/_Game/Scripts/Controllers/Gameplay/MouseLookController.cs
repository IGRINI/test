using System;
using Game.Common;
using Game.Views.Player;
using UnityEngine;
using Zenject;

namespace Game.Controllers.Gameplay
{
    public class MouseLookController
    {
        private readonly SignalBus _signalBus;
        private readonly Settings _settings;
        
        private PlayerView _player;
        private bool _isPlayerCreated;
        private bool _isMouseLookEnabled;
        
        private float _cameraRotationX;
        private float _cameraRotationY;
        
        public MouseLookController(SignalBus signalBus, Settings settings)
        {
            _signalBus = signalBus;
            _settings = settings;
            
            _signalBus.Subscribe<GameSignals.PlayerSpawned>(OnPlayerSpawned);
            _signalBus.Subscribe<GameSignals.PlayerMoveActive>(OnPlayerMoveActive);
            _signalBus.Subscribe<MouseSignals.MouseDeltaPerformed>(obj => MoveCamera(obj.Value));
        }

        private void OnPlayerSpawned(GameSignals.PlayerSpawned eventObject)
        {
            _player = eventObject.Player;
            _isPlayerCreated = true;
        }

        private void OnPlayerMoveActive(GameSignals.PlayerMoveActive eventObject)
        {
            _isMouseLookEnabled = eventObject.IsActive;
        }

        private void MoveCamera(Vector2 mouseDelta)
        {
            if(!_isPlayerCreated || !_isMouseLookEnabled) return;
            
            _cameraRotationX = _player.Transform.localRotation.eulerAngles.y + mouseDelta.x * _settings.Mouse.Sensitivity.x;

            _player.Transform.localEulerAngles = new Vector3(0, _cameraRotationX);
            
            _cameraRotationY = Mathf.Clamp(_cameraRotationY +
                                           mouseDelta.y * _settings.Mouse.Sensitivity.y, _settings.Mouse.YClamps.x,
                _settings.Mouse.YClamps.y);
            
            _player.CameraTransform.localEulerAngles = new Vector3(-_cameraRotationY, 0);
        }
        
        [Serializable]
        public class Settings
        {
            public MouseSettings Mouse;

            [Serializable]
            public class MouseSettings
            {
                public Vector2 Sensitivity;
                public Vector2 YClamps;
            }
        }
    }
}