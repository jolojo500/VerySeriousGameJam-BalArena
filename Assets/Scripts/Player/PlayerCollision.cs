using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace Venice {
    [Serializable]
    public class PlayerCollision
    {
        public Player Player;
        public float GroundRayDistance = 0.6f;
        public float AirRayDistance = 0.6f;
        public LayerMask GroundMask;
        public float MaxAngle = 60f;
        public RaycastHit GroundHitInfo;

        public Vector3 CenterPoint => Player.transform.position + Player.transform.up * .5f;

        public bool GroundCollision()
        {
            if (Physics.Raycast(CenterPoint, -Player.SurfaceNormal, out GroundHitInfo, GroundRayDistance, GroundMask))
            {
                if (Vector3.Angle(GroundHitInfo.normal, Vector3.up) <= MaxAngle)
                {

                Player.SurfaceNormal = GroundHitInfo.normal;
                Player.SurfaceAngle = Vector3.Angle(GroundHitInfo.normal, Vector3.up);
                Player.Rb.position = GroundHitInfo.point;
                Debug.DrawRay(CenterPoint, -Player.SurfaceNormal * GroundRayDistance, Color.red);
                return true;
                }
                else
                {
                    Player.Rb.linearVelocity = Vector3.ClampMagnitude(Player.Rb.linearVelocity, 5);
                }
            }
            Player.SurfaceNormal = Vector3.zero;
            Player.SurfaceAngle = 0;
            Debug.DrawRay(CenterPoint, -Player.SurfaceNormal * GroundRayDistance, Color.green);
            return false;
        }
        public bool AirGroundCollision()
        {
            if (Physics.Raycast(CenterPoint, -Vector3.up, out GroundHitInfo, AirRayDistance, GroundMask))
            {
                if (Vector3.Angle(GroundHitInfo.normal, Vector3.up) <= MaxAngle)
                {
                    Player.SurfaceNormal = GroundHitInfo.normal;
                    Player.SurfaceAngle = Vector3.Angle(GroundHitInfo.normal, Vector3.up);
                    Player.Rb.position = GroundHitInfo.point;
                    Player.YSpeed = 0f;

                    Debug.DrawRay(CenterPoint, -Player.SurfaceNormal * AirRayDistance, Color.red);
                    return true;
                }
            }
            Debug.DrawRay(CenterPoint, -Player.SurfaceNormal * AirRayDistance, Color.green);
            return false;
        }

    }



}
