using Unity.Netcode;
using UnityEngine;

public class JoinServer : MonoBehaviour
{
    public void Host()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void Join()
    {
        NetworkManager.Singleton.StartClient();
    }
}
