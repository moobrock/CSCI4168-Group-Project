using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Vector3 destination;

    private float moveDist = 1f;

    private float baseDamage = 0.1f;        // base damage done every attackFreqency seconds
    private float damageModifier = 0.05f;   // slight random modifier (+/-) to damage
    private float attackFrequency = 1f;     // in seconds
    private float attackRange = 1f;         // can only attack in this range

    private TowerController attackTarget;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        SetDestination();
    }

    private void Update()
    {
        // keep rotation
        transform.rotation = Quaternion.identity;

        DevelopmentControls();
    }

    // call when game is paused or unpaused
    public void OnPause(bool pause)
    {
    }

    private void SetDestination()
    {
        destination = GameManager.gameManager.barnAttackPosition.position;

        SetDestination(destination);
    }

    private void SetDestination(Vector3 position)
    {
        destination = position;
        navMeshAgent.SetDestination(position);
    }

    // manually control enemy for development
    private void DevelopmentControls()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            navMeshAgent.Move(new Vector3(0, 0, moveDist));
        }

        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            navMeshAgent.Move(new Vector3(0, 0, -moveDist));
        }

        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            navMeshAgent.Move(new Vector3(-moveDist, 0, 0));
        }

        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            navMeshAgent.Move(new Vector3(moveDist, 0, 0));
        }
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
        }
    }

    private IEnumerator ApproachTower()
    {
        while (attackTarget != null)
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
    }

    private IEnumerator AttackTower()
    {
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
}
