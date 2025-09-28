using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerExplotion : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] sprites; // 0:水平, 1:下, 2:上
    private float changeInterval = 0.1f;
    private float time = 0f;
    private bool running;

    void Start()
    {
        time = 0f;
        running = false;
    }
    void Update()
    {
        if (running) return;
        time += Time.deltaTime;
        int frame = Mathf.FloorToInt(time / changeInterval);
        if (frame > sprites.Length)
        {
            StartCoroutine(scenechange());
        }
        spriteRenderer.sprite = sprites[frame];
    }

    IEnumerator scenechange()
    {
        running = true;
        spriteRenderer.enabled = false;
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneType.TitleScene.ToString());
    }    
}