
using UnityEngine;
using Zenject;
using TMPro;

public class AddLife : MonoBehaviour
{
    [SerializeField] TMP_Text hpText;
    [SerializeField]private PlayerLife playerLife;
    [SerializeField]private BoxCollider2D boxCollider2D;
    private SoundManager soundManager;
    [Inject]
    public void Construct(SoundManager soundManager)
    {
        this.soundManager = soundManager;
    }
    void Start()
    {
        hpText.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.Player))
        {
            hpText.enabled = true;
            playerLife.AddLife();
            soundManager.SEPlay(SEType.Upgrade);
            boxCollider2D.enabled=false;
        }
    }
}
