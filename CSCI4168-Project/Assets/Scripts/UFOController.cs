using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOController : MonoBehaviour, EnemyController
{
    public HealthController healthController;

    public GameObject coinPrefab;
    public int baseCoinDrop = 5;
    public int coinDropVariance = 2;

    private Rect boundaryRect;

    private Vector3 targetPosition;

    private float baseDamage = 0.3f;        // base damage done every attackFreqency seconds
    private float damageModifier = 0.05f;   // slight random modifier (+/-) to damage
    private float attackFrequency = 1f;     // in seconds
    private float attackRange = 1f;         // can only attack in this range

    private float health = 1f;
    private float maxHealth = 1f;

    private TowerController attackTarget;

    private float speed = 0.05f;

    void Start()
    {
        targetPosition = transform.position;

        RectTransform rt = GameObject.Find("Level Base")?.transform.Find("UI")?.Find("Camera Boundary")?.GetComponent<RectTransform>();

        if (rt != null)
        {
            Vector3[] v = new Vector3[4];
            rt.GetWorldCorners(v);

            boundaryRect = Rect.MinMaxRect(v[0].x, v[0].z, v[2].x, v[2].z);

            FindTargetPosition();
        }

        else
        {
            Destroy(this.gameObject);
        }
    }

    void Update()
    {
        if ((targetPosition - transform.position).magnitude < 0.1f)
        {
            FindTargetPosition();
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed);
        transform.LookAt(targetPosition);
    }

    private void FindTargetPosition()
    {
        // find new target position
        if (boundaryRect != null)
        {
            float x = Random.Range(boundaryRect.xMin, boundaryRect.xMax);
            float z = Random.Range(boundaryRect.yMin, boundaryRect.yMax);
            targetPosition = new Vector3(x, 0, z);

            Debug.Log("Target position: " + targetPosition);
        }
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
                    targetPosition= attackPosition.position;

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
        Debug.Log("Attack target " + attackTarget.name + ", health: " + attackTarget.GetHealth());

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
    }

    public void Damage(float damage)
    {
        health -= damage;

        Debug.Log("Enemy damaged for " + damage + " points. Health = " + health);

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

    //returns child transform (UFO model) or this transform if not found
    public Transform GetTransform()
    {
        return transform?.GetChild(0)?.transform ?? transform;
    }
}
