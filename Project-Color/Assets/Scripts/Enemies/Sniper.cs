using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : EnemyType
{
    private void Awake()
    {
        target = GameObject.FindWithTag("PlayerRoot");
        type = Type.sniper;
    }
}
