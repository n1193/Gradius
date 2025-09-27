using UnityEngine;

public class DebugUpdater : MonoBehaviour
{
    void Awake()
    {
        var prefab = Resources.Load<GameObject>("ProjectContext");
        Debug.Log("Loaded ProjectContext: " + prefab);

        if (prefab != null)
        {
            var instance = Instantiate(prefab);
            Debug.Log("Manually instantiated ProjectContext: " + instance);
        }
        else
        {
            Debug.LogWarning("ProjectContext not found in Resources.");
        }
    }
}
