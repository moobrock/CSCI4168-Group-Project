using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrebucheggController : MonoBehaviour
{
    private float fireRate = 1.6f;
    private float force = 3f;

    public GameObject trebucheggPrefab;

    public EnemyController targettedEnemy;
    public Vector3 shootDirection;

    private void Start()
    {
        shootDirection = -transform.up;

        StartCoroutine(FireBullets());
    }

    private void FixedUpdate()
    {
        shootDirection = (targettedEnemy != null) ? -(transform.position - targettedEnemy.transform.position) : -transform.up;
    }

    // fires towards last seen enemy every fireRate seconds
    private IEnumerator FireBullets()
    {
        while (true)
        {
            GameObject projectile = Instantiate(trebucheggPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity); // TODO: rotate projectile towards enemy
            projectile.transform.LookAt(transform.position + shootDirection);
            projectile.transform.rotation = Quaternion.Euler(90, projectile.transform.rotation.eulerAngles.y, projectile.transform.rotation.eulerAngles.z);
            projectile.GetComponent<Rigidbody>()?.AddForce(shootDirection * force, ForceMode.Impulse);

            yield return new WaitForSeconds(fireRate);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && targettedEnemy == null)
        {
            targettedEnemy = other.GetComponent<EnemyController>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Enemy" && targettedEnemy == null)
        {
            targettedEnemy = other.GetComponent<EnemyController>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy" && targettedEnemy != null && targettedEnemy == other.GetComponent<EnemyController>())
        {
            targettedEnemy = null;
        }
    }
}
