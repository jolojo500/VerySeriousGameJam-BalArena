using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Venice
{
    [Serializable]
    public class PlayerControllers
    {
        Player Player;
        public List<PlayerController> Controllers = new List<PlayerController>();

        public void Init(Player player)
        {
            Player = player;
            foreach (PlayerController controller in Controllers)
            {
                controller.Init(player);
            }
        }

        public void Update()
        {
            foreach (PlayerController controller in Controllers)
            {
                controller.OnUpdate();
            }
        }

        public void AddController(PlayerController controller)
        {
            Controllers.Add(controller);
            if (Player != null)
            {
                controller.Init(Player);
            }
        }


        public void FixedUpdate()
        {
            foreach (PlayerController controller in Controllers)
            {
                controller.OnFixedUpdate();
            }
        }

        public T GetController<T>() where T : PlayerController
        {
            foreach (PlayerController controller in Controllers)
            {
                if (controller is T tController)
                {
                    return tController;
                }
            }
            return null;
        }
    }
}