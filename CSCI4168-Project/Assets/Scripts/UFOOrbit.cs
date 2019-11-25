using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOOrbit : MonoBehaviour
{
    Vector3 startPos;
    Vector3 endPos;
    float lerp;
    float diff = 0.005f;

    private void Start()
    {
        startPos = transform.position + Vector3.left;
        endPos = transform.position + Vector3.right;
    }

    void FixedUpdate()
    {
        lerp += diff;

        if (lerp > 1)
        {
            lerp = 1f;
            diff = -0.1f;
        }

        if (lerp < 0)
        {
            lerp = 0f;
            diff = 0.1f;
        }

        transform.position = Vector3.Lerp(startPos, endPos, lerp);
    }
}
