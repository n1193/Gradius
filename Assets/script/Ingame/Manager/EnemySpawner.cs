using UnityEngine;
using System.Collections.Generic;
using System.Globalization;
using UnityEditorInternal;
using System.Collections;
using System;
using Zenject;
using Unity.VisualScripting;


public enum SpawnPosition
{
    Left,
    Right,
    CustomPoint
}
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private ScrollDirector scroll;
    [SerializeField] private Transform parentTransform;
    [SerializeField] private TextAsset tsvFile;  // ← ここにTSVを直接アサイン

    private List<SpawnPointData> spawnPoints = new();
    private int currentIndex = 0;
    private DropGroupManager dropGroupManager;
    private List<Coroutine> activeRoutines = new();
    private bool isSpawning = true;

    [Inject]
    public void Construct(EnemyManager enemyManager, ScrollDirector scrollDirector, DropGroupManager dropGroupManager)
    {
        this.enemyManager = enemyManager;
        scroll = scrollDirector;
        this.dropGroupManager = dropGroupManager;

    }
    void Start()
    {
        parentTransform = this.transform;
        StartCoroutine(DelayedLoad());
    }

    void Update()
    {
        if (currentIndex >= spawnPoints.Count) return;

        var d = spawnPoints[currentIndex];
        //Debug.Log($"[EnemySpawner] checking {d.enemyType} at X={d.x}");
        if (scroll.X >= d.x)
        //if (true) // ← scroll.CurrentX 条件を外して常に発火
        {
            StartSpawnRoutine(d);
            currentIndex++;
        }
    }

    private void LoadSpawnPoints()
    {
        tsvFile = Resources.Load<TextAsset>("EnemySpawnData");
        if (tsvFile == null)
        {
            Debug.LogError("[EnemySpawner] TSV未設定");
            return;
        }

        var rawRows = SimpleTsv.Parse(tsvFile, new SimpleTsv.Options { CommentPrefix = "#", HeaderMarker = "##" });
        var rows = SimpleTsv.Wrap(rawRows);
        spawnPoints.Clear();
        int line = 0;
        foreach (var r in rows)
        {
            line++;
            // 必須: enemyType / x / y
            var enemy = r.GetEnum("enemyType", EnemyType.Fan);
            float x = r.GetFloat("x", float.NaN);
            float y = r.GetFloat("y", float.NaN);

            if (float.IsNaN(x) || float.IsNaN(y))
            {
                Debug.LogError($"[TSV] 必須列欠落 line {line}: {r}");
                continue;
            }

            try
            {
                var data = new SpawnPointData
                {
                    enemyType = enemy,
                    x = x,
                    y = r.GetFloat("y", 0f),
                    count = r.GetInt("count", 1),
                    interval = r.GetFloat("interval", 0.25f),
                    drop = r.GetBool("drop", false),
                    spawnPos = r.Get("spawnPos", "CustomPoint"),  // 必要なら Enum 化して使う
                    delay = r.GetFloat("delay", 0f),
                };
                spawnPoints.Add(data);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[TSV行パース失敗]: {e.Message}");
            }
        }

        Debug.Log($"[EnemySpawner] {spawnPoints.Count}行ロード完了");
    }
    
    public void StartSpawnRoutine(SpawnPointData data)
    {
        if (!isSpawning) return;
        Coroutine co = StartCoroutine(SpawnRoutine(data));
        activeRoutines.Add(co);
    }
    private IEnumerator SpawnRoutine(SpawnPointData d)
    {
        if (d.delay > 0f) yield return new WaitForSeconds(d.delay);
        DropGroup dropGroup = null;
        if (d.drop)
        {
            dropGroup = dropGroupManager.CreateGroup(d.count);
        }
        var cam = Camera.main;
        float spawnX = 0f;
        if (d.spawnPos == "Right")
        {
            spawnX = cam.ViewportToWorldPoint(new Vector3(1, 0.5f, cam.nearClipPlane)).x;
        }
        else if (d.spawnPos == "Left")
        {
            spawnX = cam.ViewportToWorldPoint(new Vector3(0, 0.5f, cam.nearClipPlane)).x;
        }


        for (int i = 0; i < d.count; i++)
        {
            var pos = new Vector3(spawnX, d.y, 0f);
            enemyManager.SpawnEnemy(d.enemyType, pos, dropGroup, parentTransform);

            if (i < d.count - 1 && d.interval > 0f)
                yield return new WaitForSeconds(d.interval);
        }
        // ここで生成完了フラグを立てる
        if (dropGroup != null)
        {
            dropGroup.MarkSpawnComplete();
        }
    }
    private IEnumerator DelayedLoad()
    {
        yield return null; // 1フレーム待つ
        LoadSpawnPoints();
    }
    public void ResetSpawner(float X)
    {
        foreach (var co in activeRoutines)
        {
            if (co != null)
                StopCoroutine(co);
        }
        currentIndex = 0;
        foreach (SpawnPointData point in spawnPoints)
        {
            if (point.x >= X)
            {
                return;
            }
            currentIndex++;
        }   
    }
}
