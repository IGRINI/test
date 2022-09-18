using Game.Network;
using UnityEngine;
using Zenject;

namespace Game.Utils
{
    public class ClientGui : MonoBehaviour
    {
        private ClientController _clientController;
        
        private string _ip = "109.195.51.158";
        private string _port = "30502";
        private string _nickName = "Keker";

        [Inject]
        private void Constructor(ClientController clientController)
        {
            _clientController = clientController;
        }
        
        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(450);
            GUILayout.BeginVertical();
            GUILayout.Label($"Client state: {RakClient.State} {_ip}:{_port}");
            switch (RakClient.State)
            {
                case ClientState.IS_DISCONNECTED:
                {
                    GUILayout.BeginHorizontal();
                        GUILayout.Label("IP: ");
                        _ip = GUILayout.TextField(_ip, GUILayout.Width(300));
                        GUILayout.Label("Port: ");
                        _port = GUILayout.TextField(_port, GUILayout.Width(70));
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                        GUILayout.Label("NickName: ");
                        _nickName = GUILayout.TextField(_nickName);
                    GUILayout.EndHorizontal();
                    if(GUILayout.Button("Connect to server"))
                    {
                        _clientController.SetNickName(_nickName);
                        _clientController.Connect(_ip, ushort.Parse(_port));
                    }

                    break;
                }
                case ClientState.IS_CONNECTING:
                    GUILayout.Label("Client connecting...");
                    break;
                default:
                {
                    if (GUILayout.Button("Disconnect"))
                    {
                        _clientController.Disconnect();
                    }

                    GUILayout.Box("Ping: "+RakClient.Ping);
                    GUILayout.Box("Average ping: "+RakClient.AveragePing);
                    GUILayout.Box("Lowest ping: "+RakClient.LowestPing);
                    GUILayout.Box("Connection time: "+RakClient.Statistics.ConnectionTime());
                    GUILayout.Box("Bytes received: "+RakClient.Statistics.GetStatsTotal(RNSPerSecondMetrics.ACTUAL_BYTES_RECEIVED));
                    GUILayout.Box("Bytes sended: "+RakClient.Statistics.GetStatsTotal(RNSPerSecondMetrics.ACTUAL_BYTES_SENT));
                    break;
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }
}