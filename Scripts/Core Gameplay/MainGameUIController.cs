using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainGameUIController : MonoBehaviour
{
    public static MainGameUIController LocalInstance { get; private set; }

    [SerializeField] private List<Image> livesImages = new List<Image>();
    [SerializeField] private GameObject healthPanel, deathText;
    [field: SerializeField] public TMP_Text respawnText { get; private set; }

    private const string respawnString = "Respawning in ";

    //Awake method
    private void Awake()
    {
        LocalInstance = this;
    }


    //Runs as soon as a player loses a life for its local game UI
    public void HandleLifeLost(int livesLeft, int respawnTime, ulong clientId)
    {
        livesImages[livesLeft].color = Color.black;
        if(livesLeft > 0)
        {
            StartCoroutine(RespawnCountDown(respawnTime, clientId));
        }
        else
        {
            healthPanel.SetActive(false);
            deathText.SetActive(true);
        }
    }

    //Updates the respawn text
    public void UpdateRespawnCountdownUI(int time)
    {
        respawnText.text = respawnString + time.ToString();
    }

    //Count downs the respawn counter
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
