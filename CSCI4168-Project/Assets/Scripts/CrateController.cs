using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrateController : TowerController
{
    public Sprite fullHealthSprite;
    public Sprite halfHealthSprite;
    public Sprite quarterHealthSprite;

    private bool halfHealth;
    private bool quarterHealth;

    public SpriteRenderer sprite;

    private AudioSource audio;

    protected override void Start()
    {
        base.Start();
        audio = GetComponent<AudioSource>();
    }

    protected override void SetHealth(float health)
    {
        if (health > (maxHealth / 1.25f))
        {
            sprite.sprite = fullHealthSprite;
        }

        else if (health > (maxHealth / 4f) && !halfHealth)
        {
            halfHealth = true;
            sprite.sprite = halfHealthSprite;
            audio?.Play();
        }

        else if (sprite.sprite != quarterHealth && !quarterHealth)
        {
            quarterHealth = true;
            sprite.sprite = quarterHealthSprite;
            sprite.transform.position = new Vector3(sprite.transform.position.x, 0, sprite.transform.position.z);
            audio?.Play();
        }

        base.SetHealth(health);
    }
}
