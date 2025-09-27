using UnityEngine;

using System.Collections.Generic;
using UnityEngine.Rendering;
using Zenject;
using UnityEngine.UIElements;


public class Shield : MonoBehaviour
{
    [Header("Shield Settings")]
    public int maxShieldHits = 3;
    [SerializeField] public List<Sprite> sprites = new List<Sprite>();
    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] GameObject shieldImage;
    private Upgrade upgrade;
    private int currentHits;
    [SerializeField]private BoxCollider2D boxCollider;

    int spriteIndex = 0;
    int spriteChangeInterval = 15; // スプライトを変更する間隔


    [Inject]
    public void Construct(Upgrade upgrade)
    {
        this.upgrade = upgrade;
    }
    void Start()
    {
        currentHits = maxShieldHits;
        boxCollider.enabled = false;
        spriteRenderer.enabled=false;
        
    }
    void Update()
    {
        UpdateSprite();
    }
    public void ActivateShield()
    {
        currentHits = maxShieldHits;
        shieldImage.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        spriteRenderer.enabled=true;
        boxCollider.enabled = true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 敵弾や敵との衝突判定
        if (collision.CompareTag("EnemyBullet") || collision.CompareTag("Enemy"))
        {
            ShieldHit();
            Destroy(collision.gameObject);
        }
    }

    void ShieldHit()
    {
        currentHits--;
        Vector3 scale = shieldImage.gameObject.transform.localScale;
        shieldImage.gameObject.transform.localScale = new Vector3(scale.x * 0.8f, scale.y * 0.8f, scale.z * 0.8f);
        if (currentHits <= 0)
        {
            BreakShield();
        }
    }

    void BreakShield()
    {
        if (shieldImage == null)
            return;
        boxCollider.enabled = false;
        upgrade.SetLevel(UpgradeData.UpgradeType.Shield, 0);
        spriteRenderer.enabled=false;
     }
    void UpdateSprite()
    {
        if (!spriteRenderer.enabled) return;
        spriteIndex++;
        if (spriteIndex >= sprites.Count * spriteChangeInterval)
        {
            spriteIndex = 0; // スプライトのインデックスをリセット
        }
        spriteRenderer.sprite = sprites[spriteIndex / spriteChangeInterval]; // スプライトを更新
    }
}
