using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarnController : TowerController
{
    public GameObject[] cows;

    public Transform cowPen;

    private new void DestroyTower()
    {
        towerModel.SetActive(false);

        GameManager.gameManager.EndRound();
    }

    public float AbductCow()
    {
        float damage = cows.Length > 0 ? maxHealth / (float)cows.Length : 0;

        // take away a cow
        foreach (GameObject cow in cows)
        {
            if (cow.activeInHierarchy)
            {
                StartCoroutine(Fade(cow.GetComponent<SpriteRenderer>()));
                break;
            }
        }

        return Attack(damage);
    }

    private IEnumerator Fade(SpriteRenderer spriteRenderer, bool fadeOut = false)
    {
        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime;

            if (fadeOut)
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, Mathf.Lerp(0f, 1f, time));
            else
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, Mathf.Lerp(1f, 0f, time));

            yield return new WaitForEndOfFrame();
        }

        spriteRenderer.gameObject.SetActive(fadeOut);
    }

    public void ReturnCows(int count)
    {
        Debug.Log("Returning " + count + " Cows");
        float damage = cows.Length > 0 ? maxHealth / (float)cows.Length : 0;

        int returned = 0;

        // put back cows
        foreach (GameObject cow in cows)
        {
            if (!cow.activeInHierarchy)
            {
                StartCoroutine(Fade(cow.GetComponent<SpriteRenderer>(), true));

                returned++;

                if (returned == count)
                    break;
            }
        }

        health = Mathf.Min(maxHealth, health + damage);
    }
}
