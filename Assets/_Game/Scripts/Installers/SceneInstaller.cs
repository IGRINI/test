using Game.Common;
using Game.Network;
using Game.Player;
using Game.PrefabsActions;
using Game.Utils;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Game.Installers
{
    public class SceneInstaller : MonoInstaller
    {
        [FormerlySerializedAs("_chatUi")] [SerializeField] private OldChatUi oldChatUi;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<OldChatUi>()
                .FromInstance(oldChatUi)
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