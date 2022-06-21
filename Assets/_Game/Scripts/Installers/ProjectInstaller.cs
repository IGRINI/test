using System.ComponentModel;
using Game.Controllers;
using UnityEngine;
using Zenject;
using Game.Player;
using Game.PrefabsActions;
using UnityEngine.InputSystem;

namespace Game.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private PlayerModel _player;
        [SerializeField] private InputActionAsset _inputAsset;
        
        public override void InstallBindings()
        {
            SignalsInstaller.Install(Container);
            
            Container.Bind<PlayerModel>().FromInstance(_player).AsSingle();
            
            _inputAsset.Enable();
            Container.Bind<InputActionAsset>().FromInstance(_inputAsset).AsSingle().NonLazy();

            Container.BindInterfacesTo<MouseController>().AsSingle().NonLazy();
            Container.BindInterfacesTo<KeyboardController>().AsSingle().NonLazy();
        }
    }
}