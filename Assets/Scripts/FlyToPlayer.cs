using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyToPlayer : MonoBehaviour
{
    [SerializeField] AnimationCurve relativeDistanceToSizeCurve;
    [SerializeField] MonoBehaviour enableWhenFinished;

    public void Fly(Transform target, System.Action callback)
    {
        StartCoroutine(MoveToPlayerRoutine(target, callback));
    }

    private IEnumerator MoveToPlayerRoutine(Transform target, Action callback)
    {
        float startDistance = Vector2.Distance(transform.position, target.position);
        float distance = float.MaxValue;

        while (distance > 0.1f)
        {
            distance = Vector2.Distance(transform.position, target.position);
            transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime);
            transform.localScale = relativeDistanceToSizeCurve.Evaluate(distance / startDistance) * Vector3.one;
            yield return null;
        }

        callback?.Invoke();
    }
}
