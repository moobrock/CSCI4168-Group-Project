using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GroundEnemyController : MonoBehaviour, EnemyController
{
    private NavMeshAgent navMeshAgent;
    private Vector3 destination;

    private float baseSpeed = 3f;

    private float baseDamage = 0.3f;        // base damage done every attackFreqency seconds
    private float damageModifier = 0.05f;   // slight random modifier (+/-) to damage
    private float attackFrequency = 1f;     // in seconds
    private float attackRange = 1f;         // can only attack in this range

    private float health = 1f;

    private TowerController attackTarget;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = baseSpeed;

        SetDestination();
    }

    public Transform GetTransform()
    {
        return transform;
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
        Debug.Log("Trigger enter - " + other.name);

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
            }
        }
    }

    private IEnumerator ApproachTower()
    {
        while (attackTarget != null)
        {
            Debug.Log("Approaching");

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
    }

    private IEnumerator AttackTower()
    {
        Debug.Log("Attack target " + attackTarget.name + ", health: " + attackTarget.GetHealth() );

        while (attackTarget != null && attackTarget.GetHealth() > 0f)
        {
            float damage = baseDamage + Random.Range(-damageModifier, damageModifier);

            bool success = attackTarget.Attack(damage);

            Debug.Log("Attacking " + attackTarget.tag + " for " + damage + " damage. Tower health = " + attackTarget.GetHealth());

            // enemy is or was dead
            // stop attacking and exit coroutine
            if (!success)
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

        Debug.Log("Enemy damaged for " + damage + " points. Health = " + health);

        if (health <= 0f)
        {
            // kill enemy
            GameManager.gameManager.LogKill();

            Destroy(this.gameObject);
        }
    }
}
