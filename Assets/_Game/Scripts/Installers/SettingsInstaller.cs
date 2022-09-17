using Game.Controllers.Gameplay;
using Game.Player;
using UnityEngine;
using Zenject;

namespace Game.Installers
{
    [CreateAssetMenu(fileName = "SettingsInstaller", menuName = "Installers/SettingsInstaller")]
    public class SettingsInstaller : ScriptableObjectInstaller<SettingsInstaller>
    {
        [SerializeField] private InteractiveController.Settings _interactiveSettings;
        [SerializeField] private PlayerMoveController.Settings _playerMoveSettings;
        [SerializeField] private MouseLookController.Settings _mouseLookSettings;
        [SerializeField] private CannonController.Settings _cannonSettings;
        
        public override void InstallBindings()
        {
            Container.Bind<InteractiveController.Settings>().FromInstance(_interactiveSettings).AsSingle().CopyIntoAllSubContainers().NonLazy();
            Container.Bind<PlayerMoveController.Settings>().FromInstance(_playerMoveSettings).AsSingle().CopyIntoAllSubContainers().NonLazy();
            Container.Bind<MouseLookController.Settings>().FromInstance(_mouseLookSettings).AsSingle().CopyIntoAllSubContainers().NonLazy();
            Container.Bind<CannonController.Settings>().FromInstance(_cannonSettings).AsSingle().CopyIntoAllSubContainers().NonLazy();
        }
    }
}