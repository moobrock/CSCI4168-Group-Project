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

    private float attackRange = 1f;         // can only attack if in this range

    private float health = 1f;
    private float maxHealth = 1f;

    private BarnController attackTarget;

    private float speed = 0.05f;

    private int cowsAbducted;

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
        if (cowsAbducted == 0)
        {
            if ((targetPosition - transform.position).magnitude < 0.1f)
            {
                FindTargetPosition();
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed);
            transform.LookAt(targetPosition);
            healthController.transform.rotation = Quaternion.Euler(90, 0, 0);
        }
    }

    private void FindTargetPosition()
    {
        // find new target position
        if (boundaryRect != null)
        {
            float x = Random.Range(boundaryRect.xMin, boundaryRect.xMax);
            float z = Random.Range(boundaryRect.yMin, boundaryRect.yMax);
            targetPosition = new Vector3(x, 0, z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Barn" && cowsAbducted == 0)
        {
            // get tower attack position
            BarnController barn = other.GetComponent<BarnController>();

            if (barn != null)
            {
                Transform attackPosition = barn.cowPen ?? barn.GetAttackPosition();

                if (attackPosition != null)
                {
                    targetPosition = attackPosition.position;

                    attackTarget = barn;

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
            float distance = (transform.position - attackTarget.cowPen.position).magnitude;

            // in attack range of tower
            // start attacking and exit coroutine
            if (distance <= attackRange)
            {
                AbductCow();

                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private void AbductCow()
    {
        float damageDone = attackTarget.AbductCow();

        Debug.Log("Abducting cow " + attackTarget.tag + " for " + damageDone + " damage. Tower health = " + attackTarget.GetHealth());

        if (damageDone > 0f)
        {
            cowsAbducted++;

            StopAllCoroutines();
            StartCoroutine(GoToSpawn());
        }
    }

    private IEnumerator GoToSpawn()
    {
        Vector3 spawn = GameManager.gameManager.GetNearestSpawn(transform.position).position;
        Vector3 position;

        while ((spawn - transform.position).sqrMagnitude > 9f)
        {
            position = Vector3.MoveTowards(transform.position, spawn, speed);
            position.y = 0;
            transform.position = position;
            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
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

            // return cows if any were abducted
            if (cowsAbducted > 0)
            {
                if (attackTarget == null)
                {
                    attackTarget = GameManager.gameManager.GetBarnController();
                }

                if (attackTarget == null)
                    Debug.LogWarning("Cant return cows");

                attackTarget?.ReturnCows(cowsAbducted);
            }

            Destroy(this.gameObject);
        }

        healthController.SetHealth(health / maxHealth);
    }

    //returns child transform (UFO model) or this transform if not found
    public Transform GetTransform()
    {
        try
        {
            return transform?.GetChild(0)?.transform ?? transform;
        }

        catch (MissingReferenceException e)
        {
            return null;
        }
    }
}
