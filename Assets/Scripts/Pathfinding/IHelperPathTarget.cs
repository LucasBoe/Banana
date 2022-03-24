using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHelperPathTarget
{    
    Transform TargetTransform { get; }
    Room Room { get; }
    bool IsAlive { get; }
}
