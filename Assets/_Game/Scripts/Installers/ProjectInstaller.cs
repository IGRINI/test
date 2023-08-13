using Game.Controllers;
using Game.Controllers.Gameplay;
using Game.Network;
using UnityEngine;
using Zenject;
using Game.Views.Player;
using UnityEngine.InputSystem;

namespace Game.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private PlayerView _player;
        [SerializeField] private InputActionAsset _inputAsset;
        
        public override void InstallBindings()
        {
            SignalsInstaller.Install(Container);
            
            Container.Bind<PlayerView>().FromInstance(_player).AsSingle();

            Container.BindInterfacesAndSelfTo<InteractiveController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<PlayerMoveController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MouseLookController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ClientController>().AsSingle().MoveIntoAllSubContainers().NonLazy();
            
            _inputAsset.Enable();
            Container.Bind<InputActionAsset>().FromInstance(_inputAsset).AsSingle().NonLazy();

            Container.BindInterfacesTo<MouseController>().AsSingle().NonLazy();
            Container.BindInterfacesTo<KeyboardController>().AsSingle().NonLazy();
        }
    }
}