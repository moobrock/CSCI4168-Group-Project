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
                cow.SetActive(false);
                break;
            }
        }

        return Attack(damage);
    }

    public void ReturnCows(int count)
    {
        float damage = cows.Length > 0 ? maxHealth / (float)cows.Length : 0;

        health = Mathf.Min(maxHealth, health + damage);
    }
}
