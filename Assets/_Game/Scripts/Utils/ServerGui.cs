using Game.Controllers.Network;
using UnityEngine;
using Zenject;

namespace Game.Utils
{
    public class ServerGui : MonoBehaviour
    {
        private ServerController _serverController;

        [Inject]
        private void Constructor(ServerController serverController)
        {
            _serverController = serverController;
        }
        
        private void OnGUI()
        {
            if (RakServer.State is ServerState.NOT_STARTED or ServerState.STOPPED)
            {
                if (GUILayout.Button("Start Server"))
                {
                    RakServer.Start("127.0.0.1", 30502);
                }
            }
            else
            {
                if (GUILayout.Button("Stop Server"))
                {
                    RakServer.Stop();
                }

                GUILayout.Box("Connected clients");

                foreach (var data in _serverController.Clients)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Box(data.PlayerName);
                    if (GUILayout.Button("Kick"))
                    {
                        RakServer.CloseConnection(data.Guid, true);
                    }

                    if (GUILayout.Button("Ban"))
                    {
                        RakServer.AddBanIP(RakServer.GetAddress(data.Guid, false));
                        RakServer.CloseConnection(data.Guid, true);
                    }

                    GUILayout.EndHorizontal();
                }
            }
        }
    }
}