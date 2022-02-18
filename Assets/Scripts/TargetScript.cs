using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Enemies
{
    public class TargetScript : AHealth
    {
        public float health = 0;
        public float hitTime = 1.5f;

        public Material defaultMat;
        public Material hitMat;

        MeshRenderer rend;
        bool hit = false;

        private void Start()
        {
            rend = GetComponent<MeshRenderer>();
            SetClear();
        }

        private void SetHit() {
            rend.material = hitMat;
            hit = true;
        }

        private void SetClear() {
            rend.material = defaultMat;
            hit = false;
        }

        public override void TakeDamage(float damage)
        {
            if (!hit) {
                SetHit();
                Invoke("SetClear", hitTime);
            }
        }

        public override void HealDamage(float healing)
        {
        }

        public override float GetHealth()
        {
            return 0;
        }
    }
}
