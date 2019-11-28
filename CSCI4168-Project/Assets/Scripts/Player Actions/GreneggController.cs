using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreneggController : MonoBehaviour
{
    public GameObject particleEffect;

    private List<EnemyController> enemyControllers;

    private float damage = 0.5f;
    private float damageRadius = 3f;
    private float timeToExplosion = 3f;     // time in seconds before exploding
    private float time = 0f;

    private float alpha;
    private float lerp;
    private float diff = 0.1f;

    private SpriteRenderer targetSpriteRenderer;
    private AudioSource audio;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
        enemyControllers = new List<EnemyController>();

        targetSpriteRenderer = transform.Find("Target")?.GetComponent<SpriteRenderer>();
        targetSpriteRenderer.transform.localScale = new Vector2(damageRadius * 0.6f, damageRadius * 0.6f);    // approximation of radius 

        StartCoroutine(Explode());
    }

    private IEnumerator Explode()
    {
        while (time < timeToExplosion)
        {
            lerp += diff;

            if (lerp > 1)
            {
                lerp = 1f;
                diff = -0.1f;
            }

            if (lerp < 0)
            {
                lerp = 0f;
                diff = 0.1f;
            }

            alpha = Mathf.Lerp(0.25f, 1f, lerp);


            targetSpriteRenderer.color = new Color(targetSpriteRenderer.color.r,
                                                   targetSpriteRenderer.color.g,
                                                   targetSpriteRenderer.color.b,
                                                   alpha);

            time += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        // times up - explode 

        Ray ray = new Ray(transform.position, transform.position + transform.forward);
        Physics.SphereCast(ray, 5f);

        RaycastHit[] hits = Physics.SphereCastAll(ray, damageRadius, damageRadius);

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform?.tag == "Enemy")
            {
                EnemyController enemy = hit.transform.GetComponent<EnemyController>();

                enemyControllers.Add(enemy);
            }
        }

        foreach (EnemyController enemy in enemyControllers)
        {
            enemy?.Damage(damage);
        }

        // TODO: play particle effect

        audio?.Play();

        while (audio.isPlaying) yield return new WaitForEndOfFrame(); // wait for sound to stop before destroying

        Destroy(gameObject);
    }
}
