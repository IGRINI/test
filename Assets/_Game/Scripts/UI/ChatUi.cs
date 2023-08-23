using System;
using System.Threading;
using Game.Network;
using Game.PrefabsActions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using Zenject;

namespace Game.Utils
{
    public class ChatUi : MonoBehaviour
    {
        private PrefabCreator _prefabCreator;
        private ClientController _clientController;
        
        [SerializeField]
        private ScrollRect _scrollRect;
        [SerializeField]
        private TMP_Text _chatTextPrefab;
        [SerializeField]
        private TMP_InputField _chatInput;

        [Inject]
        private void Constructor(PrefabCreator prefabCreator,
            ClientController clientController)
        {
            _prefabCreator = prefabCreator;
            _clientController = clientController;
        }
        
        private void Start()
        {
            _chatInput.onSubmit.AddListener(OnSubmit);
            _clientController.OnPacketReceived += ReceivePacket;
        }

        private void OnDestroy()
        {
            _clientController.OnPacketReceived -= ReceivePacket;
        }

        private void OnSubmit(string text)
        {
            AddChatLine(_clientController.NickName, text);
            _chatInput.text = "";
            _clientController.SendPacket(new NetworkPackets.ClientChatMessage()
            {
                Text = text
            });
        }

        public void AddChatLine(string nick, string text)
        {
            _prefabCreator.Create<TMP_Text>(_chatTextPrefab, _scrollRect.content).text = $"[{nick}]: {text}";
            LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollRect.content);
            Observable.NextFrame()
                .Subscribe(x =>
                {
                    _scrollRect.verticalNormalizedPosition = 0f;
                });
        }

        public void ReceivePacket(NetworkPackets.Packet packet)
        {
            Debug.Log(Thread.CurrentThread.ManagedThreadId);
            if (packet is NetworkPackets.ServerChatMessage chatMessage)
            {
                AddChatLine(chatMessage.NickName, chatMessage.Text);
            }
        }
    }
}