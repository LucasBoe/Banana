using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyCombatTarget
{
    Vector2 Position { get; }
    bool IsNull { get; }
}
