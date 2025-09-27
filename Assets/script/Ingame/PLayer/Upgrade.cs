using UnityEngine;
using Zenject;
public static class UpgradeData
{
    public enum UpgradeType { Speed, Missile, Double, Laser, Option, Shield }
    public static readonly int[] MaxUpgradeLevels = { 5, 1, 1, 1, 2, 1 };
    public static int GetMax(UpgradeType t) => MaxUpgradeLevels[(int)t];

    public static UpgradeType TryGetUpgradeType(int index)
    {
        if (index>=0 && index <  System.Enum.GetValues(typeof(UpgradeType)).Length)
        {
            return (UpgradeType)index;
        }
        return UpgradeData.UpgradeType.Speed; // デフォルト値を返す
    }
}
public class Upgrade : MonoBehaviour
{
    private int[] _upgradeLevels = new int[UpgradeData.MaxUpgradeLevels.Length];
    private int selectedUpgradeIndex = -1;
    PlayerController playerController;
    [SerializeField] UpgradeCell[] upgradeCells = new UpgradeCell[UpgradeData.MaxUpgradeLevels.Length];
    private SoundManager SoundManager;

    [Inject]
    public void Construct(PlayerController playerController, SoundManager soundManager)
    {
        this.playerController = playerController;
        this.SoundManager = soundManager;
    }
   
    void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.U))
        {
            ChangeSelect();
        }
        #endif
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ApplyUpgrade();
        }
    }
    public void ChangeSelect(bool reset = false)
    {
        if (selectedUpgradeIndex >= 0)
        {
            upgradeCells[selectedUpgradeIndex].SetSelect(false);
        }
        if (reset)
        {
            selectedUpgradeIndex = -1;
        }
        else
        {
            SoundManager.SEPlay(SEType.GetUpgradeItem, 0.5f);
            selectedUpgradeIndex++;
            selectedUpgradeIndex %= UpgradeData.MaxUpgradeLevels.Length;
            upgradeCells[selectedUpgradeIndex].SetSelect(true);
        }
    
    }
    public void ApplyUpgrade()
    {
        if (selectedUpgradeIndex < 0)
            return;

        if (_upgradeLevels[selectedUpgradeIndex] >= UpgradeData.MaxUpgradeLevels[selectedUpgradeIndex])
            return;

        _upgradeLevels[selectedUpgradeIndex]++;
        if (_upgradeLevels[selectedUpgradeIndex] >= UpgradeData.MaxUpgradeLevels[selectedUpgradeIndex])
        {
            upgradeCells[selectedUpgradeIndex].SetMax(true);
        }
        playerController.ChangeWeapon(UpgradeData.TryGetUpgradeType(selectedUpgradeIndex));
        ChangeSelect(true);
        selectedUpgradeIndex = -1;
        SoundManager.SEPlay(SEType.Upgrade, 0.5f);
   }
    public void SetLevel(UpgradeData.UpgradeType upgradeType, int level)
    {
        _upgradeLevels[(int)upgradeType] = level;
        upgradeCells[(int)upgradeType].SetMax(level >= UpgradeData.MaxUpgradeLevels[(int)upgradeType]);
    }
}