using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Copter : EnemyType
{
    void Awake()
    {
        target = GameObject.FindWithTag("PlayerRoot");
        type = Type.basic;
    }
}
