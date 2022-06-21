using Game.Common;
using Game.PrefabsActions;
using UnityEngine;
using Zenject;

namespace Game.Player
{
    public class PlayerCreator
    {
        private readonly PlayerModel _playerModelPrefab;
        private readonly PrefabCreator _prefabCreator;
        
        private PlayerCreator(PlayerModel playerModelPrefab, PrefabCreator prefabCreator, SignalBus signalBus)
        {
            _playerModelPrefab = playerModelPrefab;
            _prefabCreator = prefabCreator;
            
            signalBus.Subscribe<GameSignals.PlayerSpawnRequest>(CreatePlayerForRequest);
        }

        private void CreatePlayerForRequest(GameSignals.PlayerSpawnRequest playerSpawnRequest)
        {
            _prefabCreator.Create(_playerModelPrefab).transform.position = playerSpawnRequest.Position;
        }
    }
}