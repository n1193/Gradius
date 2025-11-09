using UnityEngine;
using Zenject;
public class Effect_UpgradeSelect : MonoBehaviour, ICollectibleEffect
{
    [Inject] Upgrade _upgrade;
    public void Apply(GameObject collector) => _upgrade.ChangeSelect();
}