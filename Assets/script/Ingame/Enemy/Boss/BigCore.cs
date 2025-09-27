using System;
using System.Collections.Generic;
using System.Data;
using ModestTree;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;
using UnityEngine.SceneManagement;
class BigCore : Enemy
{
    enum MoveTypes
    {
        MoveLeft,       // 左に直進
        MoveUp,         // 上に直進
        MoveDown       // 下に直進
    }
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject activeSprite;
    [SerializeField] private GameObject bodySprite;
    [SerializeField] private GameObject weekMask; // 弱点マスク

    private List<Vector3> visiblePosition;
    float clampYMax = 1920f;
    float clampYMin = 0f;
    MoveTypes moveType;
    int currentHealth;
    bool isActive = false; // ボスがアクティブかどうか
    float activedelay = 10.0f; // ボスがアクティブになるまでの遅延時間

    const int ShotDelayMax = 2; // 弾を撃つ間隔(秒)  
    float ShotDelay;


    Action Shot; // 通常弾やレーザー弾の発射アクションのイベント
    Vector2 direction; // 移動方向
    float speed = 2f; // 移動速度 
    [SerializeField] string signalId = "Boss";

    int score = 10000; // 倒したときのスコア
    [SerializeField] ScrollDirector scroll;

    [Inject]
    void Construct(ScrollDirector scroll)
    {
        this.scroll = scroll;
    }

    void Awake()
    {

        isActive = false;
        activeSprite.SetActive(isActive);


        ShotDelay = ShotDelayMax;
        Debug.Log($"[Boss] Awake  activeSelf={gameObject.activeSelf}");
        currentHealth = 6;
        visiblePosition = new List<Vector3> //シールドの表示座標
        {
            new Vector3(0.92f, 0f,0f),
            new Vector3(0.66f, 0f,0f),
            new Vector3(0.48f, 0f,0f),
            new Vector3(0.32f, 0f,0f),
            new Vector3(0.16f, 0f,0f),
            new Vector3(0f, 0f,0f)
        };

        Shot += () =>
        {
            float heightSize = spriteRenderer.bounds.size.y;
            weaponManager.Fire(
                WeaponType.Boss,
                transform.position + new Vector3(0.8f, heightSize * 0.4f, 0),
                Vector3.left,
                Tags.EnemyBullet,
                BulletOwner.Enemy);
            weaponManager.Fire(
                WeaponType.Boss,
                transform.position + new Vector3(0, heightSize * 0.15f, 0),
                Vector3.left,
                Tags.EnemyBullet,
                BulletOwner.Enemy);
            weaponManager.Fire(
                WeaponType.Boss,
                transform.position + new Vector3(0, heightSize * -0.15f, 0),
                Vector3.left,
                Tags.EnemyBullet,
                BulletOwner.Enemy);
            weaponManager.Fire(
                WeaponType.Boss,
                transform.position + new Vector3(0.8f, heightSize * -0.4f, 0),
                Vector3.left,
                Tags.EnemyBullet,
                BulletOwner.Enemy);
        };
        clampYMax = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.8f, 0f)).y;
        clampYMin = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.2f, 0f)).y;
        direction = Vector2.left;
        moveType = MoveTypes.MoveLeft; // 初期は左に進む
    }
    void OnEnable() { Debug.Log($"[Boss] OnEnable"); }

    void Update()
    {
        if (!isActive)
        {
            activedelay -= Time.deltaTime;
            if (activedelay <= 0)
            {
                isActive = true;
                activeSprite.SetActive(isActive);
            }
        }
        Move();
        UpdateSprite();
        BulletShot();
    }
    void Move()
    {
        if (moveType == MoveTypes.MoveLeft)
        {
            if (transform.position.x < Camera.main.ViewportToWorldPoint(new Vector3(0.6f, 0.2f, 0f)).x)
            {
                moveType = MoveTypes.MoveUp;
                direction = Vector2.up;
            }
        }
        transform.position += (Vector3)direction * speed * Time.deltaTime;
        float newY = Mathf.Clamp(transform.position.y, clampYMin, clampYMax);

        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        if (transform.position.y == clampYMax || transform.position.y == clampYMin)
        {
            changeMoveType();
        }
    }
    void changeMoveType()

    {
        switch (moveType)
        {
            case MoveTypes.MoveLeft:
            case MoveTypes.MoveDown:
                moveType = MoveTypes.MoveUp; // 左に進みすぎたら上へ進む
                direction = Vector2.up;
                break;
            case MoveTypes.MoveUp:
                moveType = MoveTypes.MoveDown; // 上に進みすぎたら下へ進む
                direction = Vector2.down;
                break;
        }
    }
    void BulletShot()
    {
        ShotDelay -= Time.deltaTime;
        if (ShotDelay > 0) return;
        Shot?.Invoke(); // 弾を発射
        ShotDelay = ShotDelayMax;
    }
    void UpdateSprite()
    {
        if (currentHealth <= 0) return;
        weekMask.gameObject.transform.localPosition = visiblePosition[currentHealth - 1];
        activeSprite.SetActive(isActive);
    }
    public void Damage()
    {
        if (!isActive) return; // ボスがアクティブになるまでダメージを受けない
        currentHealth--;
        if (currentHealth <= 0)
        {
            scroll.NotifySignal(signalId);
            scoreManager.AddScore(score);
            SceneManager.LoadScene(SceneType.TitleScene.ToString());
            Destroy(this.gameObject);
            return;
        }
        UpdateSprite();
    }
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        //デフォルト処理の削除
    }
}
