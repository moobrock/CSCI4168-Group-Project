using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrateController : TowerController
{
    public Sprite fullHealth;
    public Sprite halfHealth;
    public Sprite quarterHealth;

    public SpriteRenderer sprite;

    protected override void SetHealth(float health)
    {
        if (health > (maxHealth / 1.25f))
        {
            sprite.sprite = fullHealth;
        }

        else if (health > (maxHealth / 4f))
        {
            sprite.sprite = halfHealth;
        }

        else
        {
            sprite.sprite = quarterHealth;
        }

        base.SetHealth(health);
    }
}
