using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager networkManager;
    private Dictionary<ulong, string> clientIdToAuthId = new Dictionary<ulong, string>();
    private Dictionary<string, SharedUserData> authIdToUserData = new Dictionary<string, SharedUserData>();

    public NetworkServer(NetworkManager networkManager)
    {
        this.networkManager = networkManager;

        //This event is triggered whenever someone connects to the server
        networkManager.ConnectionApprovalCallback += ConnectionApproval;

        networkManager.OnServerStarted += OnNetworkReady;
    }

    //Subscriber to the connection approval callback, runs when someone connects to the server
    private void ConnectionApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        string payload = Encoding.UTF8.GetString(request.Payload);
        SharedUserData userData = JsonUtility.FromJson<SharedUserData>(payload);

        clientIdToAuthId[request.ClientNetworkId] = userData.authenticationId;
        authIdToUserData[userData.authenticationId] = userData;

        response.CreatePlayerObject = true;
        Transform spawnPoint = PlayerSpawner.Instance.GetRandomSpawnPoint();
        response.Position = spawnPoint.position;
        //response.Rotation = spawnPoint.rotation;
        response.Approved = true;
    }

    private void OnNetworkReady()
    {
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if(clientIdToAuthId.TryGetValue(clientId, out string authId))
        {
            clientIdToAuthId.Remove(clientId);
            authIdToUserData.Remove(authId);
        }
    }

    public void Dispose()
    {
        if(networkManager != null)
        {
            networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
            networkManager.ConnectionApprovalCallback -= ConnectionApproval;
            networkManager.OnServerStarted -= OnNetworkReady;

            if(networkManager.IsListening)
            {
                networkManager.Shutdown();
            }
        }
    }

    public SharedUserData GetSharedUserData(ulong clientId)
    {
        if(clientIdToAuthId.TryGetValue(clientId, out string authId))
        {
            if (authIdToUserData.TryGetValue(authId, out SharedUserData sharedUserData))
            {
                return sharedUserData;
            }
        }

        return null;
    }
}
