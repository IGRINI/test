using System;
using Steamworks;
using UnityEngine;
using Zenject;

namespace Game.Services
{
    public class SteamService : IInitializable, ITickable, IDisposable
    {
        public static readonly AppId_t STEAM_ID = new(480);

        private bool _initialized;
        
        public void Initialize()
        {
            if (_initialized)
                return;
            
            if (!Packsize.Test()) {
                Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.");
            }

            if (!DllCheck.Test()) {
                Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.");
            }
            
            try {
                if (SteamAPI.RestartAppIfNecessary(STEAM_ID)) {
                    Application.Quit();
                }
            }
            catch (DllNotFoundException e) { // We catch this exception here, as it will be the first occurrence of it.
                Debug.LogError("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location.\n" + e);

                Application.Quit();
            }

            _initialized = SteamAPI.Init();

            AcquireLaunchCommandLine();
            if (_initialized) return;
            
            Debug.LogError("[Steamworks.NET] SteamAPI_Init() failed.");
            Application.Quit();
        }
        
        private void AcquireLaunchCommandLine( )
        {
            string launchCmd;
            if( SteamApps.GetLaunchCommandLine( out launchCmd, 260 ) > 0 )
                Debug.Log( $"Got Steam AcquireLaunchCommandLine '{launchCmd}'" );
        }

        public void Tick()
        {
            if (!_initialized)
                return;
            
            SteamAPI.RunCallbacks();
        }

        public void Dispose()
        {
            if (!_initialized)
                return;
            
            _initialized = false;
            SteamAPI.Shutdown();
        }
    }
}