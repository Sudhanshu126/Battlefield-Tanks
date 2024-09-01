using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MainGameUIController : MonoBehaviour
{
    public static MainGameUIController LocalInstance { get; private set; }

    [SerializeField] private List<Image> livesImages = new List<Image>();
    [field: SerializeField] public TMP_Text respawnText { get; private set; }

    private const string respawningText = "Respawning in ";

    private void Awake()
    {
        LocalInstance = this;
    }

    public void HandleLifeLost(int livesLeft, int respawnTime, ulong clientId)
    {
        livesImages[livesLeft].color = Color.black;
        StartCoroutine(RespawnCountDown(respawnTime, clientId));
    }

    public void UpdateRespawnCountdownUI(int time)
    {
        respawnText.text = respawningText + time.ToString();
    }

    private IEnumerator RespawnCountDown(int respawnTime, ulong clientId)
    {
        int spawnTime = respawnTime;
        UpdateRespawnCountdownUI(spawnTime);
        respawnText.gameObject.SetActive(true);

        while (spawnTime >= 0)
        {
            UpdateRespawnCountdownUI(spawnTime);
            yield return new WaitForSeconds(1);
            spawnTime--;
        }
        respawnText.gameObject.SetActive(false);

        PlayerController.LocalInstance.RespawnPlayerServerRpc(clientId);
    }
}
