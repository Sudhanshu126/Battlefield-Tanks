using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    public static PlayerSpawner Instance { get; private set; }

    [SerializeField] private List<Transform> respawnPoints = new List<Transform>();

    private List<Transform> spawnPoints = new List<Transform>();

    //Awake method
    private void Awake()
    {
        Instance = this;
        SetSpawnPoints();
    }

    //Gets random respawn point after player is killed
    public Transform GetRandomRespawnPoint()
    {
        if (respawnPoints.Count == 0)
        {
            return transform;
        }

        return respawnPoints[Random.Range(0, respawnPoints.Count)];
    }

    //Gets random spawn point when player joins the game
    public Transform GetRandomSpawnPoint()
    {
        if(spawnPoints.Count == 0)
        {
            return transform;
        }
        Transform spawnPoint = spawnPoints[Random.Range(0,spawnPoints.Count)];

        //Removes a spawn point once used so no two player spawns on same spawn point
        spawnPoints.Remove(spawnPoint);
        return spawnPoint;
    }

    //Makes a copy of respawn Point into spawn point
    private void SetSpawnPoints()
    {
        foreach (Transform t in respawnPoints)
        {
            spawnPoints.Add(t);
        }
    }
}
