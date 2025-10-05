using UnityEngine;
using Zenject;


public class SpawnPoint : MonoBehaviour
{
    [SerializeField] public EnemyType enemyType;
    [SerializeField] public int spawnCount = 1;
    [SerializeField] public float spawnInterval = 0.5f;
    [SerializeField] public bool drop = false;

    [SerializeField] EnemyManager enemyManager;

    [Inject]
    public void Construct(EnemyManager enemyManager)
    {
        this.enemyManager = enemyManager;
    }
    void OnBecameVisible()
    {
        //enemyManager.Instance(enemyType, transform.position, drop, spawnCount, spawnInterval, spawnPosition);
        Destroy(this.gameObject);
    }
}