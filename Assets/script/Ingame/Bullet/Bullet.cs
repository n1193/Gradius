using UnityEngine;
using UnityEditor;

public enum BulletOwner { Player, Enemy }


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public abstract class Bullet : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] protected float speed;   // ← プレハブ既定値 or 生成時オーバーライド

    [Header("Runtime")]
    protected BulletOwner owner;
    protected Vector2 dir;

    [Header("Refs")]
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected BoxCollider2D col;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    public bool isDead;
    protected BulletPool bulletPool;

    protected virtual void Awake()
    {
        isDead = false;
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!col) col = GetComponent<BoxCollider2D>();
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();

        // ★ 動かすなら Dynamic + velocity が一番簡単
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; ;
        col.isTrigger = true;
    }

    public virtual void Initialize(BulletOwner owner, BulletPool bulletPool, string tag = Tags.PlayerBullet, float? overrideSpeed = null)
    {
        this.owner = owner;
        this.bulletPool = bulletPool;
        gameObject.tag = tag;
        refrash();
    }
    public virtual void refrash()
    {
        isDead = false;
        gameObject.SetActive(true);
    }
   protected virtual void FixedUpdate()
    {
        rb.linearVelocity = dir * speed;
    }
    protected virtual void OnBecameInvisible()
    {
        if (bulletPool != null && !isDead)
            bulletPool.Return(this);
        else
            Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (owner == BulletOwner.Player)
        {
            if (other.CompareTag(Tags.Player) || other.CompareTag(Tags.PlayerBullet)) return;
            if (other.CompareTag(Tags.Enemy) || other.CompareTag(Tags.Ground))
            {
                if (bulletPool != null&!isDead)
                    gameObject.SetActive(false);
                else
                    Destroy(gameObject);
            }
        }
        else
        {
            if (other.CompareTag(Tags.Enemy) || other.CompareTag(Tags.EnemyBullet)) return;
            if (other.CompareTag(Tags.Player) || other.CompareTag(Tags.Ground))
            {
                if (bulletPool != null && !isDead)
                    gameObject.SetActive(false);
                else
                    Destroy(gameObject);
            }
        }
    }
    public void Die()
    { 
        isDead = true;
    }
}