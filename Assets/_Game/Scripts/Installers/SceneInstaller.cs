using Game.Common;
using Game.Network;
using Game.Player;
using Game.PrefabsActions;
using Game.Utils;
using UnityEngine;
using Zenject;

namespace Game.Installers
{
    public class SceneInstaller : MonoInstaller
    {
        [SerializeField] private ChatUi _chatUi;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<ChatUi>()
                .FromInstance(_chatUi)
                .AsSingle()
                .NonLazy();
            
            Container.Bind<PrefabCreator>()
                .AsSingle()
                .NonLazy();
            
            Container.Bind<PlayerCreator>().AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<CannonController>().AsSingle().NonLazy();
        }
    }
}