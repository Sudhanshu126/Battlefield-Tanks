using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PublicLobbyTemplate : MonoBehaviour
{
    [SerializeField] private TMP_Text lobbyNameText, playersCountText;

    private Lobby assignedLobby;

    public void Initialize(Lobby lobby)
    {
        string playerCount = $"{lobby.Players.Count} / {lobby.MaxPlayers}";

        assignedLobby = lobby;
        lobbyNameText.text = lobby.Name;
        playersCountText.text = playerCount;
    }

    public void JoinThisLobby()
    {
        MainMenuUIController.Instance.ShowLoadingScreen();
        PublicLobbiesHandler.Instance.JoinLobbyAsync(assignedLobby);
    }
}
