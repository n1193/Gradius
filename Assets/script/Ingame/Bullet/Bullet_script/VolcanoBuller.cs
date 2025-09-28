using UnityEngine;
using Zenject;

public class VolcanoBullet : Bullet
{
    public Vector2 upSpeedRange = new Vector2(7f, 12f);   // 上向き初速 [min,max]
    public Vector2 horizSpeedRange = new Vector2(-10f, 10f); // 左右初速 [min,max]（負で左）
    //public Vector2 gravityRange = new Vector2(9.81f, 9.81f * 1.5f); // 重力加速度 [min,max] 
    public float gravity = 9.81f;  // m/s^2（Physics2D.gravity に合わせる）
    ScoreManager scoreManager;
    
    public float fallMultiplier = 2.5f; // 落下時だけ重力を何倍にするか
    float _baseGravityScale;

    [Inject]
    void Construct(ScoreManager scoreManager)
    {
        this.scoreManager = scoreManager;
    }
    public override void Initialize(BulletOwner owner, BulletPool bulletPool, string tag = Tags.PlayerBullet, float? overrideSpeed = null)
    {
        this.bulletPool = bulletPool;
        this.owner = owner;
        this.dir = Vector2.up;
        this.speed = overrideSpeed ?? speed;
        gameObject.tag = tag;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.simulated = true;
        rb.linearDamping = 0f;   // 旧 drag
        rb.angularDamping = 0f;   // 旧 angularDrag
        rb.gravityScale   = _baseGravityScale;
        _baseGravityScale = gravity / Mathf.Abs(Physics2D.gravity.y);
        float vy = UnityEngine.Random.Range(upSpeedRange.x, upSpeedRange.y);
        float vx = UnityEngine.Random.Range(horizSpeedRange.x, horizSpeedRange.y);
        dir = new Vector2(vx, vy);
    }
    protected override void FixedUpdate()
    {
        if (dir != Vector2.zero)
        {
            rb.linearVelocity = dir;
            dir = Vector2.zero;
        }
        // ★ ここで落下中だけ重力強化
        if (rb.linearVelocity.y <= 0f)
            rb.gravityScale = _baseGravityScale * fallMultiplier; // 速く落ちる
        else
            rb.gravityScale = _baseGravityScale;                  // 上昇は今のまま
    }



    protected override void OnBecameInvisible()
    {
        base.OnBecameInvisible();
    }
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Tags.PlayerBullet))
        {
            scoreManager.AddScore(100);
            other.gameObject.SetActive(false);
            Destroy(gameObject);
        }
        if (other.CompareTag(Tags.Ground))
        {
            Destroy(gameObject);
        }
    }
}
