using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject; // Zenjectの依存性注入を使用
using UnityEngine.SceneManagement;

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

    private Action<Vector3> mainShot; // トリガ（メイン＋各オプションが購読）
    private Action<Vector3> subShot; // トリガ（メイン＋各オプションが購読）
    DiContainer _container;
    [Inject]
    public void Construct(WeaponManager weaponManager, Upgrade upgrade, SoundManager soundManager, DiContainer container)
    {
        _container = container;
        this._upgrade = upgrade;
        this.weaponManager = weaponManager;
        this.soundManager = soundManager;
    }

    void Start()
    {
        if (!rb2D) rb2D = GetComponent<Rigidbody2D>();
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        if (!playerTrail) playerTrail = GetComponent<PlayerTrail>();


        rb2D.gravityScale = 0f;
        margin = new Vector3[]{ Camera.main.ViewportToWorldPoint(new Vector3(0.1f, 0.1f, 0f)),
                                Camera.main.ViewportToWorldPoint(new Vector3(0.9f, 0.9f, 0f)) };
        playerTrail.initialize(transform.position);
        // メイン機発射
        mainShot = (pos) =>
        {
            weaponManager.Fire(
            WeaponType.Normal,
            pos,
            Vector3.right,
            Tags.PlayerBullet,
            BulletOwner.Player);
            soundManager.SEPlay(SEType.PlayerShot, 0.5f);
        };
    }


    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        _move = new Vector2(x, y).normalized;

        ChangeSprite(y);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            FireMain();
            FireSub();
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
    public void FireMain() => mainShot?.Invoke(transform.position);
    public void FireSub() => subShot?.Invoke(transform.position);

    void FixedUpdate()
    {
        // 速度適用は FixedUpdate で
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
        UpdateOptionsWeapon();
    }
    void UpdateOptionsWeapon()
    {
        if (_options == null) return;

        foreach (var opt in _options)
        {
            if (opt == null) continue;
            opt.SetMainShot(mainShot);
            opt.SetSubShot(subShot);
        }
    }

    public void ChangeWeapon(UpgradeData.UpgradeType upgrade)
    {
        Debug.Log($"ChangeWeapon: {upgrade}");
        switch (upgrade)
        {
            case UpgradeData.UpgradeType.Speed:
                baseSpeed = Mathf.Min(baseSpeed + 2f, maxSpeed);
                break;
            case UpgradeData.UpgradeType.Missile:
                subShot = (pos) =>
                {
                    weaponManager.Fire(
                    WeaponType.Missile,
                    pos,
                    new Vector3(1, -1f, 0).normalized,
                    Tags.PlayerBullet,
                    BulletOwner.Player);
                };
                break;

            case UpgradeData.UpgradeType.Double:
                mainShot = (pos) =>
                {
                    weaponManager.Fire(
                    WeaponType.Normal,
                    pos,
                    Vector3.right,
                    Tags.PlayerBullet,
                    BulletOwner.Player);
                    weaponManager.Fire(
                    WeaponType.Normal,
                    pos,
                    new Vector3(1, 1f, 0),
                    Tags.PlayerBullet,
                    BulletOwner.Player);
                    soundManager.SEPlay(SEType.PlayerShot, 0.5f);
                };
                _upgrade.SetLevel(UpgradeData.UpgradeType.Laser, 0);
                break;

            case UpgradeData.UpgradeType.Laser:
                mainShot = (pos) =>
                {
                    weaponManager.Fire(
                    WeaponType.Laser,
                    pos,
                    Vector3.right,
                    Tags.PlayerBullet,
                    BulletOwner.Player);
                    soundManager.SEPlay(SEType.Laser, 0.5f);
                };
                _upgrade.SetLevel(UpgradeData.UpgradeType.Double, 0);
                break;

            case UpgradeData.UpgradeType.Option:
                // Option数は Upgrade 側で管理。ここは反映ロジックがあれば追加。
                CreateOptions();
                break;

            case UpgradeData.UpgradeType.Shield:
                shield?.ActivateShield();
                break;
        }
        UpdateOptionsWeapon();
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
      
        SceneManager.LoadScene(SceneType.TitleScene.ToString());
    }
}
