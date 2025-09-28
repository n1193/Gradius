using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using Zenject;


public class AddScore : MonoBehaviour
{
    private int score;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private ScoreManager scoreManager;
    private SoundManager soundManager;
    private BoxCollider2D boxCollider2D;
    [Inject]
    public void Construct(ScoreManager scoreManager, SoundManager soundManager)
    {
        this.scoreManager = scoreManager;
        this.soundManager = soundManager;
    }
    void Start()
    {
        score = 5000;
        spriteRenderer.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.Player))
        {
            spriteRenderer.enabled = true;
            scoreManager.AddScore(score);
            soundManager.SEPlay(SEType.Upgrade);
            boxCollider2D.enabled=false;
        }
    }
}
