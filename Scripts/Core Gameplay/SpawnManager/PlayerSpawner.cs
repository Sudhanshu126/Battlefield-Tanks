using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    public static PlayerSpawner Instance { get; private set; }

    [SerializeField] private List<Transform> respawnPoints = new List<Transform>();

    private List<Transform> spawnPoints = new List<Transform>();

    private void Awake()
    {
        Instance = this;
        SetSpawnPoints();
    }

    public Transform GetRandomRespawnPoint()
    {
        if (respawnPoints.Count == 0)
        {
            return transform;
        }

        return respawnPoints[Random.Range(0, respawnPoints.Count)];
    }

    public Transform GetRandomSpawnPoint()
    {
        if(spawnPoints.Count == 0)
        {
            return transform;
        }
        Transform spawnPoint = spawnPoints[Random.Range(0,spawnPoints.Count)];
        spawnPoints.Remove(spawnPoint);
        return spawnPoint;
    }

    void SetSpawnPoints()
    {
        foreach (Transform t in respawnPoints)
        {
            spawnPoints.Add(t);
        }
    }
}
