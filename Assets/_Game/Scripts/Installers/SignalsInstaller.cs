using Game.Common;
using Zenject;

namespace Game.Installers
{
    public class SignalsInstaller : Installer<SignalsInstaller>
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            DeclareSignal<MouseSignals.MouseDeltaPerformed>().OptionalSubscriber();
            
            DeclareSignal<KeyboardSignals.MovePerformed>();
            DeclareSignal<KeyboardSignals.JumpPerformed>();
            DeclareSignal<KeyboardSignals.IsSprintPerformed>();
            DeclareSignal<KeyboardSignals.InteractPerformed>();
            DeclareSignal<KeyboardSignals.EscapePerformed>();
            
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