using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreneggController : MonoBehaviour
{
    public GameObject particleEffect;

    private List<EnemyController> enemyControllers;

    private float damage = 0.5f;

    private void Start()
    {
        enemyControllers = new List<EnemyController>();

        StartCoroutine(Explode());
    }

    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(0.5f);

        Ray ray = new Ray(transform.position, transform.position + transform.forward);
        Physics.SphereCast(ray, 5f);

        RaycastHit[] hits = Physics.SphereCastAll(ray, 5f, 5f);

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform?.tag == "Enemy")
            {
                EnemyController enemy = hit.transform.GetComponent<EnemyController>();

                enemyControllers.Add(enemy);
            }
        }

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
