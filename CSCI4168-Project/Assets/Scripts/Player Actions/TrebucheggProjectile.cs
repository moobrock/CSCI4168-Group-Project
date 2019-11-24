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

    private void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.otherCollider.tag == "Enemy")
            {
                EnemyController enemy = contact.otherCollider.GetComponent<EnemyController>();

                enemy.Damage(damage);
            }
        }
    }
}
