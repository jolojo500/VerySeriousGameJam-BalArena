using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Venice
{

    public class DashRing : StageObject
    {

        public float LaunchSpeed;
        public void OnTouchIt(Player player)
        {
            player.Visual.Play("Jump");
            player.SurfaceNormal = Vector3.zero;
            player.transform.position = transform.position + transform.up;
            float xRot = transform.eulerAngles.x;
            player.Visual.PlayerSkin.transform.localRotation = Quaternion.Euler(
                -xRot,
                0,
                0
            );
            player.Rb.linearVelocity = transform.up * LaunchSpeed;
            player.OnObject(this, true);
        }

        void OnDrawGizmosSelected()
        {
            var launchDirection = transform.up;
            DrawTraj(launchDirection * LaunchSpeed);
        }

    }

}