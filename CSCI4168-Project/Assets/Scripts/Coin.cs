using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    AudioSource audio;

    bool collected;

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!collected && Input.GetMouseButtonUp(0))
        {
            // check if this coin was clicked
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit[] hits = Physics.RaycastAll(ray);

            foreach (RaycastHit hit in hits)
            {
                if (hit.transform == this.transform)
                {
                    StartCoroutine(PickupCoin());
                }
            }
        }
    }

    private IEnumerator PickupCoin()
    {
        collected = true;

        audio?.Play();
        GameManager.gameManager.AddCoins(1);

        while (audio?.isPlaying ?? false)
        {
            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
    }
}
