using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


class Rush : Enemy
{
    enum MoveType
    { 
        SPAWN,
        POSITIONING
    }
    SpriteRenderer spriteRenderer;
    public List<Sprite> sprites = new List<Sprite>();
    int spriteIndex = 0; // スプライトのインデックス
    MoveType moveType;
    int spriteChangeInterval = 30; // スプライトを変更する間隔
    Vector2 direction; // 移動方向
    float speed = 5f; // 移動速度


    [Inject]
    public void Construct(PlayerController playerController, WeaponManager weaponManager)
    {
        //this.playerController = playerController;
    }
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        moveType = MoveType.SPAWN;
        direction = transform.position.y > playerController.transform.position.y ? Vector2.down : Vector2.up;
    }

    void Update()
    {
        Move();
        UpdateSprite();
    }
    void Move()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
        if (moveType == MoveType.SPAWN && Math.Abs(transform.position.y-playerController.transform.position.y) < 0.1f)
        {
            transform.position = new Vector3(transform.position.x, playerController.transform.position.y, transform.position.z);
            direction = Vector2.left; // 方向を反転
            moveType = MoveType.POSITIONING; // 移動タイプを変更
        }
    }
    void UpdateSprite()
    {
       spriteIndex++;
        if (spriteIndex >= (sprites.Count-1)*spriteChangeInterval)
        {
            spriteIndex = 0; // スプライトのインデックスをリセット
        }
        spriteRenderer.sprite = sprites[spriteIndex/spriteChangeInterval]; // スプライトを更新
    }
}
