using System;
using System.Linq;
using Game.Common;
using Game.Entities;
using Game.Entities.Modifiers;
using Game.Interactables;
using ReadOnlyAttribute;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Player
{
    public class PlayerModel : BaseEntityModel
    {
        [SerializeField] private Camera _camera;

        private Transform _cameraTransform;
        private Transform _transform;
        
        private CharacterController _characterController;

        private float _cameraRotationX;
        private float _cameraRotationY;
        
        private Settings _settings;
        private SignalBus _signalBus;

        private Vector2 _inputs;

        private bool _interact;

        private float _yForce;

        private void Awake()
        {
            _cameraTransform = _camera.transform;
            _transform = transform;
            _characterController = GetComponent<CharacterController>();
        }

        [Inject]
        private void Constructor(Settings settings, SignalBus signalBus)
        {
            _settings = settings;
            _signalBus = signalBus;
            
            _signalBus.Subscribe<MouseSignals.MouseDeltaPerformed>(obj => MoveCamera(obj.Value));
            _signalBus.Subscribe<KeyboardSignals.MovePerformed>(obj => MoveRigidbody(obj.Value));
            _signalBus.Subscribe<KeyboardSignals.JumpPerformed>(_ => Jump());
            _signalBus.Subscribe<KeyboardSignals.SprintPerformed>(_ => SetSprint(true));
            _signalBus.Subscribe<KeyboardSignals.SprintCanceled>(_ => SetSprint(false));
            _signalBus.Subscribe<KeyboardSignals.InteractPerformed>(_ => Interact());

            Observable.EveryUpdate()
                .Subscribe(_ => EveryUpdate())
                .AddTo(this);
            Observable.EveryFixedUpdate()
                .Subscribe(_ => EveryFixedUpdate())
                .AddTo(this);
        }

        private void Interact()
        {
            _interact = true;
        }

        private void EveryFixedUpdate()
        {
            if (Physics.SphereCast(_cameraTransform.position, _settings.Mouse.InteractiveRayRadius, _cameraTransform.forward, out var hit, _settings.Mouse.InteractiveRayDistance, _settings.Mouse.InteractiveSphereLayerMask)
                &&
                hit.collider.TryGetComponent<IInteractable>(out var interactable))
            {
                if (_interact)
                {
                    interactable.Interact(hit);
                }
            }
            _interact = false;
        }

        private void EveryUpdate()
        {
            Move(_inputs);
        }

        private void Move(Vector2 inputs)
        {
            if (!IsGrounded())
                _yForce += Physics.gravity.y * Time.deltaTime;
            else if (_yForce < 0f)
                _yForce = 0f;

            var force = _yForce * Time.deltaTime;
            
            var movedVector = new Vector3(0f, force);
            
            if (inputs.Equals(Vector3.zero))
            {
                _characterController.Move(movedVector);
                return;
            }

            movedVector += _transform.forward * inputs.y * Time.deltaTime * _settings.Move.Speed;
            
            if (inputs.y > 0)
                movedVector *= GetSpeedMultiplier();

            movedVector.y = force;
            
            movedVector += _transform.right * inputs.x * Time.deltaTime * _settings.Move.Speed;

            _characterController.Move(movedVector);
        }

        private void SetSprint(bool sprint)
        {
            if (sprint)
            {
                AddModifier(new SprintModifier(_settings.Move.SprintMultuplier));
            }
            else
            {
                RemoveModifier<SprintModifier>();
            }
        }

        private bool IsGrounded()
        {
            var bounds = _characterController.bounds;
            return Physics.CheckSphere(new Vector3(bounds.center.x, bounds.min.y + _settings.Move.GroundCheck.SphereDownOffset, bounds.center.z),
                _settings.Move.GroundCheck.SphereRadius,
                _settings.Move.GroundCheck.GroundLayerMask);
        }

        private void Jump()
        {
            if(IsGrounded())
            {
                _yForce = _settings.Move.JumpForce;
            }
        }

        private void MoveCamera(Vector2 mouseDelta)
        {
            _cameraRotationX = _transform.localRotation.eulerAngles.y + mouseDelta.x * _settings.Mouse.Sensitivity.x;

            _transform.localEulerAngles = new Vector3(0, _cameraRotationX);
            
            _cameraRotationY = Mathf.Clamp(_cameraRotationY +
                    mouseDelta.y * _settings.Mouse.Sensitivity.y, _settings.Mouse.YClamps.x,
                _settings.Mouse.YClamps.y);
            
            _cameraTransform.localEulerAngles = new Vector3(-_cameraRotationY, 0);
        }

        private void MoveRigidbody(Vector2 moveDelta)
        {
            _inputs = moveDelta;
        }

        [Serializable]
        public class Settings
        {
            public MoveSettings Move;
            public MouseSettings Mouse;

            [Serializable]
            public class MouseSettings
            {
                public float InteractiveRayDistance;
                public float InteractiveRayRadius;
                public LayerMask InteractiveRayLayerMask;
                public LayerMask InteractiveSphereLayerMask;
                public Vector2 Sensitivity;
                public Vector2 YClamps;
            }
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
