using System;
using Game.Common;
using Game.Interactables;
using Game.Views.Player;
using UnityEngine;
using Zenject;

public class CannonController
{
    private readonly SignalBus _signalBus;
    private readonly Settings _settings;
    private PlayerView _player;
    
    private bool IsOnCannon;
    private Cannon _currentCannon;

    private Quaternion _cannonRotationX;
    private float _cannonRotationY;

    private float _cameraRotationX;
    private float _cameraRotationY;
    
    private readonly float QuaternionXClamp;
    private Vector2 mouseDelta;
    
    
    public CannonController(SignalBus signalBus, Settings settings)
    {
        _signalBus = signalBus;
        _settings = settings;
        _signalBus.Subscribe<GameSignals.CannonInteract>(OnCannonInteract);
        _signalBus.Subscribe<GameSignals.PlayerSpawned>(OnPlayerSpawned);
        QuaternionXClamp = Quaternion.Euler(0, _settings.Mouse.XClamps, 0).y;
    }
    
    private void OnPlayerSpawned(GameSignals.PlayerSpawned eventObject)
    {
        _player = eventObject.Player;
    }

    private void OnCannonInteract(GameSignals.CannonInteract eventObject)
    {
        _signalBus.Fire(new GameSignals.PlayerMoveActive() { IsActive = false });
        _signalBus.Fire(new GameSignals.PlayerInteractiveActive() { IsActive = false });
        IsOnCannon = true;
        _currentCannon = eventObject.Cannon;
        
        _signalBus.Subscribe<KeyboardSignals.EscapePerformed>(CannonExit);
        _signalBus.Subscribe<MouseSignals.MouseDeltaPerformed>(CannonRotate);
        _signalBus.Subscribe<MouseSignals.MouseDeltaPerformed>(MoveCamera);
        
    }

    
    
    private void CannonExit()
    {
        _signalBus.Fire(new GameSignals.PlayerMoveActive() { IsActive = true });
        _signalBus.Fire(new GameSignals.PlayerInteractiveActive() { IsActive = true });
        IsOnCannon = false;
        _currentCannon = null;
        _signalBus.Unsubscribe<KeyboardSignals.EscapePerformed>(CannonExit);
        _signalBus.Unsubscribe<MouseSignals.MouseDeltaPerformed>(CannonRotate);
        _signalBus.Unsubscribe<MouseSignals.MouseDeltaPerformed>(MoveCamera);
    }


    private void MoveCamera(MouseSignals.MouseDeltaPerformed mouseDeltaPerformed)
    {
        if (!_currentCannon || !IsOnCannon) return;

        mouseDelta = mouseDeltaPerformed.Value;

        if (!(_cannonRotationY <= _settings.Mouse.YClamps.x || _cannonRotationY >= _settings.Mouse.YClamps.y))
        {
            _cameraRotationY = Mathf.Clamp(_cameraRotationY +
                                           mouseDelta.y * _settings.Mouse.CameraSensitivity.y, _settings.Mouse.YClamps.x,
            _settings.Mouse.YClamps.y);

            _player.CameraTransform.localEulerAngles = new Vector3(-_cameraRotationY, 0);
        }

        if (_cannonRotationX.y <= -QuaternionXClamp || _cannonRotationX.y >= QuaternionXClamp) return;
        
        _cameraRotationX = _player.Transform.localRotation.eulerAngles.y +
                           mouseDelta.x * _settings.Mouse.CameraSensitivity.x;
        _player.Transform.localEulerAngles = new Vector3(0, _cameraRotationX);
    }
    
    
    
    private void CannonRotate(MouseSignals.MouseDeltaPerformed mouseDeltaPerformed)
    {
        if (!_currentCannon || !IsOnCannon) return;

        mouseDelta = mouseDeltaPerformed.Value;
        
        _cannonRotationX = Quaternion.Euler(0, _currentCannon.CannonTransform.localRotation.eulerAngles.y + mouseDelta.x * _settings.Mouse.CannonSensitivity.x, 0);
        _cannonRotationX.y = Mathf.Clamp(_cannonRotationX.y, -QuaternionXClamp, QuaternionXClamp);
        _currentCannon.CannonTransform.localRotation = _cannonRotationX;
            
        _cannonRotationY = Mathf.Clamp(_cannonRotationY +
                                       mouseDelta.y * _settings.Mouse.CannonSensitivity.y, _settings.Mouse.YClamps.x,
            _settings.Mouse.YClamps.y);
            
        _currentCannon.BarrelTransform.localEulerAngles = new Vector3(-_cannonRotationY, 0);
    }

    
    
    [Serializable]
    public class Settings
    {
        public MouseSettings Mouse;

        [Serializable]
        public class MouseSettings
        {
            public Vector2 CameraSensitivity;
            public Vector2 CannonSensitivity;
            public Vector2 YClamps;
            public float XClamps;
        }
    }
}
