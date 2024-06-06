using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton clientSingletonPrefab;
    [SerializeField] private HostSingleton hostSingletonPrefab;

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);

        await LoadInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async Task LoadInMode(bool isDedicatedServer)
    {
        if(isDedicatedServer)
        {

        }
        else
        {
            HostSingleton hostSingleton = Instantiate(hostSingletonPrefab);
            hostSingleton.CreateHost();

            ClientSingleton clientSingleton = Instantiate(clientSingletonPrefab);
            bool authenticated =  await clientSingleton.CreateClient();

            if(authenticated)
            {
                clientSingleton.GameManager.ChangeScene(SceneCode.MainMenu);
            }
        }
    }
}
