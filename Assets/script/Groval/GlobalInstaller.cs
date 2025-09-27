using Zenject;
using UnityEngine;

public class GlobalInstaller : MonoInstaller
{
    public SoundManager soundManagerPrefab;
    public override void InstallBindings()
    {
        Debug.Log("✅ GlobalInstaller: InstallBindings() called");
        Container.Bind<SoundManager>()
                 .FromComponentInNewPrefab(soundManagerPrefab) // ← これ！
                 .AsSingle()
                 .NonLazy();
    }
}