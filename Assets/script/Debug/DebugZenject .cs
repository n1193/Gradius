using Zenject;
using UnityEngine;
public class DebugZenject : MonoBehaviour
{
    [Inject] SoundManager _snd;
    void Start()
    {
        Debug.Log($"SoundManager instance id = {_snd.GetInstanceID()}");
        Debug.Log($"ProjectContext alive? {FindObjectOfType<ProjectContext>() != null}");
    }
}