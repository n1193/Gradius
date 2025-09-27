using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using Zenject;

public class VolcanoBullet : Bullet
{
    public Vector2 upSpeedRange = new Vector2(7f, 12f);   // 上向き初速 [min,max]
    public Vector2 horizSpeedRange = new Vector2(-10f, 10f); // 左右初速 [min,max]（負で左）
    //public Vector2 gravityRange = new Vector2(9.81f, 9.81f * 1.5f); // 重力加速度 [min,max] 
    public float gravity = 9.81f;  // m/s^2（Physics2D.gravity に合わせる）
    ScoreManager scoreManager;

    [Inject]
    void Construct(ScoreManager scoreManager)
    {
        this.scoreManager = scoreManager;
    }

    public override void Initialize(BulletOwner owner, ShotManager mgr, Vector3 direction,
                                    string tag = Tags.EnemyBullet, float? overrideSpeed = null)
    {
        this.owner = owner;
        this.shotManager = mgr;
        this.dir = Vector2.up;                // 今回は使わないが一応
        this.speed = overrideSpeed ?? speed;  // （参照されるなら保持）
        gameObject.tag = tag;
        //gravity = Random.Range(gravityRange.x, gravityRange.y);
        rb.gravityScale = gravity / Mathf.Abs(Physics2D.gravity.y);

        float vy = Random.Range(upSpeedRange.x, upSpeedRange.y);
        float vx = Random.Range(horizSpeedRange.x, horizSpeedRange.y);
        rb.linearVelocity = new Vector2(vx, vy);
    }

    protected override void OnBecameInvisible()
    {
        base.OnBecameInvisible();
    }
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Tags.PlayerBullet))
        {
            shotManager?.RemoveBullet(this);
            scoreManager.AddScore(100);
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
        if (other.CompareTag(Tags.Ground) || other.CompareTag(Tags.Player))
        {
            shotManager?.RemoveBullet(this);
            Destroy(gameObject);
        }
    }
}
