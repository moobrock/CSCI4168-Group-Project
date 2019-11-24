using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreneggPlayerAction : PlayerActionDraggable
{
    public GameObject greneggPrefab;

    private Vector3 dropPosition;

    protected override void OnDrop()
    {
        dropPosition = Camera.main.ScreenToWorldPoint(transform.position);
        dropPosition.y = 0;

        Instantiate(greneggPrefab, dropPosition, Quaternion.Euler(90, 0, 0));


        base.OnDrop();
    }
}
