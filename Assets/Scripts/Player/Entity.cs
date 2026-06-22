using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Venice;

public class Entity : MonoBehaviour
{


    public float XSpeed
    {
        get
        {
            return Rb.linearVelocity.x;
        }

        set
        {
            Rb.linearVelocity = new(value, Rb.linearVelocity.y, Rb.linearVelocity.z);
        }
    }
    public float YSpeed
    {
        get
        {
            return Rb.linearVelocity.y;
        }

        set
        {
            Rb.linearVelocity = new(Rb.linearVelocity.x, value, Rb.linearVelocity.z);
        }
    }
    public float ZSpeed
    {
        get
        {
            return Rb.linearVelocity.z;
        }

        set
        {
            Rb.linearVelocity = new(Rb.linearVelocity.x, Rb.linearVelocity.y, value);
        }
    }

    public Vector3 SurfaceNormal = Vector3.up;
    public float SurfaceAngle = 0f; // Angle in degrees between surface normal and Vector3.up
    public float RadSurfaceAngle => SurfaceAngle * Mathf.Deg2Rad; // Angle in radians between surface normal and Vector3.up
    public Vector3 HorizontalVelocity => Vector3.ProjectOnPlane(Rb.linearVelocity, SurfaceNormal == Vector3.zero ? Vector3.up : SurfaceNormal);
    public Vector3 VerticalVelocity => Vector3.Project(Rb.linearVelocity, SurfaceNormal == Vector3.zero ? Vector3.up : SurfaceNormal);

    public Rigidbody Rb;



    public virtual void Init()
    {
        Rb = GetComponent<Rigidbody>();
    }
    public void SetHorizontalVelocity(Vector3 newVel)
    {
        Vector3 verticalVel = VerticalVelocity;
        Rb.linearVelocity = newVel + verticalVel;
    }
    public void SetVerticalVelocity(Vector3 newVel)
    {
        Vector3 horizontalVel = HorizontalVelocity;
        Rb.linearVelocity = newVel + horizontalVel;
    }


    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

    public bool HandleTimer(ref float timer)
    {
        return HandleTimer(ref timer, Time.deltaTime);
    }
    public bool HandleFixedTimer(ref float timer)
    {
        return HandleTimer(ref timer, Time.fixedDeltaTime);
    }

    public bool HandleTimer(ref float timer, float delta)
    {
        if (timer > 0)
        {
            timer = Mathf.Clamp(timer - delta, 0f, timer);
            if (timer <= 0f)
            {
                timer = 0;
                return true;
            }
        }

        return false;
    }


}
