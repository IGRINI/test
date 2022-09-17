using System;
using Game.Common;
using Game.Entities.Modifiers;
using Game.Views.Player;
using UnityEngine;
using Zenject;

namespace Game.Controllers.Gameplay
{
    public class PlayerMoveController : ITickable
    {
        private readonly SignalBus _signalBus;
        private readonly Settings _settings;
        
        private PlayerView _player;
        private bool _isPlayerCreated;
        private bool _isMoveEnabled;
        private float _yForce;
        private Vector2 _inputs;

        private PlayerMoveController(SignalBus signalBus, Settings settings)
        {
            _signalBus = signalBus;
            _settings = settings;
            
            _signalBus.Subscribe<GameSignals.PlayerSpawned>(OnPlayerSpawned);
            _signalBus.Subscribe<GameSignals.PlayerMoveActive>(OnPlayerMoveActive);
            
            _signalBus.Subscribe<KeyboardSignals.MovePerformed>(eventObject => SetMoveInputs(eventObject.Value));
            _signalBus.Subscribe<KeyboardSignals.JumpPerformed>(Jump);
            _signalBus.Subscribe<KeyboardSignals.IsSprintPerformed>(eventObject => SetSprint(eventObject.IsPerformed));
        }

        private void OnPlayerSpawned(GameSignals.PlayerSpawned eventObject)
        {
            _player = eventObject.Player;
            _isPlayerCreated = true;
        }

        private void OnPlayerMoveActive(GameSignals.PlayerMoveActive eventObject)
        {
            _isMoveEnabled = eventObject.IsActive;
        }

        private void SetMoveInputs(Vector2 inputs)
        {
            _inputs = inputs;
        }

        private void Jump()
        {
            if(!_isPlayerCreated || !_isMoveEnabled) return;
            
            if(IsGrounded())
            {
                _yForce = _settings.Move.JumpForce;
            }
        }

        private void SetSprint(bool sprint)
        {
            if (sprint)
            {
                _player.AddModifier(new SprintModifier(_settings.Move.SprintMultuplier));
            }
            else
            {
                _player.RemoveModifier<SprintModifier>();
            }
        }
        
        private bool IsGrounded()
        {
            var bounds = _player.CharacterController.bounds;
            return Physics.CheckSphere(new Vector3(bounds.center.x, bounds.min.y + _settings.Move.GroundCheck.SphereDownOffset, bounds.center.z),
                _settings.Move.GroundCheck.SphereRadius,
                _settings.Move.GroundCheck.GroundLayerMask);
        }

        public void Tick()
        {
            if(!_isPlayerCreated || !_isMoveEnabled) return;
            
            if (!IsGrounded())
                _yForce += Physics.gravity.y * Time.deltaTime;
            else if (_yForce < 0f)
                _yForce = 0f;

            var force = _yForce * Time.deltaTime;

            var speedMultiplier = Time.deltaTime * _settings.Move.Speed;
            
            var movedVector = new Vector3(0f, force);
            
            if (_inputs.Equals(Vector3.zero))
            {
                _player.CharacterController.Move(movedVector);
                return;
            }

            movedVector += _player.Transform.forward * _inputs.y * speedMultiplier;
            
            if (_inputs.y > 0)
                movedVector *= _player.GetSpeedMultiplier();

            movedVector.y = force;
            
            movedVector += _player.Transform.right * _inputs.x * speedMultiplier;

            _player.CharacterController.Move(movedVector);
        }
        
        [Serializable]
        public class Settings
        {
            public MoveSettings Move;
            
            [Serializable]
            public class MoveSettings
            {
                public float Speed;
                public float JumpForce;
                public float SprintMultuplier;
                
                public GroundCheck GroundCheck;
            }
            [Serializable]
            public class GroundCheck
            {
                public float SphereRadius;
                public float SphereDownOffset;

                public LayerMask GroundLayerMask;
            }
        }
    }
}