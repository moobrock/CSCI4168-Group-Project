using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrebucheggController : MonoBehaviour
{
    private float fireRate = 1.6f;
    private float force = 3f;

    public GameObject trebucheggPrefab;

    public Vector3 shootDirection;

    private void Start()
    {
        shootDirection = -transform.up;

        StartCoroutine(FireBullets());
    }

    // fires towards last seen enemy every fireRate seconds
    private IEnumerator FireBullets()
    {
        while (true)
        {
            Ray ray = new Ray(transform.position, transform.position + transform.forward);
            RaycastHit hit;
            Physics.SphereCast(ray, 5f, out hit);   // TODO: use layermask to only collide with enemies

            if (hit.transform?.tag == "Enemy")
            {
                shootDirection = hit.transform.position - transform.position;
                shootDirection.y = 0;
            }

            GameObject projectile = Instantiate(trebucheggPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity); // TODO: rotate projectile towards enemy
            projectile.transform.LookAt(transform.position + shootDirection);
            projectile.transform.rotation = Quaternion.Euler(90, projectile.transform.rotation.eulerAngles.y, projectile.transform.rotation.eulerAngles.z);
            projectile.GetComponent<Rigidbody>()?.AddForce(shootDirection * force, ForceMode.Impulse);

            yield return new WaitForSeconds(fireRate);
        }
    }
}
