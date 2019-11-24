using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreneggController : MonoBehaviour
{
    public GameObject particleEffect;

    public List<EnemyController> enemyControllers;

    private float damage = 0.5f;

    private void Start()
    {
        enemyControllers = new List<EnemyController>();

        StartCoroutine(Explode());
    }

    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(0.2f);

        // TODO: play particle effect

        foreach (EnemyController enemy in enemyControllers)
        {
            enemy?.Damage(damage);
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            EnemyController enemy = other.GetComponent<EnemyController>();

            enemyControllers.Add(enemy);
        }
    }
}
