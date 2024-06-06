using TMPro;
using UnityEngine;

public class MainMenuUIController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu, playMenu;
    [SerializeField] private TMP_InputField joinCodeInput;

    private void Start()
    {
        DisableAllMenu();
        mainMenu.SetActive(true);
    }

    private void DisableAllMenu()
    {
        mainMenu.SetActive(false);
        playMenu.SetActive(false);
    }

    public void LoadUI(GameObject uiToLoad)
    {
        DisableAllMenu();
        uiToLoad.SetActive(true);
    }

    public void LoadUIOnTop(GameObject uiToLoad)
    {
        uiToLoad.SetActive(true);
    }

    public async void StartHostAsync()
    {
        await HostSingleton.Instance.GameManager.StartHostAsync();
    }

    public async void JoinGame()
    {
        string joinCode = joinCodeInput.text;
        if(!string.IsNullOrEmpty(joinCode))
        {
            await ClientSingleton.Instance.GameManager.StartClientAsync(joinCode);
        }
    }
}
