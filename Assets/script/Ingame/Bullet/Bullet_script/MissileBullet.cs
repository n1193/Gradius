using UnityEngine;
public class MissileBullet : Bullet
{
    private LayerMask groundLayer;
    RaycastHit2D hit;

    private void Start()
    {
        speed = 15f;
        dir = new Vector2(1, -1); // 初期方向を右に設定
        groundLayer = LayerMask.GetMask("Ground");
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.Ground) || collision.CompareTag(Tags.Enemy))
        {
            Destroy(gameObject); // ミサイルを破壊
        }

    }
    void Update()
    {
        UpdateSprite();
        Vector2 checkDirection = Vector2.down * 2f;
        hit = Physics2D.Raycast(transform.position, checkDirection, 0.4f, groundLayer);
        if (hit.collider != null)
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