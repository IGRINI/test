﻿using Cysharp.Threading.Tasks;
using Game.Services;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Utils.Common
{
    public class InvitesView : MonoBehaviour
    {
        [Inject] private readonly SteamService _steamService;
        
        [SerializeField] private InviteButton _myButton;
        [SerializeField] private InviteButton _firstFriend;
        [SerializeField] private InviteButton _secondFriend;
        [SerializeField] private UIButton _lobbyButton;
        [SerializeField] private TextMeshProUGUI _lobbyButtonText;

        private Texture2D _mySmallAvatar;

        private void Start()
        {
            var avatar = _steamService.GetAvatar();
            _myButton.SetButtonInfo(avatar);
            
            _myButton.OnClick.AddListener(CreateLobby);
            _firstFriend.OnClick.AddListener(CreateLobby);
            _secondFriend.OnClick.AddListener(CreateLobby);
            _lobbyButton.OnClick.AddListener(LobbyButtonCallback);

            _steamService.LobbyClosed.Subscribe(_ =>
            {
                _firstFriend.SetButtonInfo();
                _secondFriend.SetButtonInfo();
                _lobbyButtonText.text = "Invite";
            });

            _steamService.LobbyMembers.ObserveCountChanged()
                .Subscribe(x =>
                {
                    switch (x)
                    {
                        case 0:
                            _firstFriend.SetButtonInfo();
                            _secondFriend.SetButtonInfo();
                            break;
                        case 1:
                            _firstFriend.SetButtonInfo(_steamService.GetAvatar(_steamService.LobbyMembers[0]));
                            _secondFriend.SetButtonInfo();
                            break;
                        case 2:
                            _firstFriend.SetButtonInfo(_steamService.GetAvatar(_steamService.LobbyMembers[0]));
                            _secondFriend.SetButtonInfo(_steamService.GetAvatar(_steamService.LobbyMembers[1]));
                            break;
                    }
                });
        }

        private async void CreateLobby()
        {
            var lobby = await _steamService.CreateLobbyOrInvite();

            _lobbyButtonText.text = lobby ? "Leave" : "Invite";
        }

        private void LobbyButtonCallback()
        {
            if (_steamService.IsInLobby)
            {
                _steamService.LeaveLobby();
                _lobbyButtonText.text = "Invite";
                return;
            }

            CreateLobby();
        }
    }
}