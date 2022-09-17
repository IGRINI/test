using Game.Entities;
using UnityEngine;

namespace Game.Views.Player
{
    public class PlayerView : BaseEntityModel
    {
        [SerializeField] private Camera _camera;

        public Camera Camera => _camera;
        
        public Transform CameraTransform { get; private set; }

        public Transform Transform { get; private set; }

        public CharacterController CharacterController { get; private set; }
        
        private void Awake()
        {
            CameraTransform = _camera.transform;
            Transform = transform;
            CharacterController = GetComponent<CharacterController>();
        }
    }
}