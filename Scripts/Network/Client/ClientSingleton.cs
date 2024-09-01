using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{
    public static ClientSingleton Instance {  get; private set; }

    public ClientGameManager GameManager { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        GameManager?.Dispose();
    }

    public async Task<bool> CreateClient()
    {
        GameManager = new ClientGameManager();
        return await GameManager.InitializeAsync();
    }
}
