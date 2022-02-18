using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
namespace Assets.Scripts.Guns
{
    class Harpoon : MonoBehaviour, Projectile
    {
        [SerializeField] private float startSpeed;
        private float maxLifetime = 3.0f;
        private float timer;
        private bool tickDownLifetime = false;
        private Rigidbody rb;
        private UnityEvent<GameObject, Collision> hitObjectEvent;
        private UnityEvent<GameObject> projDestroyedEvent;

        public void Initialize(Vector3 direction, UnityAction<GameObject, Collision> hitCallback,
            UnityAction<GameObject> projDestroyedCallback)
        {
            hitObjectEvent = new UnityEvent<GameObject, Collision>();
            hitObjectEvent.AddListener(hitCallback);
            rb = GetComponent<Rigidbody>();
            rb.velocity = direction.normalized * startSpeed;
            transform.rotation = Quaternion.LookRotation(direction);
            projDestroyedEvent = new UnityEvent<GameObject>();
            projDestroyedEvent.AddListener(projDestroyedCallback);
            tickDownLifetime = true;
            timer = maxLifetime;
        }

        public void Update()
        {
            if (tickDownLifetime) {
                timer -= Time.deltaTime;
                if (timer < 0) {
                    Debug.Log("Cleaning Up");
                    projDestroyedEvent.Invoke(gameObject);
                    Destroy(gameObject);
                }
            }
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Collidable")
            {
                hitObjectEvent.Invoke(gameObject, collision);
                tickDownLifetime = false;
                rb.velocity = new Vector3(0, 0, 0);
                rb.useGravity = false;
                rb.detectCollisions = false;
                rb.freezeRotation = true;
            }
        }
    }
}
