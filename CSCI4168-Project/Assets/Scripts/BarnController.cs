using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarnController : TowerController
{
    private new void DestroyTower()
    {
        towerModel.SetActive(false);

        GameManager.gameManager.EndRound();
    }
}
