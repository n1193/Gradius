using System.Collections.Generic;
using UnityEngine;


class Garun : Enemy
{
    SpriteRenderer spriteRenderer;
    public List<Sprite> sprites = new List<Sprite>();

    int spriteIndex = 0; // スプライトのインデックス
    int spriteChangeInterval = 30; // スプライトを変更する間隔
    int angleDeg = 0;
    float initialY = 0;
    private const int maxAngleDeg = 360;
    float speed = 5f; // 移動速度
    Vector2 direction; // 移動方向
    void Start()
    {

        spriteRenderer = GetComponent<SpriteRenderer>();
        direction = Vector2.left;

        initialY = transform.position.y;
        direction.x = -1; // 左方向に移動
    }

    void Update()
    {
        Move();
        UpdateSprite();
    }
    void Move()
    {
        //360度
        angleDeg++;
        if (angleDeg >= maxAngleDeg)
        {
            angleDeg = 0;
        }
        float angleRad = angleDeg * Mathf.Deg2Rad;
        float positionX = transform.position.x + direction.x;
        transform.position += new Vector3(direction.x, Mathf.Sin(angleRad)/2) * speed * Time.deltaTime;

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
