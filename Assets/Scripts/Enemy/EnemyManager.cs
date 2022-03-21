using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : SingletonBehaviour<EnemyManager>
{
    [SerializeField] Enemy current;

    public Enemy GetEnemy()
    {
        return current;
    }
}
