using TMPro;
using UnityEngine;
using System.Collections;
using Zenject;
using System;

public class PlayerLife : MonoBehaviour
{
    [SerializeField] TMP_Text hpText;
    private int _hp = 3;
    GameManager gameManager;

    [Inject]
    public void Construct(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    public int HP
    {
        get => _hp;         // ← ここで同じ _hp にアクセス
        private set => _hp = value; // ← ここでも同じ _hp に代入
    }
    void Start()
    {
        hpText.text = _hp.ToString("D1");
    }

    public void TakeDamage()
    {
        _hp--;
        hpText.text = _hp.ToString("D1");
        StartCoroutine(Die());    
    }

    public void AddLife()
    {
        _hp++;
        hpText.text = _hp.ToString("D1");
    }
    IEnumerator Die()
    {
        yield return new WaitForSeconds(1f);
        if (_hp <= 0)
        {
            gameManager.OnGameOver();
        }
        else
        {
            gameManager.RestartGame();
        }
    }
}
