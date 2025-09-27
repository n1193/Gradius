using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Zenject;

class Ducker : Enemy
{
    enum EnemyState
    {
        Move,
        Shot,
        Exit
    }
    private float groundCheckDistance = 1f; // 地面チェックの距離
    private LayerMask groundLayer; // 地面のレイヤーマスク
    SpriteRenderer spriteRenderer;
    public List<Sprite> sprites = new List<Sprite>();
    int spriteIndex = 0; // スプライトのインデックス
    int spriteChangeInterval = 30; // スプライトを変更する間隔
    float bottomY = 0f;
    EnemyState state = EnemyState.Move;
    Action Shot; // 通常弾やレーザー弾の発射アクションのイベント
    const int shotIntervalTime = 300; // 弾を発射する間隔
    int shotInterval = 60; // 弾を発射する間隔
    int shotCount = 0; // 発射した弾の数
    const int maxShotCount = 3; // 最大発射数

    Vector2 direction; // 移動方向
    float speed = 3f; // 移動速度
    void Start()
    {
        bottomY = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.2f, 0f)).y;
        spriteRenderer = GetComponent<SpriteRenderer>();
        direction = new Vector2(3, 0);
        groundLayer = LayerMask.GetMask("Ground");
        Shot = () =>
        {
            float bottomPosition = spriteRenderer.bounds.min.y;
            float heightSize = spriteRenderer.bounds.size.y;
            Vector3 currentPos = (playerController.transform.position - transform.position).normalized;
            weaponManager.Fire(
            WeaponType.Enemy,
            transform.position,
            currentPos,
            Tags.EnemyBullet,
            BulletOwner.Enemy);
        };
        state = EnemyState.Move;
         if (transform.position.y > 0)
        {
            spriteRenderer.flipY = true; // スプライトを反転
        }
    }

    void Update()
    {
        if (state == EnemyState.Shot)
        {
            BulletShot();
            return;
        }
        UpdateSprite();
        Move();
    }
    void Move()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
        Vector2 checkDirection = transform.position.y > 0 ? Vector2.up : Vector2.down;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, checkDirection, groundCheckDistance, groundLayer);
        if (hit.collider != null)
        {
            float offsetY = transform.position.y > 0 ? -spriteRenderer.bounds.extents.y : spriteRenderer.bounds.extents.y;
            Vector3 newPosition = new Vector3(transform.position.x, hit.point.y + offsetY, transform.position.z);
            transform.position = newPosition;
        }

        if (state == EnemyState.Move && transform.position.x > playerController.transform.position.x + 3f)
        {
            ChangeStaete(EnemyState.Shot); // 攻撃状態に移行
        }
        spriteIndex++;       
    }
    void BulletShot()
    {
        shotInterval--;
        if (shotInterval <= 0)
        {
            shotInterval = shotIntervalTime; // リセット
            Shot?.Invoke(); // 弾を発射
            shotCount++;
            if (shotCount >= maxShotCount)
            {
                ChangeStaete(EnemyState.Exit);
            }
        }
        else if (transform.position.x < playerController.transform.position.x)
        {
            ChangeStaete(EnemyState.Move); // 攻撃状態に移行
        }
    }
    void ChangeStaete(EnemyState state)
    {
        this.state = state;

        if (state == EnemyState.Move)
        {
            direction = new Vector2(3, 0); // 右に移動
            spriteRenderer.flipX = false; // スプライトを反転
        }
        else if (state == EnemyState.Shot)
        {
            state = EnemyState.Shot; // 攻撃状態に移行
            spriteRenderer.sprite = sprites[sprites.Count - 1]; // スプライトを更新
            spriteRenderer.flipX = true; // スプライトを反転
        }
        else // (state == EnemyState.Exit)
        {
            direction = new Vector2(-1, 0); // 右に移動
            spriteRenderer.flipX = true; // スプライトを反転
        }
    }

    void UpdateSprite()
    {
        if (spriteIndex >= (sprites.Count-1)*spriteChangeInterval)
        {
            spriteIndex = 0; // スプライトのインデックスをリセット
        }
        spriteRenderer.sprite = sprites[spriteIndex/spriteChangeInterval]; // スプライトを更新
    }


}
