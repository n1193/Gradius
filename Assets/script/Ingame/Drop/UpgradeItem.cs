using UnityEngine;
using Zenject;

class UpgradeItem : MonoBehaviour
{
    private Upgrade upgrade;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite[] sprites;
    float maxspriteWaitTime = 0.3f;
    float spriteWaitTime = 0f;
    int spriteIndex = 0; // スプライトのインデックス

    [Inject]
    public void Construct(Upgrade upgrade)
    {
        this.upgrade = upgrade;
    }
    void Update()
    {
        UpdateSprite();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Tags.Player))
        {
            upgrade.ChangeSelect();
            Destroy(gameObject); // アイテムを消す
        }
    }
    void UpdateSprite()
    {
        if (sprites.Length == 0) return;
        spriteWaitTime += Time.deltaTime;
        if (spriteWaitTime >= maxspriteWaitTime)
        {
            spriteWaitTime = 0f;
            spriteIndex++;
            if (spriteIndex >= sprites.Length) spriteIndex = 0;
            spriteRenderer.sprite = sprites[spriteIndex];
        }
    }
}