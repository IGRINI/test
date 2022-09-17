using System;
using Game.Common;
using Game.Interactables;
using UnityEngine;
using Zenject;

public class CannonController
{
    private readonly SignalBus _signalBus;
    private readonly Settings _settings;
    
    private bool IsOnCannon;
    private Cannon _currentCannon;

    private Quaternion _cannonRotationX;
    private float _cannonRotationY;

    private float QuaternionXClamp;
    private Vector2 mouseDelta;
    
    public CannonController(SignalBus signalBus, Settings settings)
    {
        _signalBus = signalBus;
        _settings = settings;
        _signalBus.Subscribe<GameSignals.CannonInteract>(OnCannonInteract);
        QuaternionXClamp = Quaternion.Euler(0, _settings.Mouse.XClamps, 0).y;
    }

    private void OnCannonInteract(GameSignals.CannonInteract eventObject)
    {
        _signalBus.Fire(new GameSignals.PlayerMoveActive() { IsActive = false });
        _signalBus.Fire(new GameSignals.PlayerInteractiveActive() { IsActive = false });
        IsOnCannon = true;
        _currentCannon = eventObject.Cannon;
        _signalBus.Subscribe<KeyboardSignals.EscapePerformed>(CannonExit);
        _signalBus.Subscribe<MouseSignals.MouseDeltaPerformed>(CannonRotate);
    }

    private void CannonExit()
    {
        _signalBus.Fire(new GameSignals.PlayerMoveActive() { IsActive = true });
        _signalBus.Fire(new GameSignals.PlayerInteractiveActive() { IsActive = true });
        IsOnCannon = false;
        _currentCannon = null;
        _signalBus.Unsubscribe<KeyboardSignals.EscapePerformed>(CannonExit);
        _signalBus.Unsubscribe<MouseSignals.MouseDeltaPerformed>(CannonRotate);
    }
    
    private void CannonRotate(MouseSignals.MouseDeltaPerformed mouseDeltaPerformed)
    {
        if (!_currentCannon || !IsOnCannon) return;

        mouseDelta = mouseDeltaPerformed.Value;
        
        _cannonRotationX = Quaternion.Euler(0, _currentCannon.CannonTransform.localRotation.eulerAngles.y + mouseDelta.x * _settings.Mouse.Sensitivity.x, 0);
        _cannonRotationX.y = Mathf.Clamp(_cannonRotationX.y, -QuaternionXClamp, QuaternionXClamp);
        _currentCannon.CannonTransform.localRotation = _cannonRotationX;
            
        _cannonRotationY = Mathf.Clamp(_cannonRotationY +
                                       mouseDelta.y * _settings.Mouse.Sensitivity.y, _settings.Mouse.YClamps.x,
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
            public Vector2 Sensitivity;
            public Vector2 YClamps;
            public float XClamps;
        }
    }
}
