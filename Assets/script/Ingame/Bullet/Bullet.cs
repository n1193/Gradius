using UnityEngine;
using UnityEditor;

public enum BulletOwner { Player, Enemy }


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public abstract class Bullet : MonoBehaviour
{
    protected ShotManager shotManager;

    [Header("Config")]
    [SerializeField] protected float speed;   // ← プレハブ既定値 or 生成時オーバーライド

    [Header("Runtime")]
    protected BulletOwner owner;
    protected Vector2 dir;

    [Header("Refs")]
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected BoxCollider2D col;
    [SerializeField] protected SpriteRenderer spriteRenderer;

    protected virtual void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!col) col = GetComponent<BoxCollider2D>();
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();

        // ★ 動かすなら Dynamic + velocity が一番簡単
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;;
        col.isTrigger = true;
    }

    public virtual void Initialize(BulletOwner owner, ShotManager mgr, Vector3 direction,
                                   string tag = Tags.PlayerBullet, float? overrideSpeed = null)
    {
        this.owner = owner;
        shotManager = mgr;
        dir = ((Vector2)direction).normalized;
        gameObject.tag = tag;

        // 生成時に与えられた速度があればそれを採用、なければプレハブの既定値を使う
        speed = overrideSpeed ?? speed;

        // ★ 正しいAPI：velocity
        rb.linearVelocity = dir * speed;
    }
    protected virtual void OnBecameInvisible()
    {
        shotManager?.RemoveBullet(this);
        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        // ……（ここはそのままでOK。Layerで極力切るのがベスト）
        if (owner == BulletOwner.Player)
        {
            if (other.CompareTag(Tags.Player) || other.CompareTag(Tags.PlayerBullet)) return;
            if (other.CompareTag(Tags.Enemy) || other.CompareTag(Tags.Ground))
            {
                shotManager?.RemoveBullet(this);
                Destroy(gameObject);
            }
        }
        else
        {
            if (other.CompareTag(Tags.Enemy) || other.CompareTag(Tags.EnemyBullet)) return;
            if (other.CompareTag(Tags.Player) || other.CompareTag(Tags.Ground))
            {
                shotManager?.RemoveBullet(this);
                Destroy(gameObject);
            }
        }
    }
}