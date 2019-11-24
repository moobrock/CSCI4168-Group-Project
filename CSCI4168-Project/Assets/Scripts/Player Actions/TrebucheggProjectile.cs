using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrebucheggProjectile : MonoBehaviour
{
    private float damage = 0.1f;

    private float time;
    private float timeToLive = 10f;

    private void Start()
    {
        time = 0f;

        StartCoroutine(Destroy());
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.up * 0.5f, 5f);
    }

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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trebuchegg Projectile damaging enemy for: " + damage);

        EnemyController enemy = other.transform?.GetComponent<EnemyController>();

        enemy?.Damage(damage);
    }
}
