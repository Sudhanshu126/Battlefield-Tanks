using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton clientSingletonPrefab;
    [SerializeField] private HostSingleton hostSingletonPrefab;
    [SerializeField] private SceneLoader sceneLoader;

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = 60;

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
                //clientSingleton.GameManager.ChangeScene(SceneCode.MainMenu);
                sceneLoader.LoadScene(SceneCode.MainMenu);
            }
        }
    }
}
