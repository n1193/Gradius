using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Zenject; // Zenjectの依存性注入を使用

[RequireComponent(typeof(PlayerTrail))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] Sprite[] sprites;             // 0:水平, 1:下, 2:上

    [Header("Move")]
    [SerializeField] float maxSpeed = 10f;
    [SerializeField] float baseSpeed = 5f;

    [Header("Refs")]
    [SerializeField] Rigidbody2D rb2D;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] PlayerTrail playerTrail;
    [SerializeField] PlayerOption optionPrefab;
    [SerializeField] Shield shield;
    [SerializeField] GameObject explosionEffectPrefab;

    [Header("Weapons")]
    [SerializeField] WeaponManager weaponManager;

    Vector2 _move;
    List<PlayerOption> _options = new();

    [Header("Camera")]
    [SerializeField] Transform cam;
    [SerializeField] Vector3[] margin; // 画面端からのマージン(ワールド座標)

    public GameObject[] BulletArray = new GameObject[2];
    private Upgrade _upgrade;
    private SoundManager soundManager;
    [SerializeField] private GameObject explosionPrefab;

    private List<BulletPool> mainShotBulletPool; // トリガ（メイン＋各オプションが購読）
    private BulletPool subShotBulletPool; // トリガ（メイン＋各オプションが購読）
    DiContainer _container;
    PlayerLife playerLife;


    [Inject]
    public void Construct(WeaponManager weaponManager, Upgrade upgrade, SoundManager soundManager, DiContainer container, PlayerLife playerLife)
    {
        _container = container;
        this._upgrade = upgrade;
        this.weaponManager = weaponManager;
        this.soundManager = soundManager;
        this.playerLife = playerLife;
    }

    void Start()
    {
        Initialize();
    }
    public void Initialize()
    {
        transform.position = new Vector3(-6.47f, 0f, 0f);
        mainShotBulletPool = new List<BulletPool>();
        mainShotBulletPool.Clear();
        if (!rb2D) rb2D = GetComponent<Rigidbody2D>();
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        if (!playerTrail) playerTrail = GetComponent<PlayerTrail>();
        rb2D.gravityScale = 0f;
        margin = new Vector3[]{ Camera.main.ViewportToWorldPoint(new Vector3(0.1f, 0.1f, 0f)),
                                Camera.main.ViewportToWorldPoint(new Vector3(0.9f, 0.9f, 0f)) };
        playerTrail.initialize(transform.position);
        BulletPool mainbulletPool = weaponManager.CreateBulletPool(transform);  // ←ここで new しない、上書きしない！
        mainbulletPool.Initialize(BulletOwner.Player, WeaponType.Normal, 2, 0.15f, SEType.PlayerShot, Tags.PlayerBullet.ToString());
        mainShotBulletPool.Add(mainbulletPool);
        spriteRenderer.enabled = true;
        ClearOptions();
        shield?.DeleteShield();
    }


    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        _move = new Vector2(x, y).normalized;
        ChangeSprite(y);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Fire Pressed");
            foreach (var pool in mainShotBulletPool)
            {
                pool.Fire(transform.position, BulletOwner.Player);
            }
            subShotBulletPool?.Fire(transform.position, BulletOwner.Player);

            if (_options != null)
            {
                foreach (var opt in _options)
                {
                    if (opt == null) continue;
                    opt.FireMain();
                    opt.FireSub();
                }
            }
        }
    }
    void FixedUpdate()
    {
        rb2D.linearVelocity = _move * baseSpeed;
        if (_move == Vector2.zero)
        {
            rb2D.linearVelocity = Vector2.zero;
            return;
        }
        playerTrail.BufUpdate();
        if (_options != null)
        {
            foreach (var opt in _options)
            {
                if (opt == null) continue;
                opt.UpdatePosition();
            }
        }
    }

    void LateUpdate()
    {
        if (!cam) return;
        var p = transform.position;
        var left = margin[0].x;
        var right = margin[1].x;
        var bottom = margin[0].y;
        var top = margin[1].y;
        p.x = Mathf.Clamp(p.x, left, right);
        p.y = Mathf.Clamp(p.y, bottom, top);
        transform.position = p;
    }
    void ChangeSprite(float moveY)
    {
        if (sprites == null || sprites.Length < 3) return;
        if (moveY > 0) spriteRenderer.sprite = sprites[2];
        else if (moveY < 0) spriteRenderer.sprite = sprites[1];
        else spriteRenderer.sprite = sprites[0];
    }

    void CreateOptions()
    {
        PlayerOption _option = Instantiate(optionPrefab, transform.parent);
        _option.Initialize(playerTrail, transform, (OptionIndex)(_options.Count - 1));
        _option.UpdatePosition();
        _options.Add(_option);

        foreach (BulletPool obj in mainShotBulletPool)
        {
            BulletPool mainBulletPool = weaponManager.CreateBulletPool(_option.gameObject.transform);
            mainBulletPool.CopySettingsFrom(obj);
            _option.AddMainShot(mainBulletPool);
        }
        if (subShotBulletPool != null)
        {
            BulletPool subBulletPool = weaponManager.CreateBulletPool(_option.gameObject.transform);
            subBulletPool.CopySettingsFrom(subShotBulletPool);
            _option.AddMainShot(subBulletPool);
        }
    }
    void UpdateOptionMainWeapon(BulletPool _bulletPool)
    {
        foreach (var opt in _options)
        {
            if (opt == null) continue;
            BulletPool bulletPool = weaponManager.CreateBulletPool(opt.transform);
            bulletPool.CopySettingsFrom(_bulletPool);
            opt.AddMainShot(bulletPool);
        }
    }
    void UpdateOptionSubWeapon(BulletPool _bulletPool)
    {
        foreach (var opt in _options)
        {
            if (opt == null) continue;
            BulletPool bulletPool = weaponManager.CreateBulletPool(opt.transform);
            bulletPool.CopySettingsFrom(_bulletPool);
            opt.AddMainShot(bulletPool);
        }
    }
    void CleanMainShot()
    {
        foreach (BulletPool obj in mainShotBulletPool)
        {
            obj.Dead();
            Destroy(obj);
        }
        mainShotBulletPool.Clear();

        foreach (var opt in _options)
        {
            if (opt == null) continue;
            opt.CleanMainShot();
        }
    }

    public void ChangeWeapon(UpgradeData.UpgradeType upgrade)
    {
        switch (upgrade)
        {
            case UpgradeData.UpgradeType.Speed:
                baseSpeed = Mathf.Min(baseSpeed + 2f, maxSpeed);
                break;
            case UpgradeData.UpgradeType.Missile:
                subShotBulletPool = weaponManager.CreateBulletPool(transform);  // ←ここで new しない、上書きしない！
                subShotBulletPool.Initialize(BulletOwner.Player, WeaponType.Missile, 1, 1f, SEType.None, Tags.PlayerBullet.ToString());
                UpdateOptionSubWeapon(subShotBulletPool);
                break;

            case UpgradeData.UpgradeType.Double:
                CleanMainShot();
                var bulletPool1 = weaponManager.CreateBulletPool(transform);
                bulletPool1.Initialize(BulletOwner.Player, WeaponType.Normal, 2, 0.2f, SEType.PlayerShot, Tags.PlayerBullet);
                mainShotBulletPool.Add(bulletPool1);
                UpdateOptionMainWeapon(bulletPool1);
                var bulletPool2 = weaponManager.CreateBulletPool(transform);
                bulletPool2.Initialize(BulletOwner.Player, WeaponType.Double, 2, 0.2f, SEType.None, Tags.PlayerBullet);
                mainShotBulletPool.Add(bulletPool2);
                UpdateOptionMainWeapon(bulletPool2);
                _upgrade.SetLevel(UpgradeData.UpgradeType.Laser, 0);

                break;

            case UpgradeData.UpgradeType.Laser:
                CleanMainShot();
                BulletPool bulletPool3 = weaponManager.CreateBulletPool(transform);
                bulletPool3.Initialize(BulletOwner.Player, WeaponType.Laser, 1, 0.25f, SEType.Laser, Tags.PlayerBullet.ToString());
                mainShotBulletPool.Add(bulletPool3);
                _upgrade.SetLevel(UpgradeData.UpgradeType.Double, 0);
                UpdateOptionMainWeapon(bulletPool3);
                break;

            case UpgradeData.UpgradeType.Option:
                // Option数は Upgrade 側で管理。ここは反映ロジックがあれば追加。
                CreateOptions();
                break;

            case UpgradeData.UpgradeType.Shield:
                shield?.ActivateShield();
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.I))
            return;
#endif

        if (col.CompareTag(Tags.EnemyBullet) || col.CompareTag(Tags.Enemy))
        {
            Die();
        }
    }
    void Die()
    {
        Vector3 pos = transform.position;
        soundManager.SEPlay(SEType.PlayerDead);
        soundManager.BGMStop();
        spriteRenderer.enabled = false;

        Instantiate(explosionPrefab, transform.position, transform.rotation, transform.parent);

        playerLife.TakeDamage();

        gameObject.SetActive(false);
    }
    public void ResetAt(Vector3 pos)
    {
        transform.position = pos;
        rb2D.linearVelocity = Vector2.zero;// HPや移動状態の初期化など最小限
    }
    public void ClearOptions()
{
    foreach (var opt in _options)
    {
        if (opt != null)
            Destroy(opt.gameObject);
    }
    _options.Clear();
}
}
