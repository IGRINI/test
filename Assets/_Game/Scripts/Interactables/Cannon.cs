using Game.Common;
using UnityEngine;
using Zenject;

namespace Game.Interactables
{
    public class Cannon : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform _cannonTransform;
        [SerializeField] private Transform _barrelPivot;

        public Transform CannonTransform => _cannonTransform;
        public Transform BarrelTransform => _barrelPivot;
        
        private SignalBus _signalBus;

        [Inject]
        private void Constructor(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public void Interact(RaycastHit hit)
        {
            _signalBus.Fire(new GameSignals.CannonInteract{ Cannon = this });
            
        }
    }
} 