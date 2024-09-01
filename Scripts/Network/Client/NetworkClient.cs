using System;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkClient : IDisposable
{
    private NetworkManager networkManager;
    public NetworkClient(NetworkManager networkManager)
    {
        this.networkManager = networkManager;

        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if(clientId != 0 && clientId != networkManager.LocalClientId) { return; }

        if(SceneManager.GetActiveScene().buildIndex != (int)SceneCode.MainMenu)
        {
            SceneManager.LoadScene((int)SceneCode.MainMenu);
        }

        if(networkManager.IsConnectedClient)
        {
            networkManager.Shutdown();
        }
    }

    public void Dispose()
    {
        if(networkManager != null)
        {
            networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        }
    }
}
