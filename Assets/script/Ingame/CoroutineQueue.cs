using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineQueue : MonoBehaviour
{
    private readonly Queue<IEnumerator> _queue = new Queue<IEnumerator>();
    private bool _running;

    public void Enqueue(IEnumerator routine)
    {
        _queue.Enqueue(routine);
        if (!_running) StartCoroutine(Process());
    }

    private IEnumerator Process()
    {
        _running = true;
        while (_queue.Count > 0)
            yield return StartCoroutine(_queue.Dequeue());
        _running = false;
    }

    public void Clear() => _queue.Clear();
}
