using Zenject;
using UnityEngine;

public class GlobalInstaller : MonoInstaller
{
    public SoundManager soundManagerPrefab;
    public override void InstallBindings()
    {
        Debug.Log("🛠 InstallBindings called");
        Container.Bind<SoundManager>()
            .FromComponentInNewPrefab(soundManagerPrefab)
            .AsSingle()
            .NonLazy();
    }
    void Awake()
    {
        Debug.Log("🚀 GlobalInstaller Start() called");
    }
}
