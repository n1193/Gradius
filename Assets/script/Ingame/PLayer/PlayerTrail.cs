using Unity.VisualScripting;
using UnityEngine;

public class PlayerTrail : MonoBehaviour
{
    [SerializeField] int capacity = 2;
    static Vector3[] buf;
    static int head;

    void Awake() {
        buf = new Vector3[capacity];
        head = 0;
        }

    public void BufUpdate()
    {
        buf[head] = transform.position;
        head = (head + 1) % buf.Length;
    }
    public void initialize(Vector3 position)
    {
        for (int i = 0; i < buf.Length; i++)
        {
            buf[i] = position;
        }
    }
    public bool TryGetDelayed(int frames, out Vector3 pos)
    {
        if (frames >= buf.Length)
        {
            pos = default;
            return false;
        }
        int idx = (head - 1 - frames) % buf.Length;
        if (idx < 0)
        {
            idx += buf.Length;
        }
        pos = buf[idx]; return true;
    }

    public static void Prefill(Vector3 p)
    {
        for (int i = 0; i < buf.Length; i++)
        {
            buf[i] = p; head = 0;
        }
    }
}
