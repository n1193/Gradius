using System;
using System.Collections.Generic;
using UnityEngine;

class Dee_01 : Enemy
{
    SpriteRenderer spriteRenderer;
    public List<Sprite> sprites = new List<Sprite>();
    int spriteIndex = 0; // スプライトのインデックス
    Action Shot; // 通常弾やレーザー弾の発射アクションのイベント
    const float shotIntervalTime = 2.5f; // 弾を発射する間隔
    float shotInterval = 60f; // 弾を発射する間隔
    Vector2 direction; // 移動方向
    float speed = 2f; // 移動速度
    private BulletPool bulletPool; // トリガ（メイン＋各オプションが購読）
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        direction = new Vector2(1, 0);

        bulletPool = weaponManager.CreateBulletPool(transform);
        bulletPool.Initialize(BulletOwner.Enemy, WeaponType.Enemy, 2,1f,SEType.None, Tags.EnemyBullet.ToString());
        if (this.transform.position.y > 0)
        {
            spriteRenderer.flipY = true;
        }
    }

    void Update()
    {
        UpdateSprite();
        Attack();
    }
    void Move()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;       
    }
    void Attack()
    {
        shotInterval -= Time.deltaTime;
        if (shotInterval <= 0)
        {
            shotInterval = shotIntervalTime; // リセット
            bulletPool.Fire(transform.position,BulletOwner.Enemy); // 弾を発射
        }
    }
    void UpdateSprite()
    {
        Vector3 position = (playerController.transform.position - transform.position).normalized;
        if (position.x < 0)
        {
            spriteRenderer.flipX = true; // スプライトを反転  
        }
        else
        {
            spriteRenderer.flipX = false; // スプライトを反転  
        }

        position = new Vector3(Mathf.Abs(position.x), Mathf.Abs(position.y), 0);
        float angle = Mathf.Abs(position.x - position.y);
        if (angle < 0.25f)
        {
            spriteIndex = 0; // 左右のスプライト
        }
        else
        {
            if (position.x < position.y)
            {
                spriteIndex = 1; // 上のスプライト
            }
            else
            {
                spriteIndex = 2; // 下のスプライト          
            }
        }
                  
        spriteRenderer.sprite = sprites[spriteIndex]; // スプライトを更新
    }
}
