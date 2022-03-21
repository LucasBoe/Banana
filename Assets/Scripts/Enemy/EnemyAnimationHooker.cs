using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyAnimationHooker : MonoBehaviour
{
    [SerializeField] UnityEvent[] events;

    public void ExecuteEvent(int index)
    {
        events[index]?.Invoke();
    }
}
