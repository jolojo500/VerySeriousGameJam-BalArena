using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace Venice {
    [Serializable]
    public class EntityCollision
    {
        public BallerinaEntity BallerinaEntity;
        public float GroundRayDistance = 0.6f;
        public float AirRayDistance = 0.6f;
        public LayerMask GroundMask;
        public float MaxAngle = 60f;
        public RaycastHit GroundHitInfo;

        public Vector3 CenterPoint => BallerinaEntity.transform.position + BallerinaEntity.transform.up * .5f;

        public bool GroundCollision()
        {
            if (Physics.Raycast(CenterPoint, -BallerinaEntity.SurfaceNormal, out GroundHitInfo, GroundRayDistance, GroundMask))
            {
                if (Vector3.Angle(GroundHitInfo.normal, Vector3.up) <= MaxAngle)
                {

                BallerinaEntity.SurfaceNormal = GroundHitInfo.normal;
                BallerinaEntity.SurfaceAngle = Vector3.Angle(GroundHitInfo.normal, Vector3.up);
                BallerinaEntity.Rb.position = GroundHitInfo.point;
                Debug.DrawRay(CenterPoint, -BallerinaEntity.SurfaceNormal * GroundRayDistance, Color.red);
                return true;
                }
                else
                {
                    BallerinaEntity.Rb.linearVelocity = Vector3.ClampMagnitude(BallerinaEntity.Rb.linearVelocity, 5);
                }
            }
            BallerinaEntity.SurfaceNormal = Vector3.zero;
            BallerinaEntity.SurfaceAngle = 0;
            Debug.DrawRay(CenterPoint, -BallerinaEntity.SurfaceNormal * GroundRayDistance, Color.green);
            return false;
        }
        public bool AirGroundCollision()
        {
            if (Physics.Raycast(CenterPoint, -Vector3.up, out GroundHitInfo, AirRayDistance, GroundMask))
            {
                if (Vector3.Angle(GroundHitInfo.normal, Vector3.up) <= MaxAngle)
                {
                    BallerinaEntity.SurfaceNormal = GroundHitInfo.normal;
                    BallerinaEntity.SurfaceAngle = Vector3.Angle(GroundHitInfo.normal, Vector3.up);
                    BallerinaEntity.Rb.position = GroundHitInfo.point;
                    BallerinaEntity.YSpeed = 0f;

                    Debug.DrawRay(CenterPoint, -BallerinaEntity.SurfaceNormal * AirRayDistance, Color.red);
                    return true;
                }
            }
            Debug.DrawRay(CenterPoint, -BallerinaEntity.SurfaceNormal * AirRayDistance, Color.green);
            return false;
        }

    }



}
