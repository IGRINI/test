using System;
using Game.Network;
using Game.PrefabsActions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

namespace Game.Utils
{
    public class ChatUi : MonoBehaviour, IClientPacketReader
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
            _scrollRect.verticalNormalizedPosition = 1f;
        }

        public void ReceivePacket(NetworkPackets.Packet packet)
        {
            if (packet is NetworkPackets.ServerChatMessage chatMessage)
            {
                AddChatLine(chatMessage.NickName, chatMessage.Text);
            }
        }
    }
}