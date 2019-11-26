using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            // check if this coin was clicked
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit[] hits = Physics.RaycastAll(ray);

            foreach (RaycastHit hit in hits)
            {
                if (hit.transform == this.transform)
                {
                    Debug.Log("Coin clicked!");

                    GameManager.gameManager.AddCoins(1);

                    Destroy(gameObject);
                }
            }
        }
    }
}
