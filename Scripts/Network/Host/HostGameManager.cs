using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using System.Collections;
using System.Text;
using Unity.Services.Authentication;

public class HostGameManager : IDisposable
{
    public NetworkServer NetworkServer { get; private set; }

    private const int maxConnections = 10;
    private Allocation allocation;
    private string joinCode, lobbyId;
    private float lobbyHeartBeat = 15f;

    public async Task StartHostAsync()
    {
        try
        {
            allocation =  await Relay.Instance.CreateAllocationAsync(maxConnections);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return;
        }

        try
        {
            joinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return;
        }

        UnityTransport transport =  NetworkManager.Singleton.GetComponent<UnityTransport>();

        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
        transport.SetRelayServerData(relayServerData);

        try
        {
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
            lobbyOptions.IsPrivate = false;
            lobbyOptions.Data = new Dictionary<string, DataObject>()
            {
                {
                    "JoinCode", new DataObject(
                        visibility: DataObject.VisibilityOptions.Member,
                        value: joinCode
                    )
                }
            };

            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync($"{LocalDataHandler.Instance.PlayerData.nickName}'s lobby", maxConnections, lobbyOptions);

            lobbyId = lobby.Id;

            HostSingleton.Instance.StartCoroutine(HeartBeatLobby(lobbyHeartBeat));
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogError(ex.Message);
            return;
        }

        NetworkServer = new NetworkServer(NetworkManager.Singleton);
        SharedUserData userData = new SharedUserData
        {
            nickName = LocalDataHandler.Instance.PlayerData.nickName,
            authenticationId = AuthenticationService.Instance.PlayerId
        };
        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

        /* Disabled for debug   ========
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene(SceneCode.MainGame.ToString(), LoadSceneMode.Single);
        Debug.Log("HOST STARTED");
        */

        //DEBUG
        MainMenuUIController.Instance.GetSceneLoader().onSceneLoaded += OnSceneLoaded;
        MainMenuUIController.Instance.GetSceneLoader().LoadScene(SceneCode.MainGame);
    }

    private void OnSceneLoaded()
    {
        NetworkManager.Singleton.StartHost();
    }

    private IEnumerator HeartBeatLobby(float waitForTimeSeconds)
    {
        WaitForSecondsRealtime lobbyHeartRate = new WaitForSecondsRealtime(waitForTimeSeconds);
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return lobbyHeartRate;
        }
    }

    public async void Dispose()
    {
        HostSingleton.Instance.StopCoroutine(nameof(HeartBeatLobby));
        MainMenuUIController.Instance.GetSceneLoader().onSceneLoaded -= OnSceneLoaded;

        if (!string.IsNullOrEmpty(lobbyId))
        {
            try
            {
                await Lobbies.Instance.DeleteLobbyAsync(lobbyId);
            }
            catch(LobbyServiceException ex)
            {
                Debug.LogError(ex.Message);
                return;
            }

            lobbyId = string.Empty;
        }

        NetworkServer?.Dispose();
    }
}
