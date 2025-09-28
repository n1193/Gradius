using NUnit.Framework;
using UnityEngine;
using Zenject;

public class InGameInstaller : MonoInstaller
{
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Upgrade upgrade;
    [SerializeField] public ItemFactory itemFactory;
    [SerializeField] private DropManager dropManager;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private ScrollDirector scrollDirector;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private SoundManager soundManager;
    public override void InstallBindings()
    {
        //Container.Bind<TimeManager>().FromInstance(timeManager).AsSingle();
        // ここに他のマネージャーもバインド…
        Container.Bind<WeaponManager>().FromInstance(weaponManager).AsSingle();
        Container.Bind<PlayerController>().FromInstance(playerController).AsSingle();
        Container.Bind<Upgrade>().FromInstance(upgrade).AsSingle();
        Container.Bind<ItemFactory>().FromInstance(itemFactory).AsSingle();
        Container.Bind<DropManager>().FromInstance(dropManager).AsSingle();
        Container.Bind<EnemyManager>().FromInstance(enemyManager).AsSingle();
        Container.Bind<SpawnManager>().FromInstance(spawnManager).AsSingle();
        Container.Bind<ScrollDirector>().FromInstance(scrollDirector).AsSingle();
        Container.Bind<ScoreManager>().FromInstance(scoreManager).AsSingle();
        Container.Bind<SoundManager>().FromInstance(soundManager).AsSingle();
    }
}