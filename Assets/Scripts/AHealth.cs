using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Enemies
{
    public abstract class AHealth : UnityEngine.MonoBehaviour
    {
        public abstract void TakeDamage(float damage);
        public abstract void HealDamage(float healing);

        public abstract float GetHealth();
    }
}
