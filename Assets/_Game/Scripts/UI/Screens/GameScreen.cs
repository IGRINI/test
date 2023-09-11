using System;
using Game.Services;
using Game.Utils.Common;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game.Utils.Screens
{
    public class GameScreen : Screen
    {
        [Inject] private readonly SteamService _steamService;
        
        [SerializeField] private InvitesView _invitesView;
        [SerializeField] private TextMeshProUGUI _playerName;

        protected override void OnInitialize(Action onComplete = null)
        {
            base.OnInitialize(onComplete);

            _playerName.text = _steamService.GetUserName();
        }
    }
}