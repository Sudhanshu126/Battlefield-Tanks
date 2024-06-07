using TMPro;
using UnityEngine;

public class MainMenuUIController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu, playMenu;
    [SerializeField] private TMP_InputField joinCodeInput, nicknameInput;
    [SerializeField] private TMP_Text nicknameText, nicknameWarningText;

    private PlayerData playerData;

    private const string defaultPlayerName = "Player";

    private void Start()
    {
        DisableAllMenu();
        mainMenu.SetActive(true);

        LocalDataHandler.Instance.LoadData();
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
}
