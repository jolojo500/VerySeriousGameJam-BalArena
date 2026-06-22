using System;
using Unity.VisualScripting;
using UnityEngine;
using Venice;

public class PS_Death : BallerinaState
{

    public float SpawnTimer = 3f;

    public PS_Death() : base(67)
    {
    }
    public override void OnEnter()
    {
        SpawnTimer = 3f;
        Visual.gameObject.SetActive(false);
        Entity.PlayerCollider.enabled = false;
    }

    public override void OnExit()
    {
        Visual.gameObject.SetActive(true);
        Entity.PlayerCollider.enabled = true;
    }

    public override void OnFixedUpdate()
    {
        if(Entity.HandleFixedTimer(ref SpawnTimer))
        {
            Entity.transform.position = Vector3.zero + Vector3.up * 0.5f;
            Machine.Set<PS_Air>();
        }
    }


}