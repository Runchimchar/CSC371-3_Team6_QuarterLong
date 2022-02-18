using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Guns
{
    class HarpoonGun: ProjectileGun
    {
        [SerializeField] private GameObject player;
        [SerializeField] private float pullSpeed = 0;
        [SerializeField] private float rangeBias = 0;
        [SerializeField, Range(0.0f, 10.0f)] private float maxPullTime = 2.0f;
        private bool canFire = true;
        private Vector3 rangeBiasVec;
        private bool firstCollision;
        public override void Awake()
        {
            base.Awake();
            SetHitCallback(ResolveHit);
            firstCollision = true;
            rangeBiasVec = new Vector3(rangeBias, rangeBias, rangeBias);
        }

        public override void ResolveHit(GameObject projectile, Collision target)
        {
            if (firstCollision)
            {
                firstCollision = false;
                Debug.Log(string.Format("Resolving hit with: {0}", target.transform.name));
                StartCoroutine(pullToTarget(projectile, target.GetContact(0).point));
            }
        }

        public override void OnProjectileDestroyed(GameObject projectile)
        {
            canFire = true;
        }

        public override void Shoot(Vector3 target)
        {
            if (canFire)
            {
                canFire = false;
                firstCollision = true;
                base.Shoot(target);
            }
        }

        private bool ColliderNotInRange(Bounds playerBound, Vector3 target)
        {
            Vector3 rangeMax = playerBound.max + rangeBiasVec,
                rangeMin = playerBound.min - rangeBiasVec;
            return target.x > rangeMax.x || target.x < rangeMin.x ||
                target.y > rangeMax.y || target.y < rangeMin.y ||
                target.z > rangeMax.z || target.z < rangeMin.z;
        }

        IEnumerator pullToTarget(GameObject projectile, Vector3 target)
        {
            CharacterController playerControl = player.GetComponent<CharacterController>();
            if (playerControl == null)
            {
                Debug.LogError("Player character controller not found");
                yield break;
            }

            CapsuleCollider playerCapsule = player.GetComponent<CapsuleCollider>();
            if (playerCapsule == null)
            {
                Debug.LogError("Player capsule not found");
                yield break;
            }

            float timer = maxPullTime;
            Vector3 pullforce;
            while (timer > 0.0f && ColliderNotInRange(playerCapsule.bounds, target))
            {
                pullforce = (target - player.transform.position);
                playerControl.Move(pullforce.normalized * pullSpeed);
                timer -= Time.deltaTime;
                yield return null;

            }
            Destroy(projectile);
            canFire = true;
            yield break;
        }
    }
}
