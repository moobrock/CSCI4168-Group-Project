using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOOrbit : MonoBehaviour
{
    float lerp;
    float diff = 0.05f;
    
    void FixedUpdate()
    {
        lerp += diff;

        if (lerp > 1)
        {
            lerp = 1f;
            diff = -diff;
        }

        if (lerp < 0)
        {
            lerp = 0f;
            diff = -diff;
        }

        transform.position = Vector3.Lerp(transform.parent.position - transform.parent.right, transform.parent.position + transform.parent.right, lerp);
    }
}
