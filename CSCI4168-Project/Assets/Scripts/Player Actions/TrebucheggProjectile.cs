using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrebucheggProjectile : MonoBehaviour
{
    private float damage = 0.1f;
    private float speed = 0.05f;

    private float time;
    private float timeToLive = 10f;

    private Transform target;

    private void Start()
    {
        time = 0f;

        StartCoroutine(Destroy());
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    private void FixedUpdate()
    {
        // follow the targetted enemy, if there is one
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position + transform.up * 0.5f, speed);
        }

        // if the target enemy was lost or never existed, just continue forward
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.up * 0.5f, speed);
        }
    }

    /// <summary>
    /// Destroys the projectile timeToLive seconds after started.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Destroy()
    {
        while (true)
        {
            time += Time.deltaTime;

            if (time > timeToLive)
            {
                Destroy(gameObject);
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// Damage enemies when trigged.
    /// </summary>
    /// <param name="other">The other collider</param>
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trebuchegg Projectile damaging enemy for: " + damage);

        EnemyController enemy = other.transform?.GetComponent<EnemyController>();

        if (enemy != null)
        {
            enemy.Damage(damage);
            Destroy(gameObject);
        }
    }
}
