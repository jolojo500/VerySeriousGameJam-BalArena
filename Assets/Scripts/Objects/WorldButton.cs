using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldButton : StageObject
{




    public void OnHitByProjectile()
    {
        Debug.Log("SwitchPressed");
        GetComponent<MeshRenderer>().material.color = Random.ColorHSV();
    }

}
