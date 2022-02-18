using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Guns
{
    public abstract class AGun: UnityEngine.MonoBehaviour
    {
        public abstract void Shoot(Vector3 target);
        public abstract void Reload();
    }
}
