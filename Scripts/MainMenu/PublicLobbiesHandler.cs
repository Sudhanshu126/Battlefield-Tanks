using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PublicLobbiesHandler : MonoBehaviour
{
    public static PublicLobbiesHandler Instance { get; private set; }

    [SerializeField] private PublicLobbyTemplate lobbyTemplatePrefab;
    [SerializeField] private Transform lobbyTemplateParent;
    [SerializeField] private int maxLobbiesShown;

    private Lobby joiningLobby;
    private bool isJoining, isRefreshing;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        RefreshList();
    }

    public async void JoinLobbyAsync(Lobby lobby)
    {
        if (isJoining) { return; }

        isJoining = true;
        try
        {
            joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joiningLobby.Data["JoinCode"].Value;

            await ClientSingleton.Instance.GameManager.StartClientAsync(joinCode);
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex.Message);
            return;
        }

        isJoining = false;
    }

    public async void RefreshList()
    {
        if(isRefreshing) { return; }
        isRefreshing = true;

        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = maxLobbiesShown;
            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0"),
                new QueryFilter(
                    field: QueryFilter.FieldOptions.IsLocked,
                    op: QueryFilter.OpOptions.EQ,
                    value: "0")
            };

            QueryResponse lobbiesToShow = await Lobbies.Instance.QueryLobbiesAsync(options);

            DestroyPreviousLobbyList();

            foreach(Lobby lobby in lobbiesToShow.Results)
            {
                if(lobby.IsPrivate) { continue; }

                PublicLobbyTemplate lobbyTemplate = Instantiate(lobbyTemplatePrefab, lobbyTemplateParent);
                lobbyTemplate.Initialize(lobby);
            }
        }
        catch(LobbyServiceException ex)
        {
            Debug.Log(ex.Message);
            return;
        }

        isRefreshing = false;
    }

    private void DestroyPreviousLobbyList()
    {
        foreach(Transform child in lobbyTemplateParent)
        {
            Destroy(child.gameObject);
        }
    }
}
