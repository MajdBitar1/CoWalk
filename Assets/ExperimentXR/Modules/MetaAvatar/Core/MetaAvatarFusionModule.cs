using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;
using Oculus.Platform;
using System.Collections;
using UnityEngine.SceneManagement;

class MetaAvatarFusionModule : Fusion.Behaviour, INetworkRunnerCallbacks
{
      public static MetaAvatarFusionModule MetaAvatarModuleInstance = null;

    [Header("Use face and eye tracking for the Meta Avatar")]
    public bool FaceAndEyeTracking = true;
    //private GameObject _ovrRigCamera;

    private bool m_ModelLoaded = false;
    [SerializeField] private NetworkPrefabRef _playerPrefab;

    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    [SerializeField] private NetworkRunner _runner;

    public void Start()
    {
        MetaAvatarModuleInstance = this;
        Debug.Log("XPXR.MetaAvatar-Fusion: Starting of an other INetworkRunnerCallbacks");
        this._runner = FindObjectOfType<NetworkRunner>();
        if (this._runner is not null)
        {
            this._runner.AddCallbacks(this);
        }
        else
        {
            throw new UnityException("XPXR.MetaAvatar-Fusion: Impossible to find the Network Runner of the scene. Please make sure the Network Runner is in the scene before playing");
        }
    }

    public void OnDisable()
    {
        if (this._runner is not null)
        {
            this._runner.RemoveCallbacks(this);

        }
    }

    public void OnApplicationQuit()
    {
        if (this._spawnedCharacters.TryGetValue(this._runner.LocalPlayer, out NetworkObject networkObject))
        {
            this._runner.Despawn(networkObject);
            this._spawnedCharacters.Remove(this._runner.LocalPlayer);
        }
    }

    private void SpawnEntity(NetworkRunner runner)
    {
        m_ModelLoaded = true;
        Debug.Log("XPXR.MetaAvatar-Fusion: The host or user join the server");
        // Create a unique position for the player
        NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, null, null, runner.LocalPlayer);
        //networkPlayerObject.gameObject.tag = "Player";
        // Keep track of the player avatars so we can remove it when they disconnect
        _spawnedCharacters.Add(runner.LocalPlayer, networkPlayerObject);
        // Create custom data for the instantiated user;
    }

    #region NetworkRunner Callbacks
    public void OnConnectedToServer(NetworkRunner runner)
    {
        if (runner.GameMode == GameMode.Shared || runner.GameMode == GameMode.Host)
        {
            if (m_ModelLoaded == false)
            {
                SpawnEntity(runner);
                Debug.LogFormat("XPXR.MetaAvatar-Fusion: User instantiated, OnConnectedToServer");
            }
        }
    }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) 
    {
        if (m_ModelLoaded == false)
        {
            SpawnEntity(runner);
            // m_ModelLoaded = true;
            // Debug.Log("XPXR.MetaAvatar-Fusion: New Scene is Loading");
            // // Create a unique position for the player
            // NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, null, null, runner.LocalPlayer);
            // // Keep track of the player avatars so we can remove it when they disconnect
            // _spawnedCharacters.Add(runner.LocalPlayer, networkPlayerObject);
            // // Create custom data for the instantiated user;
            Debug.LogFormat("XPXR.MetaAvatar-Fusion: User instantiated, OnPlayerJoined");
        }
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) 
    {
        if (m_ModelLoaded == false)
        {
            SpawnEntity(runner);
            // m_ModelLoaded = true;
            // Debug.Log("XPXR.MetaAvatar-Fusion: New Scene is Loading");
            // // Create a unique position for the player
            // NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, null, null, runner.LocalPlayer);
            // // Keep track of the player avatars so we can remove it when they disconnect
            // _spawnedCharacters.Add(runner.LocalPlayer, networkPlayerObject);
            // // Create custom data for the instantiated user;
            Debug.LogFormat("XPXR.MetaAvatar-Fusion: User instantiated, OnSceneLoadDone");
        } 
    }
    public void OnSceneLoadStart(NetworkRunner runner) 
    { 
    }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    #endregion
}