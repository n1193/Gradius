using UnityEngine;
using Zenject;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] EnemyType enemyType;
    [SerializeField] int spawnCount = 1;
    [SerializeField] float spawnInterval = 0.5f;
    [SerializeField] bool drop = false;

    [SerializeField] SpawnPosition spawnPosition;

    [SerializeField] EnemyManager enemyManager;

    [Inject]
    public void Construct(EnemyManager enemyManager)
    {
        this.enemyManager = enemyManager;
    }
    void OnBecameVisible()
    {
        enemyManager.Instance(enemyType, transform.position, drop, spawnCount, spawnInterval,spawnPosition);
        Destroy(this.gameObject);
    }
}