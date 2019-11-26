using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface EnemyController 
{
    Transform GetTransform();
    void Damage(float damage);
}
