using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerPotController : MonoBehaviour
{
    private float health = 2.5f;

    private float speedModifier = 0.4f;
    private float damage = 0.001f;      // damage done to enemies and to self

    private float timeToLive = 25f;     // maximum lifetime of the object, if not destroyed

    private List<GroundEnemyController> affectedEnemies;

    private void Start()
    {
        affectedEnemies = new List<GroundEnemyController>();

        StartCoroutine(CountDown());
    }

    private IEnumerator CountDown()
    {
        while (timeToLive > 0f)
        {
            timeToLive -= Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        foreach (EnemyController enemy in affectedEnemies)
        {
            (enemy as GroundEnemyController)?.ResetSpeed();
        }

        Destroy(this.gameObject);
    }

    public void Damage(float damage)
    {
        health -= damage;

        if (health <= 0f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Debug.Log("Enemy entered broken flower pot");

            GroundEnemyController enemy = other.GetComponent<GroundEnemyController>();
            if (enemy != null)
            {
                affectedEnemies.Add(enemy);
                enemy.ModifySpeed(speedModifier);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Enemy")
        {
            other.GetComponent<GroundEnemyController>()?.Damage(damage);

            Damage(damage);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Debug.Log("Enemy exited broken flower pot");

            GroundEnemyController enemy = other.GetComponent<GroundEnemyController>();
            if (enemy != null)
            {
                affectedEnemies.Remove(enemy);
                enemy.ResetSpeed();
            }
        }
    }
}
