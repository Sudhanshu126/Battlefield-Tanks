using Unity.Netcode;
using UnityEngine;

public class CoinSpawner : NetworkBehaviour
{
    [SerializeField] private RespawningCoin respawningCoinPrefab;
    [SerializeField] private int coinValue, maxCoinsInGame;
    [SerializeField] private Vector2 xSpawnRange, ySpawnRange;
    [SerializeField] private LayerMask layerMask;

    private float coinRadius;
    private Collider2D[] coinBuffer = new Collider2D[1];

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        coinRadius = respawningCoinPrefab.GetComponent<CircleCollider2D>().radius;

        for(int i = 0; i < maxCoinsInGame; i++)
        {
            SpawnCoin();
        }
    }

    private Vector2 GetRandomSpawnPoint()
    {
        Vector2 spawnPoint = Vector2.zero;
        bool foundCoinSpawnPoint = false;

        while (!foundCoinSpawnPoint)
        {
            float xPosition = Random.Range(xSpawnRange.x, xSpawnRange.y);
            float yPosition = Random.Range(ySpawnRange.x, ySpawnRange.y);
            spawnPoint.x = xPosition;
            spawnPoint.y = yPosition;

            int numColliders = Physics2D.OverlapCircleNonAlloc(spawnPoint, coinRadius, coinBuffer, layerMask);
            if(numColliders == 0)
            {
                foundCoinSpawnPoint = true;
            }
        }

        return spawnPoint;
    }

    private void SpawnCoin()
    {
        RespawningCoin coinInstance = Instantiate(respawningCoinPrefab, GetRandomSpawnPoint(), Quaternion.identity);
        
        coinInstance.SetCoinValue(coinValue);
        coinInstance.GetComponent<NetworkObject>().Spawn();

        coinInstance.onCollected += HandleCoinCollected;
    }

    private void HandleCoinCollected(RespawningCoin coin)
    {
        coin.ResetCoin(GetRandomSpawnPoint());
    }
}
