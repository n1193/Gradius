using UnityEngine;
using System.Collections.Generic;
using Zenject;
using System;
public class BulletPool : MonoBehaviour
{
    public int maxCount = 2;
    public float fireInterval = 0.25f;
    private WeaponType weaponType;
    private Queue<Bullet> pool = new Queue<Bullet>();
    private float lastFireTime;
    private Bullet bulletPrefab;
    private BulletOwner bulletOwner;
    private WeaponManager weaponManager;
    private DiContainer diContainer;
    private SEType sEType;
    private string tagName;
    private SoundManager soundManager;

    [Inject]
    public void Construct(WeaponManager weaponManager, DiContainer diContainer, SoundManager soundManager)
    {
        this.weaponManager = weaponManager;
        this.diContainer = diContainer;
        this.soundManager =soundManager;
    }

    public void Initialize(BulletOwner bulletOwner, WeaponType weaponType, int maxCount,float fireInterval= 0.25f,SEType sEType=SEType.None, string tag = Tags.PlayerBullet)
    {
        this.fireInterval = fireInterval;
        this.sEType = sEType;
        if (maxCount < 0) maxCount = 999;
        tagName = tag;
        this.maxCount = maxCount;
        this.weaponType = weaponType;
        pool.Clear();
        for (int i = 0; i < maxCount; i++)
        {
            Bullet bullet = diContainer.InstantiatePrefabForComponent<Bullet>(weaponManager.GetBulletPrefab(weaponType), transform);
            bullet.Initialize(bulletOwner, this, tagName);
            bullet.gameObject.SetActive(false);
            bullet.tag = tag;
            pool.Enqueue(bullet);
        }
    }
    public void CopySettingsFrom(BulletPool original)
    {
        bulletOwner = original.bulletOwner;
        weaponType = original.weaponType;
        bulletPrefab = original.bulletPrefab;
        maxCount = original.maxCount;
        fireInterval = original.fireInterval;
        sEType = SEType.None;
        Initialize(bulletOwner, weaponType, maxCount); // 新たにインスタンス生成
    }
    public bool CanFire()
    {
        return Time.time - lastFireTime >= fireInterval && pool.Count > 0;
    }

    public void Fire(Vector3 position, BulletOwner owner)
    {
        //Debug.Log("pool.Count" + pool.Count);
        if (!CanFire()) return;
        Bullet bullet = pool.Dequeue();
        bullet.transform.position = position;
        //bullet.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

        Bullet bulletComponent = bullet.GetComponent<Bullet>();
        bulletComponent?.refrash();

        bullet.gameObject.SetActive(true);
        //Debug.Log($"[BulletPool] Fired bullet. Remaining in pool: {pool.Count}");

        lastFireTime = Time.time;
        soundManager.SEPlay(sEType, 0.5f);
    }

    public void Return(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
        pool.Enqueue(bullet);
        Debug.Log("弾がなくなった");
    }
    public void Dead()
    {
        if (pool != null)
        {
            foreach (Bullet bullet in pool)
            {
                bullet.Die();
            }
        }
        Destroy(this.gameObject);
    }
    /*
    public void CleanMainShot()
    {
        foreach (BulletPool obj in mainShotBulletPool)
        {
            obj.Dead();
            Destroy(obj);
        }
    }
*/
}
