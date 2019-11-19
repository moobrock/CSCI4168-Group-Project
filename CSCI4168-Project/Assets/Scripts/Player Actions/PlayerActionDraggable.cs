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
        if (Input.GetMouseButtonDown(0))
        {
            if (rectTransform.rect.Contains(rectTransform.InverseTransformPoint(Input.mousePosition)))
            {
                offset = Input.mousePosition - startPos;

                Debug.Log("Start drag");

                StartCoroutine(DragIcon());
            }
        }
    }

    protected IEnumerator DragIcon()
    {
        while (Input.GetMouseButton(0))
        {
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
