using System;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;


class Fan : Enemy
{
    enum MoveType
    {
        MoveLeft,       // 左に直進
        ReturnToRight,  // プレイヤーのY軸に合わせて右へ戻る
        MoveRight       // 右に直進}
    }
    SpriteRenderer spriteRenderer;
    public List<Sprite> sprites = new List<Sprite>();
    int spriteIndex = 0; // スプライトのインデックス
    MoveType moveType;
    int spriteChangeInterval = 20; // スプライトを変更する間隔
    Vector2 direction; // 移動方向
    float speed = 6f; // 移動速度

    float CenterX;

    void Start()
    {
        moveType = MoveType.MoveLeft; // 0または1のランダムな値を設定
        direction = Vector2.left; // 左方向に移動   
        spriteRenderer = GetComponent<SpriteRenderer>();
        CenterX = Camera.main.ViewportToWorldPoint(new Vector3(0.3f, 0f, 0f)).x;
    }
    void Update()
    {
        UpdateSprite();
        Move();
    }
    void Move()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
        if (moveType == MoveType.MoveLeft)
        {
            if (Mathf.Abs(transform.position.x - CenterX) < 0.5f)
            {
                moveType = MoveType.ReturnToRight;
                direction = Vector2.right;
                direction.y = transform.position.y > 0 ? -1f : 1f; // 整数か負数で代入
                spriteRenderer.flipX = true; // スプライトを反転
            }
        }
        else if (moveType == MoveType.ReturnToRight)
        {
            float diff_y = transform.position.y - playerController.transform.position.y;
            if ((direction.y >= 0 ? diff_y >= 0 : diff_y < 0))
            {
                direction = Vector2.right * 2.5f;
                direction.y = 0;
                moveType = MoveType.MoveRight;
            }
        }
    }
    void UpdateSprite()
    {
        spriteIndex++;
        if (spriteIndex >= sprites.Count * spriteChangeInterval)
        {
            spriteIndex = 0; // スプライトのインデックスをリセット
        }
        spriteRenderer.sprite = sprites[spriteIndex / spriteChangeInterval]; // スプライトを更新
    }
}
