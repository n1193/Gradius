using Unity.VisualScripting;
using UnityEngine;


public class EnemyExplotion : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites; // 0:水平, 1:下, 2:上
    private float changeInterval = 0.1f;
    private float time  = 0f;
    void Start()
    {
        time = 0f;
    }
    void Update()
    {
        time += Time.deltaTime;
        int frame = Mathf.FloorToInt(time / changeInterval);
  
        if (frame > sprites.Length )
        {
            Destroy(gameObject);
        }
        spriteRenderer.sprite = sprites[frame];

    }
}