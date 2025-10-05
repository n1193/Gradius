using System.Collections;
using UnityEngine;

public static class CoroutineUtil
{
    // runner は StartCoroutine できる MonoBehaviour を渡す
    public static IEnumerator WaitAll(MonoBehaviour runner, params IEnumerator[] routines)
    {
        int done = 0;
        foreach (var r in routines)
            runner.StartCoroutine(RunAndCount(runner, r, () => done++));

        yield return new WaitUntil(() => done == routines.Length);
    }

    private static IEnumerator RunAndCount(MonoBehaviour runner, IEnumerator routine, System.Action onComplete)
    {
        yield return runner.StartCoroutine(routine);
        onComplete?.Invoke();
    }
}
