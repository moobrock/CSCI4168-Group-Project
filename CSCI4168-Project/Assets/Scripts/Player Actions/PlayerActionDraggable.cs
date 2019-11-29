using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerActionDraggable : MonoBehaviour
{
    protected Vector3 startPos;
    protected Vector3 offset;

    protected RectTransform rectTransform;

    protected virtual void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = transform.position;
    }

    protected virtual void Update()
    {
        // left click to start drag
        if (Input.GetMouseButtonDown(0))
        {
            if (rectTransform.rect.Contains(rectTransform.InverseTransformPoint(Input.mousePosition)))
            {
                offset = Input.mousePosition - startPos;
                StartDrag();
            }
        }

        // right click to buy
        else if (Input.GetMouseButtonDown(1))
        {
            if (rectTransform.rect.Contains(rectTransform.InverseTransformPoint(Input.mousePosition)))
            {
                OnRightClick();
            }
        }
    }

    protected virtual void OnRightClick()
    {
        // override this to take action on right click 
        Debug.Log("Player action right clicked!");
    }

    protected virtual void StartDrag()
    {
        StartCoroutine(DragIcon());
    }


    protected IEnumerator DragIcon()
    {
        // left click to drag, right to cancel
        while (Input.GetMouseButton(0))
        {
            if (Input.GetMouseButton(1))
            {
                yield break;
            }

            transform.position = Input.mousePosition - offset;

            yield return new WaitForEndOfFrame();
        }

        OnDrop();
    }
    

    protected virtual void OnDrop()
    {
        Debug.Log("Stop drag");

        transform.position = startPos;
    }
}
