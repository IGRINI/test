using System;
using Game.Common;
using UnityEngine;
using Zenject;

namespace Game.Player
{
    public class PlayerSpawnPointModel : MonoBehaviour
    {
        private SignalBus _signalBus;
        
        [Inject]
        private void Constructor(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void Start()
        {
            _signalBus.Fire(new GameSignals.PlayerSpawnRequest() { Position = transform.position });
        }
    }
}