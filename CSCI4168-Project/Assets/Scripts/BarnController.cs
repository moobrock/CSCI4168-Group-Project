using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarnController : TowerController
{
    private new float maxHealth = 5f;

    private new void DestroyTower()
    {
        towerModel.SetActive(false);

        GameManager.gameManager.EndRound();
    }
}
