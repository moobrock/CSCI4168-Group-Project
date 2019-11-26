using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrebucheggPlayerAction : PlayerActionDraggable
{
    public GameObject trebucheggPrefab;

    private Vector3 dropPosition;

    protected override void OnDrop()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit[] hits = Physics.RaycastAll(ray);
        
        foreach (RaycastHit hit in hits)
        {
            OnRaycastHit(hit.transform);
        }

        base.OnDrop();
    }

    private void OnRaycastHit(Transform other)
    {
        Debug.Log("RayCastHit - " + other.name);

        if (other.tag == "Defense Placement")
        {
            Instantiate(trebucheggPrefab, other.transform.position, Quaternion.Euler(90, 0, 0));
        }
    }
}
