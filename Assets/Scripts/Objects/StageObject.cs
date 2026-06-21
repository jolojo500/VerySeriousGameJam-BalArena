using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Venice;
public enum InputLockType
{
    None,
    Time,
    StopOnLand,
}
public class StageObject : RWorldObject
{

    
    int trajectoryResolution = 15;
    float trajectoryFriction = 80f;
    float trajectoryAcceleration = 15f;
    float maxTrajectoryLength = 50;
    float maxTrajectoryLineTime = 2f;
    float trajectoryGravity = 20f;
    public InputLockType InputLockType = InputLockType.None;
    public float OutOfControlTime = .25f;

    public virtual void DebugDrawTrajectory(Vector3 velocity, Color color)
    {
        DrawTraj(velocity, Color.grey, trajectoryFriction);
        DrawTraj(velocity, color);
        DrawTraj(velocity, Color.grey, -trajectoryAcceleration);
    }
    public void DrawTraj(Vector3 velocity, float friction = 0)
    {
        DrawTraj(velocity, Color.red, friction);
    }

    public void DrawTraj(Vector3 velocity, Color color, float friction = 0)
    {
        Vector3 origin = transform.position + (velocity.normalized);
        Vector3 prevPoint = origin;
        float currentLength = maxTrajectoryLength;

        Vector3 horizontal = new Vector3(velocity.x, 0f, velocity.z);
        float initialHorizontalSpeed = horizontal.magnitude;
        Vector3 horizontalDir = horizontal.normalized;

        float verticalVelocity = velocity.y;

        for (int i = 1; i <= trajectoryResolution; i++)
        {
            float t = i * (maxTrajectoryLineTime / trajectoryResolution);

            float horizontalSpeed = Mathf.Max(0f, initialHorizontalSpeed - friction * t);
            Vector3 horizontalDisplacement = horizontalDir * horizontalSpeed * t;
            float verticalDisplacement = verticalVelocity * t + 0.5f * -trajectoryGravity * t * t;

            Vector3 displacement = horizontalDisplacement + Vector3.up * verticalDisplacement;
            Vector3 currentPoint = origin + displacement;

            Debug.DrawLine(prevPoint, currentPoint, color);
            currentLength -= Vector3.Distance(currentPoint, prevPoint);
            if (currentLength <= 0)
            {
                break;
            }
            prevPoint = currentPoint;
        }
    }

}
