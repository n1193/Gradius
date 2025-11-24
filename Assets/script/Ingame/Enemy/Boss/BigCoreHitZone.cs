using UnityEngine;


public class BigCoreHitZone : MonoBehaviour
{
    public enum BossZoneType { WeakPoint,Other }
    [SerializeField] private BossZoneType zoneType = BossZoneType.Other;
    [SerializeField] private BigCore boss; // 親参照。未設定ならAwakeで補完

    private Collider2D _self;

    private void Awake()
    {
        if (boss == null) boss = GetComponentInParent<BigCore>();
        _self = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(Tags.PlayerBullet))
            return;
        // ここで最終処理へ通知（親の一元管理に渡すならイベント経由でもOK）
        switch (zoneType)
        {
            case BossZoneType.WeakPoint:
                boss?.Damage();
                break;
                /*case BossZoneType.Other:
                    break;*/
        }
        //Destroy(other.gameObject); // 弾を非アクティブにする
    }
}
