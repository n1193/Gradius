using UnityEngine;
using Zenject;

public class Effect_BombClearAll : MonoBehaviour, ICollectibleEffect
{
    [InjectOptional] EnemyManager _enemiesManager;
    [InjectOptional] SoundManager _sound;
    public void Apply(GameObject collector)
    {
        _sound?.SEPlay(SEType.Bomb);
        _enemiesManager?.ClearAll();
    }
}