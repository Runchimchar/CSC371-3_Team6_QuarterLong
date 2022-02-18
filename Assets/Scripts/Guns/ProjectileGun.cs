using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Assets.Scripts.Guns;

public class ProjectileGun : AGun
{
    [SerializeField] private GameObject projectilePrefab;
    private float nextTimeToFire;
    [SerializeField] private float fireRate;
    [SerializeField] private uint bulletsPerShot;
    [SerializeField] private Camera mainCamera;
    private UnityAction<GameObject, Collision> hitCallback;
    private UnityAction<GameObject> projDestroyedCallback;
    public int bulletCollisionLayer;
    public void SetHitCallback(UnityAction<GameObject, Collision> action) { hitCallback = action; }

    public virtual void Awake()
    {
        hitCallback = ResolveHit;
        projDestroyedCallback = OnProjectileDestroyed;
        nextTimeToFire = Time.time;
    }

    public override void Shoot(Vector3 target) 
    {
        if (Time.time >= nextTimeToFire)
        {
            for (int i = 0; i < bulletsPerShot; i++)
            {
                GameObject projectileObj = Instantiate(projectilePrefab);
                projectileObj.transform.position = transform.position + mainCamera.transform.forward;
                Projectile projectile = projectileObj.GetComponent<Projectile>();
                projectile.Initialize(mainCamera.transform.forward, hitCallback, OnProjectileDestroyed);
            }
            // set cooldown on shots
        }
    }

    public virtual void ResolveHit(GameObject projectile, Collision target)
    {
        Debug.Log(string.Format("Default Projectile Gun Callback: My Projectile hit {0}", target.gameObject.name));
    }

    public virtual void OnProjectileDestroyed(GameObject projectile)
    {
        Debug.Log(string.Format("Default Projectile Destroy Callback: Destroying {0}", projectile.name));
    }

    public override void Reload() { }
}
