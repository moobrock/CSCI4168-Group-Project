using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlowerPotPlayerAction : PlayerActionDraggable
{
    public GameObject brokenPotPrefab;

    private float range = 10.0f;

    private bool CheckPoint(Vector3 center, float range, out Vector3 result)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(center, out hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        else
        {
            result = Vector3.zero;
            return false;
        }
    }
    
    protected override void OnDrop()
    {
        Vector3 inPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        inPoint.y = 0;
        Vector3 outPoint;

        Debug.Log("In point: " + inPoint.ToString());

        if (CheckPoint(inPoint, range, out outPoint))
        {
            Debug.Log("Point on navmesh found");

            GameObject.Instantiate(brokenPotPrefab, outPoint, Quaternion.Euler(90, 0 ,0));
        }

        else
        {
            Debug.Log("Point on navmesh not found");
        }

        base.OnDrop();
    }
}
