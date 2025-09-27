using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ScoreManager : MonoBehaviour
{
    [SerializeField]TMP_Text  scoreText;
    private int score = 0;
    public int Score => score;
    private void Awake()
    {

    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            AddScore(100);
        }
    }
    public void AddScore(int value)
    {
        score += value;
        scoreText.text = score.ToString("D7");
    }
    public void ResetScore()
    {
        score = 0;
    }
}
