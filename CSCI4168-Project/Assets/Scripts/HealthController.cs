using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    public Slider healthbar;

    private Quaternion rotation;
    private Vector3 offset;

    private float health = 1f;

    public Color green;
    public Color yellow;
    public Color red;

    private void Start()
    {
        offset = transform.localPosition;
        rotation = transform.rotation;
    }

    private void LateUpdate()
    {
        transform.position = transform.parent?.position + offset ?? transform.position;
        transform.rotation = rotation;
    }

    public void SetHealth(float health)
    {
        this.health = health;
        healthbar.value = health;

        if (health < 0.5f)
        {
            healthbar.image.color = Color.Lerp(red, yellow, health * 2f);
        }

        else
        {
            healthbar.image.color = Color.Lerp(yellow, green, (health - 0.5f) / 2f);
        }
    }
}
