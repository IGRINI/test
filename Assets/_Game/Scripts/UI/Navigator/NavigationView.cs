using System;
using System.Collections;
using Game.Network;
using Game.Utils;
using Game.Utils.Common;
using Game.Utils.Controllers;
using Game.Utils.Screens;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Game.UI.Navigator
{
    public class NavigationView : MonoBehaviour
    {
        [Inject] private readonly ScreenController _screenController;
        [Inject] private readonly ClientController _clientController;
        
        [SerializeField] private SelectedLight _selectLight;
        [SerializeField] private SelectButton _gameScreenButton;
        [SerializeField] private SelectButton _profileScreenButton;
        [SerializeField] private SelectButton _tavernScreenButton;
        [SerializeField] private SelectButton _shelterScreenButton;
        [SerializeField] private SelectButton _marketScreenButton;
        
        [SerializeField] private UIButton _settingsButton;
        [SerializeField] private UIButton _closeButton;

        [SerializeField] private TextMeshProUGUI _ping;
        [SerializeField] private ConnectionLoading _connectionView;

        private string _pingText;
        
        private void Start()
        {
            _gameScreenButton.OnClick.AddListener(() => _screenController.Show<GameScreen>());
            _profileScreenButton.OnClick.AddListener(() => _screenController.Show<ProfileScreen>());
            _tavernScreenButton.OnClick.AddListener(() => _screenController.Show<TavernScreen>());
            _shelterScreenButton.OnClick.AddListener(() => _screenController.Show<ShelterScreen>());
            _marketScreenButton.OnClick.AddListener(() => _screenController.Show<MarketScreen>());
            
            _gameScreenButton.OnClick.AddListener(() => SetSelected(_gameScreenButton));
            _profileScreenButton.OnClick.AddListener(() => SetSelected(_profileScreenButton));
            _tavernScreenButton.OnClick.AddListener(() => SetSelected(_tavernScreenButton));
            _shelterScreenButton.OnClick.AddListener(() => SetSelected(_shelterScreenButton));
            _marketScreenButton.OnClick.AddListener(() => SetSelected(_marketScreenButton));

            // _settingsButton.onClick;
            _closeButton.OnClick.AddListener(Application.Quit);

            // _gameScreenButton.OnClick.Invoke();

            _pingText = _ping.text;

            TryToConnect();
            _connectionView.TryAgain.AddListener(TryToConnect);

            StartCoroutine(PingSetter());
        }

        private IEnumerator PingSetter()
        {
            while (true)
            {
                _ping.text = string.Format(_pingText, _clientController.Ping);
                yield return new WaitForSeconds(1f);
            }
        }

        private void TryToConnect()
        {
            _connectionView.ShowLoading();
            _clientController.Connect("188.235.1.127", 30502);
        }

        private void OnEnable()
        {
            _clientController.ConnectionUpdate += ConnectionUpdate;
        }

        private void OnDisable()
        {
            _clientController.ConnectionUpdate -= ConnectionUpdate;
        }

        private void ConnectionUpdate(ClientController.ConnectInfo connectInfo)
        {
            Debug.Log(connectInfo.State);
            switch (connectInfo.State)
            {
                case ClientState.IS_CONNECTED:
                    _connectionView.HideScreen();
                    _gameScreenButton.OnClick.Invoke();
                    break;
                case ClientState.IS_DISCONNECTED:
                    _connectionView.ShowTryAgain();
                    break;
            }
        }

        private void SetSelected(UIButton button)
        {
            _gameScreenButton.MakeDeselected();
            _profileScreenButton.MakeDeselected();
            _tavernScreenButton.MakeDeselected();
            _shelterScreenButton.MakeDeselected();
            _marketScreenButton.MakeDeselected();
            
            _selectLight.SelectElement(button.RectTransform);
            // _selectBorder.DOSize
        }
    }
 
}
