using UnityEngine;
public class MissileBullet : Bullet
{
    private LayerMask groundLayer;
    RaycastHit2D hit;

    protected override void Awake()
    {
        speed = 15f;
        dir = new Vector2(1, -1); // 初期方向を右に設定
        groundLayer = LayerMask.GetMask("Ground");
        base.Awake();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.Ground) || collision.CompareTag(Tags.Enemy)|| collision.CompareTag(Tags.VolcanoBullet))
        {
            if (bulletPool != null & !isDead)
                gameObject.SetActive(false);
            else
                Destroy(gameObject);
        }

    }
    void Update()
    {
        UpdateSprite();
        Vector2 checkDirection = Vector2.down * 2f;
        hit = Physics2D.Raycast(transform.position, checkDirection, 1f, groundLayer);
        if (hit.collider !=null && transform.position.y <= -4.3f)
        {
            dir = Vector2.right; // 右方向に変更
        }
        else
        {
            dir = new Vector2(1, -1); // 右方向に変更
        }
        rb.linearVelocity = dir * speed;
    }
    void UpdateSprite()
    {
        if(dir == Vector2.right)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 47f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
}