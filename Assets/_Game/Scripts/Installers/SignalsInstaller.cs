﻿using Game.Common;
using Game.Network;
using UnityEngine;
using Zenject;

namespace Game.Installers
{
    public class SignalsInstaller : Installer<SignalsInstaller>
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            
            DeclareSignal<GameSignals.PlayerSpawned>();
            DeclareSignal<GameSignals.PlayerSpawnRequest>();
            DeclareSignal<GameSignals.PlayerMoveActive>();
            DeclareSignal<GameSignals.PlayerInteractiveActive>();
            DeclareSignal<GameSignals.CannonInteract>();
        }

        private DeclareSignalRequireHandlerAsyncTickPriorityCopyBinder DeclareSignal<TSignal>()
        {
            return Container.DeclareSignal<TSignal>();
        }
    }
}