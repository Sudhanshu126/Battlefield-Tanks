using TMPro;
using UnityEngine;

public class MainMenuUIController : MonoBehaviour
{
    public static MainMenuUIController Instance {  get; private set; }

    [SerializeField] private GameObject mainMenu, playMenu, loadingScreen;
    [SerializeField] private TMP_InputField joinCodeInput, nicknameInput;
    [SerializeField] private TMP_Text nicknameText, nicknameWarningText;
    [SerializeField] private SceneLoader sceneLoader;

    private PlayerData playerData;

    private const string defaultPlayerName = "Player";

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        DisableAllMenu();
        mainMenu.SetActive(true);

        playerData = LocalDataHandler.Instance.PlayerData;
        ConfigureLocalPlayerData();
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
        ShowLoadingScreen();
        await HostSingleton.Instance.GameManager.StartHostAsync();
    }

    public async void JoinGame()
    {
        string joinCode = joinCodeInput.text;
        if(!string.IsNullOrEmpty(joinCode))
        {
            ShowLoadingScreen();
            await ClientSingleton.Instance.GameManager.StartClientAsync(joinCode);
        }
    }

    public void ShowLoadingScreen() => loadingScreen.SetActive(true);

    public void ChangeNickname()
    {
        string nickname = nicknameInput.text;
        if(!string.IsNullOrEmpty(nickname))
        {
            nicknameText.text = nickname;
            playerData.nickName = nickname;

            LocalDataHandler.Instance.SaveData();
        }
    }

    public void OnNicknameInputChanged()
    {
        string nickname = nicknameInput.text;
        bool canChangeNickname = true;

        if (string.IsNullOrEmpty(nickname))
        {
            nicknameWarningText.text = "Empty nickname";
            canChangeNickname = false;
        }
        else if(nickname[0] == ' ')
        {
            nicknameWarningText.text = "Invalid nickname";
            canChangeNickname = false;
        }

        nicknameWarningText.gameObject.SetActive(!canChangeNickname);
    }

    public void ConfigureLocalPlayerData()
    {
        if (playerData != null)
        {
            if (string.IsNullOrEmpty(playerData.nickName))
            {
                playerData.nickName = defaultPlayerName;
            }
            nicknameText.text = playerData.nickName;
            nicknameInput.text = playerData.nickName;
        }
    }

    public SceneLoader GetSceneLoader()
    {
        return sceneLoader;
    }
}
