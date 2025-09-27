using UnityEngine;
using Zenject;
using System.Collections;

public enum SpawnPosition
{
    ScreenRight,
    ScreenLeft,
    CustomPoint
}

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private EnemyManager enemyManager;

    [Inject] DiContainer _container;
    [Inject] DropManager _dropManager;   // DropManager をDI

    [Inject]
    public void Construct(EnemyManager enemyManager, DiContainer container, DropManager dropManager)
    {
        this.enemyManager = enemyManager;
        _container = container;
        _dropManager = dropManager;
    }

    // 末尾に drop を追加（既存呼び出しは壊れない）
    public void Spawn(Enemy enemyPrefab, Vector3 spawnPoint, int count = 1,
                      float spawenInterval = 0.25f,
                      SpawnPosition spawnPosition = SpawnPosition.CustomPoint,
                      bool drop = false)
    {
        if (enemyPrefab == null) return;
        StartCoroutine(SpawnWaves(enemyPrefab, spawnPoint, count, spawenInterval, spawnPosition, drop));
    }

    IEnumerator SpawnWaves(Enemy enemyPrefab, Vector3 spawnPoint, int count = 1,
                           float spawenInterval = 0.25f,
                           SpawnPosition spawnPosition = SpawnPosition.CustomPoint,
                           bool drop = false)
    {
        // AllKilled 用のグループ（必要な時だけ）
        var group = (drop && count > 0) ? _dropManager.CreateGroup(null, count) : null;

        for (int i = 0; i < count; i++)
        {
            // === 位置計算 ===
            Vector3 instancePosition;
            switch (spawnPosition)
            {
                case SpawnPosition.ScreenLeft:
                case SpawnPosition.ScreenRight:
                {
                    float x = (spawnPosition == SpawnPosition.ScreenLeft) ? 0f : 1f;
                    // Viewport→World。y は指定の spawnPoint.y を採用
                    instancePosition = Camera.main.ViewportToWorldPoint(new Vector3(x, 0f, 0f));
                    instancePosition.y = spawnPoint.y;
                    break;
                }
                default:
                    instancePosition = spawnPoint;
                    break;
            }
            instancePosition.z = 0f;

            // === 生成（位置が決まってから！）===
            var enemy = _container.InstantiatePrefabForComponent<Enemy>(
                enemyPrefab.gameObject, instancePosition, Quaternion.identity, this.transform);

            // DropGroup を渡す（drop=false の時は何もしない）
            if (group != null) enemy.SetDropGroup(group);

            if (spawenInterval > 0f && i < count - 1)
                yield return new WaitForSeconds(spawenInterval);
        }
    }
}
