using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private RectTransform cameraBoundary;   // world-space rect transform boundary of the map

    private float boundaryWidth = 40f;

    private float speed = 0.5f;

    private void Start()
    {
        cameraBoundary = GameObject.Find("Level Base")?.transform.Find("Camera Boundary")?.GetComponent<RectTransform>();
    }

    private void Update()
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
            // move camera if mouse position is near edge of the screen
            if ((mousePositionScreen.x < boundaryWidth) ||                  // left
                (mousePositionScreen.x > (Screen.width - boundaryWidth)) || // right
                (mousePositionScreen.y < boundaryWidth) ||                  // down
                (mousePositionScreen.y > (Screen.height - boundaryWidth)))  // up
            {
                transform.position = Vector3.MoveTowards(transform.position, mousePositionWorld, speed);

                // TODO: keep camera within bounds of cameraBoundary
                /*
                Vector3[] frustumCorners = new Vector3[4];
                Camera.main.CalculateFrustumCorners(new Rect(0, 0, 1, 1), Camera.main.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);

                for (int i = 0; i < 4; i++)
                {
                    var worldSpaceCorner = transform.TransformVector(frustumCorners[i]);
                    Debug.DrawRay(transform.position, worldSpaceCorner, Color.blue);
                }

                Vector3 center = new Vector3(cameraBoundary.rect.center.x + cameraBoundary.transform.position.x, 0, cameraBoundary.rect.center.y + cameraBoundary.transform.position.z);
                Vector3 min = new Vector3(cameraBoundary.rect.xMin, 0, cameraBoundary.rect.yMin);
                Vector3 max = new Vector3(cameraBoundary.rect.xMax, 0, cameraBoundary.rect.yMax);

                Debug.Log("xMin: " + min.x + " zMin: " + min.z + " xMax: " + max.x + " zMax: " + max.z);

                Debug.DrawRay(center, min, Color.green);
                Debug.DrawRay(center, max, Color.red);

                Vector3 newPosition = transform.position;
                */
            }
        }
    }
}
