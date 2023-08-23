using System;
using Game.Utils.Controllers;
using Game.Utils.Screens;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Utils
{
    public class NavigationView : MonoBehaviour
    {
        [Inject] private ScreenController _screenController;
        [SerializeField] private Button _gameScreenButton;
        [SerializeField] private Button _profileScreenButton;
        [SerializeField] private Button _tavernScreenButton;
        [SerializeField] private Button _shelterScreenButton;
        [SerializeField] private Button _marketScreenButton;
        
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _closeButton;

        private void Awake()
        {
            _gameScreenButton.onClick.AddListener(() => _screenController.Show<GameScreen>());
            _profileScreenButton.onClick.AddListener(() => _screenController.Show<ProfileScreen>());
            _tavernScreenButton.onClick.AddListener(() => _screenController.Show<TavernScreen>());
            _shelterScreenButton.onClick.AddListener(() => _screenController.Show<ShelterScreen>());
            _marketScreenButton.onClick.AddListener(() => _screenController.Show<MarketScreen>());

            // _settingsButton.onClick;
            _closeButton.onClick.AddListener(Application.Quit);
        }
    }
 
}
