using System;
using System.Collections;
using UnityEngine;

namespace Venice
{
    [Serializable]
    public class PlayerController : Controller<Player>
    {
        public Player Player => Context;
        public NeoInputManager Input => Player.InputManager;


        public override void Init(Player player)
        {
            Context = player;
        }


        public virtual void OnUpdate()
        {

        }


        public virtual void OnFixedUpdate()
        {

        }
    }
}