using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IPathTarget
{
    public Transform TargetTransform => transform;
}
