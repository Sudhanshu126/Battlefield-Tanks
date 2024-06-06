using UnityEngine;

public class HostSingleton : MonoBehaviour
{
    public static HostSingleton Instance { get; private set; }
    public HostGameManager GameManager { get; private set; }

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

    public void CreateHost()
    {
        GameManager = new HostGameManager();
    }
}
