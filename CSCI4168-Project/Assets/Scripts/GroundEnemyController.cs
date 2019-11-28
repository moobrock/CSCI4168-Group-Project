﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GroundEnemyController : MonoBehaviour, EnemyController
{
    public HealthController healthController;

    public GameObject coinPrefab;
    public int baseCoinDrop = 2;
    public int coinDropVariance = 1;

    private NavMeshAgent navMeshAgent;
    private Vector3 destination;

    public float baseSpeed = 3f;

    public float baseDamage = 0.3f;        // base damage done every attackFreqency seconds
    public float damageModifier = 0.05f;   // slight random modifier (+/-) to damage
    public float attackFrequency = 1f;     // in seconds
    public float attackRange = 1f;         // can only attack in this range

    public float health = 1f;
    public float maxHealth = 1f;

    private TowerController attackTarget;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = baseSpeed;

        Physics.IgnoreLayerCollision(8, 8);

        SetDestination();
    }

    public Transform GetTransform()
    {
        try
        {
            return transform;
        }

        catch (MissingReferenceException e)
        {
            return null;
        }
    }

    private void SetDestination()
    {
        if (GameManager.gameManager.barnAttackPosition == null)
            GameManager.gameManager.FindBarn();

        destination = GameManager.gameManager.barnAttackPosition.position;

        SetDestination(destination);
    }

    private void SetDestination(Vector3 position)
    {
        destination = position;
        navMeshAgent.SetDestination(position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Tower" || other.tag == "Barn")
        {
            // get tower attack position
            TowerController tower = other.GetComponent<TowerController>();

            if (tower != null)
            {
                Transform attackPosition = tower.GetAttackPosition();

                // TODO: use navmesh agent to check path distance (to make sure the tower isn't on another nearby path)
                float pathDistance = TowerController.attackRadius;

                // go to attack position if on the same path (navmesh agent path distance is short)
                if (pathDistance <= TowerController.attackRadius * 2f)
                {
                    SetDestination(attackPosition.position);

                    attackTarget = tower;

                    StartCoroutine(ApproachTower());
                }
            }

            else
            {
                attackTarget = null;
                SetDestination();
            }
        }
    }

    private IEnumerator ApproachTower()
    {
        while (attackTarget != null && attackTarget.GetAttackPosition() != null)
        {
            float distance = (transform.position - attackTarget.GetAttackPosition().position).magnitude;

            // in attack range of tower
            // start attacking and exit coroutine
            if (distance <= attackRange)
            {
                StartCoroutine(AttackTower());

                yield break;
            }

            yield return new WaitForEndOfFrame();
        }

        SetDestination();
    }

    private IEnumerator AttackTower()
    {
        while (attackTarget != null && attackTarget.GetHealth() > 0f)
        {
            transform.LookAt(attackTarget.transform);

            float damage = baseDamage + Random.Range(-damageModifier, damageModifier);

            float damageDone = attackTarget.Attack(damage);

            // enemy is or was dead
            // stop attacking and exit coroutine
            if (damageDone <= 0f)
            {
                attackTarget = null;

                yield break;
            }

            yield return new WaitForSecondsRealtime(attackFrequency);
        }

        SetDestination();
    }

    public void ModifySpeed(float speedModifier)
    {
        navMeshAgent.speed = baseSpeed * speedModifier;
    }

    public void ResetSpeed()
    {
        if (navMeshAgent != null)
            navMeshAgent.speed = baseSpeed;
    }

    public void Damage(float damage)
    {
        health -= damage;

        if (health <= 0f)
        {
            // kill enemy
            GameManager.gameManager.LogKill();

            // drop coins
            int num = baseCoinDrop + (int)Random.Range(baseCoinDrop - coinDropVariance, baseCoinDrop + coinDropVariance);

            for (int i = 0; i < num; i++)
            {
                Instantiate(coinPrefab, transform.position, Quaternion.Euler(90, 0, 0));
            }

            Destroy(this.gameObject);
        }

        healthController.SetHealth(health / maxHealth);
    }
}
