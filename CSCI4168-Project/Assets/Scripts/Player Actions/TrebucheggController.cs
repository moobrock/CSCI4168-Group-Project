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

    private SpriteRenderer radiusSpriteRenderer;

    private float alpha;
    private float lerp;
    private float diff = 0.05f;

    private IEnumerator FadeRadius()
    {
        while (true)
        {
            lerp += diff;

            if (lerp > 1)
            {
                lerp = 1f;
                diff = -0.05f;
            }

            if (lerp < 0)
            {
                lerp = 0f;
                diff = 0.05f;
            }

            alpha = Mathf.Lerp(0.1f, 0.5f, lerp);

            radiusSpriteRenderer.color = new Color(radiusSpriteRenderer.color.r,
                                                   radiusSpriteRenderer.color.g,
                                                   radiusSpriteRenderer.color.b,
                                                   alpha);

            yield return new WaitForEndOfFrame();
        }
    }

    private void Start()
    {
        shootDirection = -transform.up;

        radiusSpriteRenderer = gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>();

        //if (radiusSpriteRenderer != null)
        //{
            StartCoroutine(FadeRadius());
        //}

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
            if (targettedEnemy != null)
            {
                GameObject projectile = Instantiate(trebucheggPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity); // TODO: rotate projectile towards enemy
                projectile.transform.LookAt(transform.position + shootDirection);

                Quaternion euler = Quaternion.Euler(90, projectile.transform.rotation.eulerAngles.y, projectile.transform.rotation.eulerAngles.z);
                projectile.transform.rotation = euler;

                euler = Quaternion.Euler(-90, projectile.transform.rotation.eulerAngles.y, projectile.transform.rotation.eulerAngles.z);
                transform.rotation = euler;

                projectile.GetComponent<Rigidbody>()?.AddForce(shootDirection * force, ForceMode.Impulse);

                projectile.GetComponent<TrebucheggProjectile>()?.SetTarget(targettedEnemy.transform);
            }

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
