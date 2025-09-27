using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
[ExecuteAlways]   // エディタでだけ動かす
public class VisibilitySanityCheck : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] targets;   // 明示的に指定
    [SerializeField] Material spriteMat;         // 作った Mat を入れる（Sprites/Default or URP 2D）

    [ContextMenu("Fix & Log")]
    void FixAndLog() {
        foreach (var sr in targets){
            if (!sr) continue;
            if (spriteMat) sr.sharedMaterial = spriteMat; // ← material ではなく sharedMaterial
            sr.maskInteraction = SpriteMaskInteraction.None; // 必要なら
            UnityEngine.Debug.Log($"{sr.name} mat={sr.sharedMaterial?.shader?.name} order={sr.sortingOrder}");
        }
    }
}
#endif
