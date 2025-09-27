using UnityEngine;
using UnityEngine.UI;

class UpgradeCell : MonoBehaviour
{
    [SerializeField] public Sprite[] icons = new Sprite[2];
    [SerializeField] public Sprite[] maxLevelIcons = new Sprite[2];
    [SerializeField] Image image;
    public bool _isSelected = false;
    private bool _isMaxLevel = false;
    void Start()
    {
        _isSelected = false;
        _isMaxLevel = false;
        ApplySprite();
    }
    public void SetSelect(bool isSelected)
    {
        _isSelected = isSelected;
        ApplySprite();
    }

    public void SetMax(bool isMaxLevel)
    {
        _isMaxLevel = isMaxLevel;
        ApplySprite();
    }

    private void ApplySprite()
    {
        if (!image) return;

        var table = _isMaxLevel ? maxLevelIcons : icons;
        // 念のため長さチェック（サイズ2を想定）
        var idx = _isSelected ? 1 : 0;
        if (table != null && table.Length > idx && table[idx] != null)
            image.sprite = table[idx];
    }
}