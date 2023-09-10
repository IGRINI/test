using Game.Controllers;
using Game.Controllers.Gameplay;
using Game.Network;
using Game.Services;
using UnityEngine;
using Zenject;
using Game.Views.Player;
using UnityEngine.InputSystem;

namespace Game.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private InputActionAsset _inputAsset;
        
        public override void InstallBindings()
        {
            SignalsInstaller.Install(Container);
            
            Container.BindInterfacesAndSelfTo<ClientController>().AsSingle().MoveIntoAllSubContainers().NonLazy();
            
            _inputAsset.Enable();
            BindInstance(_inputAsset);

            BindSingle<MouseController>();
            BindSingle<KeyboardController>();

            BindSingle<SteamService>();
        }

        private IfNotBoundBinder BindSingle<T>()
        {
            return Container.BindInterfacesAndSelfTo<T>().AsSingle().NonLazy();
        }

        private IfNotBoundBinder BindInstance<T>(T instance)
        {
            return Container.Bind<T>().FromInstance(instance).AsSingle().NonLazy();
        }
    }
}