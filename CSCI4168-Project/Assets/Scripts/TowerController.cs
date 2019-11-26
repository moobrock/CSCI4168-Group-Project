﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    public HealthController healthController;

    protected Transform attackPosition;
    protected GameObject towerModel;

    public static float attackRadius = 5f;
    public float health = 1f;
    public float maxHealth = 1f;

    protected void Start()
    {
        attackPosition = transform.GetChild(0);
        towerModel = transform.GetChild(1).gameObject;
    }
    
    public Transform GetAttackPosition() { return attackPosition; }

    public float GetHealth() { return health; }

    // true -> still alive (attack successful)
    // false -> dead (attack unsuccessful)
    public bool Attack(float damage)
    {
        if (health > 0f)
        {
            health = Mathf.Max(0f, health - damage);

            if (health <= 0f)
            {
                DestroyTower();
            }

            healthController.SetHealth(health / maxHealth);

            return true;
        }

        DestroyTower();

        return false;
    }

    protected void DestroyTower()
    {
        Debug.Log(name + " was destroyed");

        if (tag == "Barn")
            GameManager.gameManager.EndRound();

        Destroy(gameObject);
    }
}
