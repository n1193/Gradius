using TMPro;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    [SerializeField] TMP_Text hpText;
    private int hp = 1;

    void Start()
    {
        hpText.text = hp.ToString("D1");
    }

    void Damage()
    {
        hp--;
        hpText.text = hp.ToString("D1");
    }
    public void AddLife()
    {
        hp++;
        hpText.text = hp.ToString("D1");
    }
}
