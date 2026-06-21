using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
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
