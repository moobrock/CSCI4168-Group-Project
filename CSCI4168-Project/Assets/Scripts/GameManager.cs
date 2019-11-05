using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform[] towerAttackPositions;
    public Transform barnAttackPosition;

    public static GameManager gameManager;

    private void Awake()
    {
        if (gameManager != null)
        {
            Destroy(gameManager);
        }

        gameManager = this;
    }
}
