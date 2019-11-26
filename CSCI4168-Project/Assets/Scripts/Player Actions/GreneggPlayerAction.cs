using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GreneggPlayerAction : PlayerActionDraggable
{
    public Text countText;
    public Text costText;

    public int numAvailable = 3;
    public int cost = 2;

    public GameObject greneggPrefab;

    private Vector3 dropPosition;

    private new void Start()
    {
        base.Start();
        countText.text = numAvailable.ToString();
        costText.text = cost.ToString();
    }

    protected override void OnDrop()
    {
        dropPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dropPosition.y = 0;

        if (numAvailable > 0)
        {
            Instantiate(greneggPrefab, dropPosition, Quaternion.Euler(90, 0, 0));
            numAvailable--;
            countText.text = numAvailable.ToString();
        }
        base.OnDrop();
    }

    protected override void OnRightClick()
    {
        if (GameManager.gameManager.SpendCoins(cost))
        {
            AddAvailable(1);
        }

        base.OnRightClick();
    }

    public void AddAvailable(int num)
    {
        numAvailable += num;
        countText.text = numAvailable.ToString();
    }
}
