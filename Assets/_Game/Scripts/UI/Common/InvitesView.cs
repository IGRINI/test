using System;
using Game.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Utils.Common
{
    public class InvitesView : MonoBehaviour
    {
        [Inject] private readonly SteamService _steamService;
        
        [SerializeField] private UIButton _myButton;
        [SerializeField] private UIButton _firstFriend;
        [SerializeField] private UIButton _secondFriend;
        [SerializeField] private UIButton _lobbyButton;
        [SerializeField] private TextMeshProUGUI _lobbyButtonText;
        
        private Texture2D _mySmallAvatar;

        private void Awake()
        {
            var avatar = _steamService.GetAvatar();
            var sprite = Sprite.Create(avatar, new Rect(0, 0, avatar.width, avatar.height), new Vector2(.5f, .5f), 100);
            _myButton.GetComponent<Image>().sprite
        }
    }
}