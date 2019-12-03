using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrebucheggPlayerAction : PlayerActionDraggable
{
    public Text countText;
    public Text costText;

    public int numAvailable = 3;
    public int cost = 3;

    public GameObject trebucheggPrefab;

    private Vector3 dropPosition;

    private new void Start()
    {
        base.Start();
        countText.text = numAvailable.ToString();
        costText.text = cost.ToString();
    }

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
            if (numAvailable > 0 && !other.Find("Trebuchegg(Clone)"))
            {
                GameObject trebuchegg = Instantiate(trebucheggPrefab, other);
                trebuchegg.transform.rotation = Quaternion.Euler(90, 0, 0);
                numAvailable--;
                countText.text = numAvailable.ToString();
            }
        }
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
