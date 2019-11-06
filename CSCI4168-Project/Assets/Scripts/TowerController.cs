using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    private Transform attackPosition;
    private GameObject towerModel;

    public static float attackRadius = 5f;
    private static float health = 1f;

    void Start()
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

            return true;
        }

        DestroyTower();

        return false;
    }

    private void DestroyTower()
    {
        towerModel.SetActive(false);
    }
}
