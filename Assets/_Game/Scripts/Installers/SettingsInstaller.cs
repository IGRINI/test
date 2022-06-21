using Game.Player;
using UnityEngine;
using Zenject;

namespace Game.Installers
{
    [CreateAssetMenu(fileName = "SettingsInstaller", menuName = "Installers/SettingsInstaller")]
    public class SettingsInstaller : ScriptableObjectInstaller<SettingsInstaller>
    {
        [SerializeField] private PlayerModel.Settings _playerSettings;
        
        public override void InstallBindings()
        {
            Container.Bind<PlayerModel.Settings>().FromInstance(_playerSettings).AsSingle().CopyIntoAllSubContainers().NonLazy();
        }
    }
}