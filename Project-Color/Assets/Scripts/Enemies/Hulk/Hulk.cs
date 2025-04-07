using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hulk : EnemyType
{
    private void Awake()
    {
        target = GameObject.FindWithTag("PlayerRoot");
        type = Type.hulk;
    }
}
