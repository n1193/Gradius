using System;
using System.Collections.Generic;
using ModestTree;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

class Rugal : Enemy
{
    SpriteRenderer spriteRenderer;
    public List<Sprite> sprites = new List<Sprite>();
    float speed = 2f; // 移動速度
    Vector2 direction; // 移動方向


    int moveTypeIndex = 0; // 現在のMoveTypeのインデックス
    int spriteIndex = 0; // スプライトのインデックス
    int spriteChangeInterval = 10; // スプライトを変更する間隔
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        direction = Vector2.left*2;
    }

    void Update()
    {
        Move();
    }
    void Move()
    {
        direction.y = Math.Clamp(playerController.transform.position.y-transform.position.y, -0.5f, 0.5f);

        float dirX=-1;

        if (Math.Abs(direction.y) <= 0.1f)
        {
            dirX = -2f;
        }
        else
        {
            dirX = -1f;
        }

        direction.x = dirX;

        transform.position += (Vector3)direction* speed * Time.deltaTime;
        UpdateSprite(); //スプライトの更新
    }
    void UpdateSprite()
    {
        if (Math.Abs(direction.y) <= 0.1f)
        {
            spriteRenderer.sprite = sprites[0];
        }
        else if (direction.y > 0)
        {
            spriteRenderer.sprite = sprites[2];
        }
        else
        {
            spriteRenderer.sprite = sprites[1];
        }
    }   
}
