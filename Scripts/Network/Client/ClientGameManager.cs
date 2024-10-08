using System;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager : IDisposable
{
    private JoinAllocation joinAllocation;
    private NetworkClient networkClient;

    public async Task<bool> InitializeAsync()
    {
        await UnityServices.InitializeAsync();

        networkClient = new NetworkClient(NetworkManager.Singleton);

        AuthenticationState authState = await AuthenticationHandler.Authenticate();

        if(authState == AuthenticationState.Authenticated)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public void ChangeScene(SceneCode sceneCode)
    {
        SceneManager.LoadScene((int) sceneCode);
    }

    public async Task StartClientAsync(string joinCode)
    {
        try
        {
            joinAllocation = await Relay.Instance.JoinAllocationAsync(joinCode);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

        transport.SetRelayServerData(relayServerData);

        SharedUserData userData = new SharedUserData
        {
            nickName = LocalDataHandler.Instance.PlayerData.nickName,
            authenticationId = AuthenticationService.Instance.PlayerId
        };
        string payload = JsonUtility.ToJson(userData    );
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

        NetworkManager.Singleton.StartClient();
    }

    public void Dispose()
    {
        networkClient?.Dispose();
    }
}
