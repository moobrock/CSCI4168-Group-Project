using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private RectTransform cameraBoundary;   // world-space rect transform boundary of the map

    private float boundaryWidth = 80f;

    private float speed = 0.005f;

    private Vector3[] worldBoundaryCorners = new Vector3[4];
    private Vector3[] worldCameraCorners = new Vector3[4];

    private Camera camera;

    private void Start()
    {
        camera = GetComponent<Camera>();
        cameraBoundary = GameObject.Find("Level Base")?.transform.Find("Camera Boundary")?.GetComponent<RectTransform>();
    }

    private void FixedUpdate()
    {
        // get mouse position in screen-space
        Vector3 mousePositionScreen = Input.mousePosition;

        // get mouse position in world-space
        Vector3 mousePositionWorld = camera.ScreenToWorldPoint(mousePositionScreen);
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

            // check if camera left boundary
            if (cameraBoundary != null)
            {
                cameraBoundary.GetWorldCorners(worldBoundaryCorners);

                diff = transform.position.x - worldBoundaryCorners[1].x;

                if (diff < 0f)
                {
                    transform.position = new Vector3(transform.position.x - diff, transform.position.y, transform.position.z);
                }

                if (diff > cameraBoundary.rect.width)
                {
                    transform.position = new Vector3(transform.position.x + cameraBoundary.rect.width - diff, transform.position.y, transform.position.z);
                }

                diff = transform.position.z - worldBoundaryCorners[3].z;

                if (diff < 0f)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - diff);
                }

                if (diff > cameraBoundary.rect.height)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + cameraBoundary.rect.height - diff);
                }
            }
        }
    }
}
