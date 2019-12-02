using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrebucheggController : MonoBehaviour
{
    public HealthController healthController;

    public int numAvailable = 3;

    private float fireRate = 1.6f;
    private float force = 3f;

    public GameObject trebucheggPrefab;
    public Animator animator;

    public EnemyController targettedEnemy;
    public Vector3 shootDirection;

    private SpriteRenderer radiusSpriteRenderer;

    private float alpha;
    private float lerp;
    private float diff = 0.05f;

    private float startTime;
    public float lifeTime = 1 * 60f;

    public float updateTime;
    public float updateFrequency = 3f;

    public float radius = 5f;

    private void Start()
    {
        startTime = Time.realtimeSinceStartup;
        updateTime = 0f;

        shootDirection = -transform.up;

        radiusSpriteRenderer = gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>();

        if (radiusSpriteRenderer != null)
        {
            StartCoroutine(FadeRadius());
        }

        StartCoroutine(FireBullets());
    }

    private void FixedUpdate()
    {
        float time = Time.realtimeSinceStartup - startTime;
        healthController.SetHealth(1f - time / lifeTime);

        if (time > lifeTime)
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }

        updateTime += Time.fixedDeltaTime;

        try
        {
            if (targettedEnemy != null && targettedEnemy?.GetTransform() != null)
                shootDirection = targettedEnemy.GetTransform().position - transform.position;
            else
            {
                if (updateTime > updateFrequency)
                {
                    updateTime = 0f;

                    Ray ray = new Ray(transform.position, transform.position + transform.forward);

                    RaycastHit[] hits = Physics.SphereCastAll(ray, radius, radius);

                    foreach (RaycastHit hit in hits)
                    {
                        if (hit.transform?.tag == "Enemy")
                        {
                            targettedEnemy = hit.transform.GetComponent<EnemyController>() ?? hit.transform.GetComponentInChildren<EnemyController>();
                            break;
                        }
                    }
                }
            }
        }
        catch (MissingReferenceException e)
        {
            Debug.Log("Targetted enemy lost. Error: " + e.StackTrace); 

            shootDirection = -transform.forward;
        }
    }

    private IEnumerator FadeRadius()
    {
        while (true)
        {
            lerp += diff;

            if (lerp > 1)
            {
                lerp = 1f;
                diff = -0.03f;
            }

            if (lerp < 0)
            {
                lerp = 0f;
                diff = 0.03f;
            }

            alpha = Mathf.Lerp(0.1f, 0.4f, lerp);

            radiusSpriteRenderer.color = new Color(radiusSpriteRenderer.color.r,
                                                   radiusSpriteRenderer.color.g,
                                                   radiusSpriteRenderer.color.b,
                                                   alpha);

            yield return new WaitForEndOfFrame();
        }
    }

    // fires towards last seen enemy every fireRate seconds
    private IEnumerator FireBullets()
    {
        while (true)
        {
            if (targettedEnemy != null && targettedEnemy.GetTransform() != null)
            {
                GameObject projectile = Instantiate(trebucheggPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity); // TODO: rotate projectile towards enemy
                projectile.transform.LookAt(transform.position + shootDirection);

                Quaternion euler = Quaternion.Euler(90, projectile.transform.rotation.eulerAngles.y, projectile.transform.rotation.eulerAngles.z);
                projectile.transform.rotation = euler;

                projectile.SetActive(false);                // hide bullet until ready to fire

                animator.SetTrigger("shoot");

                // wait for animation to end
                float waitTime = 1.2f, time = 0f;

                while (time < waitTime)
                {
                    time += Time.deltaTime;

                    // look at enemy
                    transform.LookAt(targettedEnemy?.GetTransform()?.position ?? shootDirection);
                    euler = Quaternion.Euler(-90, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
                    transform.rotation = euler;

                    yield return new WaitForEndOfFrame();
                }

                if (targettedEnemy != null)
                {

                    projectile.SetActive(true);

                    projectile.GetComponent<Rigidbody>()?.AddForce(shootDirection * force, ForceMode.Impulse);
                    projectile.GetComponent<TrebucheggProjectile>()?.SetTarget(targettedEnemy.GetTransform());
                }

                else
                {
                    Destroy(projectile);
                }
            }

            yield return new WaitForSeconds(fireRate);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && targettedEnemy?.GetTransform() == null)
        {
            targettedEnemy = other.GetComponent<EnemyController>() ?? other.GetComponentInChildren<EnemyController>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Enemy" && targettedEnemy?.GetTransform() == null)
        {
            targettedEnemy = other.GetComponent<EnemyController>() ?? other.GetComponentInChildren<EnemyController>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy" && targettedEnemy?.GetTransform() != null && targettedEnemy == (other.GetComponent<EnemyController>() ?? other.GetComponentInChildren<EnemyController>()))
        {
            targettedEnemy = null;
        }
    }
}
