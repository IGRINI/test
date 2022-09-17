using System;
using Game.Common;
using Game.Interactables;
using Game.Utils;
using Game.Views.Player;
using UnityEngine;
using Zenject;

namespace Game.Controllers.Gameplay
{
    public class InteractiveController : IFixedTickable
    {
        private readonly SignalBus _signalBus;
        private readonly Settings _settings;
        
        private PlayerView _player;
        private bool _isPlayerCreated;
        private bool _isInteractiveEnabled;
        private bool _interact;
        
        public InteractiveController(SignalBus signalBus, Settings settings)
        {
            _signalBus = signalBus;
            _settings = settings;
            
            _signalBus.Subscribe<GameSignals.PlayerSpawned>(OnPlayerSpawned);
            _signalBus.Subscribe<GameSignals.PlayerInteractiveActive>(OnPlayerInteractiveActive);
            _signalBus.Subscribe<KeyboardSignals.InteractPerformed>(Interact);
        }

        private void OnPlayerSpawned(GameSignals.PlayerSpawned eventObject)
        {
            _player = eventObject.Player;
            _isPlayerCreated = true;
        }

        private void OnPlayerInteractiveActive(GameSignals.PlayerInteractiveActive eventObject)
        {
            _isInteractiveEnabled = eventObject.IsActive;
        }

        private void Interact()
        {
            if(!_isPlayerCreated || !_isInteractiveEnabled) return;
            
            _interact = true;
        }

        public void FixedTick()
        {
            if(!_isPlayerCreated || !_isInteractiveEnabled) return;
            
            if (Physics.SphereCast(_player.CameraTransform.position, _settings.Mouse.InteractiveRayRadius, _player.CameraTransform.forward, out var hit, _settings.Mouse.InteractiveRayDistance, _settings.Mouse.InteractiveSphereLayerMask)
                &&
                hit.collider.TryGetComponent<IInteractable>(out var interactable))
            {
                if (_interact)
                {
                    DebugExtensions.DrawWireSphere(hit.point, _settings.Mouse.InteractiveRayRadius, Color.green, 5f);
                    interactable.Interact(hit);
                }
            }
            _interact = false;
        }

        [Serializable]
        public class Settings
        {
            public MouseSettings Mouse;

            [Serializable]
            public class MouseSettings
            {
                public float InteractiveRayDistance;
                public float InteractiveRayRadius;
                public LayerMask InteractiveSphereLayerMask;
            }
        }
    }
}