using System.Collections.Generic;
using UnityEngine;


class Jumper : Enemy
{
    enum State
    {
        Left,
        right
    }
    SpriteRenderer spriteRenderer;
    public List<Sprite> sprites = new List<Sprite>();
    int spriteIndex = 0; // スプライトのインデックス
    int spriteChangeInterval = 30; // スプライトを変更する間隔
    float bottomY = 0f;
    Vector2 direction; // 移動方向
    float speed = 2f; // 移動速度

    float leftcount = 0f; // 左移動の回数カウント
    float LeftMax = 3f; // 左移動の最大回数
    float upPower = 2f; // ジャンプ力   

    void Start()
    {
        bottomY = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.2f, 0f)).y;
        spriteRenderer = GetComponent<SpriteRenderer>();
        direction = new Vector2(-3, upPower); // 初期の移動方向を下に設定
        leftcount = LeftMax;
    }

    void Update()
    {
        Move();
        UpdateSprite();
    }
    void Move()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
        direction.y -= speed * Time.deltaTime * 3; // Y方向の移動を減速
        if (transform.position.y < bottomY)
        {
            ChnageMove(); // 画面下部に到達したら移動を変更
        }
    }

    void ChnageMove()
    {
        leftcount--;

        //transform.position = new Vector3(transform.position.x, bottomY + 1f, transform.position.z);

        if (leftcount <= 0)
        {
            direction = Vector2.right * 8;
            leftcount = LeftMax;
        }
        else
        {
            direction = Vector2.left * 3;
        }
        direction.y = upPower; // Y方向の移動を反転

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
