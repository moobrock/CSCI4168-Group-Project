using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOOrbit : MonoBehaviour
{
    float lerp;
    float diff = 0.05f;

    float flipRotationTime = 0.25f;
    bool flipRotationTrigger;

    float time = 0;

    Vector3 position;

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

        position = Vector3.Slerp(transform.parent.position - transform.parent.right, transform.parent.position + transform.parent.right, lerp);
        position.y = 3;

        transform.position = position;

        time += Time.deltaTime;
        if (time > flipRotationTime)
        {
            time = 0f;
            FlipRotation();
        }
    }

    void FlipRotation ()
    {
        if (flipRotationTrigger)
        {
            transform.rotation = Quaternion.Euler(90, 0, 0);
        }

        else
        {
            transform.rotation = Quaternion.Euler(90, 0, 90);
        }

        flipRotationTrigger = !flipRotationTrigger;
    }
}
