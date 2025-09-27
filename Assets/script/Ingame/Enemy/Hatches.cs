using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System.Collections;
public enum WallAttachment
{
    Top,
    Bottom
}

class Hatches : Enemy
{
    SpriteRenderer spriteRenderer;
    public List<Sprite> sprites = new List<Sprite>();
    public Rush instancePrefab;
    int instanceCount;
    float instanceInterval; // インスタンス生成間隔
    float instanceTimer; // 再生成までのタイマー
    public WallAttachment wallAttachment = WallAttachment.Top;
    public EnemyManager enemyManager;
    DiContainer _container;

    int hp;

    Vector2 direction; // 移動方向
    float speed = 2f; // 移動速度
    [Inject]
    void Construct(EnemyManager enemyManager, DiContainer container)
    {
        this.enemyManager = enemyManager;
        _container = container;
    }
    void Start()
    {
        hp = 3;

        spriteRenderer = GetComponent<SpriteRenderer>();
        direction = new Vector2(1, 0);
        instanceCount = 2;
        instanceInterval = 2f;
        wallAttachment = (this.transform.position.y > 0) ? WallAttachment.Top : WallAttachment.Bottom;
        if (wallAttachment == WallAttachment.Top)
        {
            spriteRenderer.flipY = true;
        }
        else
        {
            spriteRenderer.flipY = false;
        }
    }

    void Update()
    {
        if (instanceCount <= 0)
        {
            return;
        }
        instanceTimer -= Time.deltaTime;
        if (instanceTimer <= 0)
        {
            instanceCount--;
            instanceTimer = instanceInterval;
            StartCoroutine(InstanceEnemy());
        }
    }

    IEnumerator InstanceEnemy()
    {
        for (int i = 0; i < 4; i++)
        {
            _container.InstantiatePrefabForComponent<Enemy>(instancePrefab, transform.position, Quaternion.identity, transform.parent);
            yield return new WaitForSeconds(0.25f); // ウェーブ間の待機時間
        }
    }
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        hp--;
        if (hp <= 0)
        {
            scoreManager?.AddScore(score);
            Destroy(gameObject);
        }
    }

}
