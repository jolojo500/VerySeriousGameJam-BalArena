using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Venice;

public class PlayerEntity : Entity
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
    public Vector3 HorizontalVelocity => Vector3.ProjectOnPlane(Rb.linearVelocity, SurfaceNormal==Vector3.zero?Vector3.up:SurfaceNormal);
    public Vector3 VerticalVelocity => Vector3.Project(Rb.linearVelocity, SurfaceNormal == Vector3.zero ? Vector3.up : SurfaceNormal);

    public Rigidbody Rb;
    public PhysicsInfo PhysicsInfo;
    public PlayerVisual Visual;

    public bool IsInvulnerable => _invulnerabilityTimer > 0f;

    public float InputDisableTimer = 0f;

    private float _invulnerabilityTimer = 0f;

    public bool DefinitiveInputLock = false;



    public virtual void Init()
    {
        Rb = GetComponent<Rigidbody>();
    }


    void Start()
    {
        
    }


    void Update()
    {
    }
    public virtual void BindValues() //From Rerun until alternative
    {
        Vector3 flatVel = HorizontalVelocity;

        if (SurfaceNormal != Vector3.zero)
        {
            if (flatVel != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(Rb.linearVelocity, SurfaceNormal);
            }

            transform.rotation = Quaternion.FromToRotation(transform.up, SurfaceNormal) * transform.rotation;
        }
        else
        {
            if (flatVel != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(flatVel, transform.up);
            }
            else
            {
            }

            transform.rotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation; // physics-wise, the rigidbody will instantly snap to be upright in the air.
        }
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

    public void ToggleInvulnerability(float v)
    {
        _invulnerabilityTimer = v;
    }

    public void HandleInvulnerability()
    {
        HandleTimer(ref _invulnerabilityTimer);
        if (!DefinitiveInputLock)
        {
            if (HandleTimer(ref InputDisableTimer))
            {
                ObjectEnableInput();
            }
        }
    }




    public void SetDefinitiveInputLock(bool inputLocked)
    {
        DefinitiveInputLock = inputLocked;
        //LockInput
    }

    public void ObjectEnableInput()
    {

        InputDisableTimer = 0;
        //UnlockInput
    }


    public void ObjectDisableInput(float time)
    {

        InputDisableTimer = time;
        //LockInput
    }
}
