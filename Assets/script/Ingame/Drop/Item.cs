using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("見た目")]
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite[] sprites;
    [SerializeField, Min(0f)] float frameTime = 0.3f;

    ICollectibleEffect _effect;
    float _t; int _i; bool _consumed;

    void Awake()
    {
        _effect = GetComponent<ICollectibleEffect>();
        if (_effect == null) Debug.LogWarning("[Item] 効果(ICollectibleEffect)が見つからない", this);
    }

    void Update()
    {
        if (sprites == null || sprites.Length == 0 || !spriteRenderer) return;
        _t += Time.deltaTime;
        if (_t >= frameTime) { _t = 0f; _i = (_i + 1) % sprites.Length; spriteRenderer.sprite = sprites[_i]; }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_consumed) return;
        if (!other.CompareTag(Tags.Player)) return; // LayerでもOK
        _consumed = true;

        _effect?.Apply(other.gameObject);
        Destroy(gameObject); // プールなら Despawn に置換
    }
}
