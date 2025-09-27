// StageRootMover.cs（StageRoot に付ける）
using UnityEngine;

public class StageRootMover : MonoBehaviour
{
    [SerializeField] ScrollDirector director;
    [SerializeField] int pixelsPerUnit = 16;
    
    void LateUpdate()
    {
        if (!director) return;
        // ステージは左へ動かす＝原点から見て -X
        float x = -director.X;
        if (pixelsPerUnit > 0)
            x = Mathf.Round(x * pixelsPerUnit) / pixelsPerUnit;
        var p = transform.position;
        p.x = x;
        transform.position = p;
    }
}
