using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Guns
{
    interface Projectile
    {
        public void Initialize(Vector3 direction, UnityAction<GameObject, Collision> hitCallback,
            UnityAction<GameObject> projDestroyedCallback);
    }
}
