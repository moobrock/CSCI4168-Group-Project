using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarnController : TowerController
{
    private int numCows = 3;

    private new void DestroyTower()
    {
        towerModel.SetActive(false);

        GameManager.gameManager.EndRound();
    }

    public float AbductCow()
    {
        float damage = maxHealth / (float)numCows;

        return Attack(damage);
    }

    public void ReturnCows(int count)
    {
        float damage = maxHealth / (float)numCows * count;

        health = Mathf.Min(maxHealth, health + damage);
    }
}
