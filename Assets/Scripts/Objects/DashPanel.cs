using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Venice
{

    public class DashPanel : StageObject
    {
        public float LaunchSpeed;
        public void OnTouchIt(Player player)
        {
            player.SurfaceNormal = transform.up;
            player.transform.position = transform.position + transform.forward;
            player.SetHorizontalVelocity(transform.forward * Mathf.Max(Mathf.Abs(LaunchSpeed), Mathf.Abs(player.HorizontalVelocity.magnitude)));
            player.OnObject(this, false);
        }
    }

}