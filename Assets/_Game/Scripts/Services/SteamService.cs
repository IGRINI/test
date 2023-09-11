using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Utils;
using Steamworks;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Game.Services
{
    public class SteamService : IInitializable, ITickable, IDisposable
    {
        public static readonly AppId_t STEAM_ID = new(480);
        public const string LOBBY_HOST_ID_KEY = "HostSteamID";
        
        public static CSteamID SteamID => _mySteamId;
        private static CSteamID _mySteamId;

        private bool _initialized;
        private static Dictionary<CSteamID, Sprite> _cacheAvatars = new();

        public bool IsInLobby => _partyCreated;
        private bool _partyCreated;
        private bool _partyIsLoading;
        private CSteamID _partyHostSteamID;
        private CSteamID _partySteamId;
        private readonly UniTaskCompletionSource<bool> _partyLoadingAwaiter = new();
        
        public readonly ReactiveCommand LobbyClosed = new();
        public readonly ReactiveCollection<CSteamID> LobbyMembers = new();

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
            InitPartyCallbacks();
            _mySteamId = SteamUser.GetSteamID();
            if (_initialized) return;
            
            Debug.LogError("[Steamworks.NET] SteamAPI_Init() failed.");
            Application.Quit();
        }

        private void InitPartyCallbacks()
        {
            Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
            Callback<LobbyEnter_t>.Create(OnLobbyEnter);
            Callback<LobbyInvite_t>.Create(OnLobbyInvite);
            Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
            Callback<LobbyKicked_t>.Create(OnLobbyKicked);
        }

        public string GetUserName(CSteamID steamID = default)
        {
            // if (steamID == default)
            // {
            //     steamID = SteamID;
            // }

            // var info = SteamFriends.GetPersonaName();
            return SteamFriends.GetPersonaName();
        }

        private void OnLobbyKicked(LobbyKicked_t param)
        {
            _partyCreated = _partyIsLoading = false;
            _partySteamId = default;
            LobbyClosed.Execute();
            Dev.Log($"{param.m_ulSteamIDAdmin} {param.m_ulSteamIDLobby}");
        }

        private void AcquireLaunchCommandLine()
        {
            if( SteamApps.GetLaunchCommandLine( out var launchCmd, 260 ) > 0 )
                Dev.Log($"AcquireLaunchCommandLine {launchCmd}");
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

        public Sprite GetAvatar(CSteamID steamID = default)
        {
            if (!_initialized)
                return null;
            
            if (steamID == default)
                steamID = _mySteamId;
            if (_cacheAvatars.TryGetValue(steamID, out var avatar))
                return avatar;
            var ret = SteamFriends.GetLargeFriendAvatar(steamID);
            var myAvatar = GetSteamImageAsTexture2D(ret);
            var sprite = Sprite.Create(myAvatar, new Rect(0, 0, myAvatar.width, myAvatar.height), new Vector2(.5f, .5f),
                100f);
            _cacheAvatars.Add (steamID, sprite);
            return sprite;
        }

        public async UniTask<bool> CreateLobbyOrInvite()
        {
            if (!_initialized)
                return false;
            
            if (_partyCreated)
            {
                SteamFriends.ActivateGameOverlayInviteDialog(_partySteamId);
                return true;
            }

            if (_partyIsLoading)
                return await _partyLoadingAwaiter.Task;
            _partyIsLoading = true;
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 3);
            SteamFriends.ActivateGameOverlayInviteDialog(SteamID);
            return true;
        }

        private void OnLobbyCreated(LobbyCreated_t callback)
        {
            if (callback.m_eResult != EResult.k_EResultOK)
            {
                _partyLoadingAwaiter.TrySetResult(_partyCreated = _partyIsLoading = false);
                return;
            }

            _partySteamId = new CSteamID(callback.m_ulSteamIDLobby);

            SteamMatchmaking.SetLobbyData(_partySteamId, LOBBY_HOST_ID_KEY, SteamID.ToString());
            SteamFriends.ActivateGameOverlayInviteDialog(_partySteamId);
            _partyIsLoading = false;
            _partyLoadingAwaiter.TrySetResult(_partyCreated = true);
        }

        private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
        {
            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }

        private void OnLobbyEnter(LobbyEnter_t callback)
        {
            _partySteamId = new CSteamID(callback.m_ulSteamIDLobby);
            _partyCreated = true;
            _partyHostSteamID = new CSteamID(ulong.Parse(SteamMatchmaking.GetLobbyData(_partySteamId, LOBBY_HOST_ID_KEY)));

            Dev.Log($"OnLobbyEnter {callback.m_rgfChatPermissions} {callback.m_EChatRoomEnterResponse} {callback.m_bLocked} {_partySteamId}");
        }

        private void OnLobbyInvite(LobbyInvite_t param)
        {
            Dev.Log($"OnLobbyInvite {param.m_ulSteamIDLobby} {param.m_ulSteamIDUser} {param.m_ulGameID}");
        }

        public void LeaveLobby()
        {
            if (!_initialized)
                return;
            if(!IsInLobby)
                return;
            
            SteamMatchmaking.LeaveLobby(_partySteamId);
            _partyCreated = _partyIsLoading = false;
            LobbyMembers.Clear();
            Dev.Log($"LeaveLobby {_partySteamId}");
        }

        private void OnLobbyChatUpdate(LobbyChatUpdate_t callback)
        {
            if ((EChatMemberStateChange)callback.m_rgfChatMemberStateChange == EChatMemberStateChange.k_EChatMemberStateChangeEntered)
            {
                var id = new CSteamID(callback.m_ulSteamIDUserChanged);

                if (_partyHostSteamID == id)
                {
                    SteamMatchmaking.LeaveLobby(_partySteamId);
                    _partyCreated = _partyIsLoading = false;
                    LobbyClosed.Execute();
                }
                LobbyMembers.Add(id);
            }
            else
            {
                var id = new CSteamID(callback.m_ulSteamIDUserChanged);
                LobbyMembers.Add(id);
            }
            if((EChatMemberStateChange)callback.m_rgfChatMemberStateChange == EChatMemberStateChange.k_EChatMemberStateChangeLeft)
            {
                LobbyMembers.Remove(new CSteamID(callback.m_ulSteamIDUserChanged));
            }

            Dev.Log($"nOnChatJoined {LobbyMembers.Count} OnChatJoined {callback.m_rgfChatMemberStateChange} {callback.m_ulSteamIDLobby} {callback.m_ulSteamIDMakingChange} {callback.m_ulSteamIDUserChanged}");
            
        }

        private static Texture2D GetSteamImageAsTexture2D(int iImage) {

            Texture2D FlipTexture(Texture2D original)
            {
                var flipped = new Texture2D(original.width, original.height);
     
                var xN = original.width;
                var yN = original.height;
     
                for(var i=0;i<xN;i++)
                {
                    for(var j=0;j<yN;j++)
                    {
                        flipped.SetPixel(i, yN-j-1, original.GetPixel(i,j));
                    }
                }

                flipped.Apply();
     
                return flipped;
            }
            
            Texture2D ret = null;
            uint ImageWidth;
            uint ImageHeight;
            var bIsValid = SteamUtils.GetImageSize(iImage, out ImageWidth, out ImageHeight);

            if (!bIsValid) return ret;
            var Image = new byte[ImageWidth * ImageHeight * 4];

            bIsValid = SteamUtils.GetImageRGBA(iImage, Image, (int)(ImageWidth * ImageHeight * 4));
            if (!bIsValid) return ret;
            ret = new Texture2D((int)ImageWidth, (int)ImageHeight, TextureFormat.RGBA32, false, true);
            ret.LoadRawTextureData(Image);
            ret.Apply();
            
            var result = FlipTexture(ret);
            Object.DestroyImmediate (ret);
            
            return result;
        }
    }
}