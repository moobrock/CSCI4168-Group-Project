using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private RectTransform cameraBoundary;   // world-space rect transform boundary of the map

    private float boundaryWidth = 80f;

    private float speed = 0.015f;

    private void Start()
    {
        cameraBoundary = GameObject.Find("Level Base")?.transform.Find("Camera Boundary")?.GetComponent<RectTransform>();
    }

    private void FixedUpdate()
    {
        // get mouse position in screen-space
        Vector3 mousePositionScreen = Input.mousePosition;

        // get mouse position in world-space
        Vector3 mousePositionWorld = Camera.main.ScreenToWorldPoint(mousePositionScreen);
        mousePositionWorld.y = 10;

        // only want to move the camera if the mouse is in the game screen
        if (mousePositionScreen.x >= 0 && 
            mousePositionScreen.x <= Screen.width && 
            mousePositionScreen.y >= 0 && 
            mousePositionScreen.y <= Screen.height)
        {
            // distance of mouse position from edge of game screen
            float diff = Mathf.Max(boundaryWidth - mousePositionScreen.x,
                mousePositionScreen.x - (Screen.width - boundaryWidth),
                boundaryWidth - mousePositionScreen.y,
                mousePositionScreen.y - (Screen.height - boundaryWidth));

            // move camera if mouse position is near edge of the screen
            if (diff > 0f)
            {
                transform.position = Vector3.MoveTowards(transform.position, mousePositionWorld, speed * diff);
            }
        }
    }
}
