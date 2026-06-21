//using NaughtyAttributes;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


namespace Venice
{
    public class REnemyObject : RWorldObject
    {
        [SerializeField]
        private int damage;
        public void OnTouchEntity(Player player)
        {
            player.TriggerDamage(damage);
        }
    }
}