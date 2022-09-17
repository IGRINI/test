using Game.Player;
using Game.PrefabsActions;
using Zenject;

namespace Game.Installers
{
    public class SceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<PrefabCreator>()
                .AsSingle()
                .NonLazy();
            
            Container.Bind<PlayerCreator>().AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<CannonController>().AsSingle().NonLazy();
        }
    }
}