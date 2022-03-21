using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Enemy enemy;
    [SerializeField] EnemyStateAnimatorStateNamePairs[] pairs;



    private void OnEnable()
    {
        enemy.SetEnemyState += OnChangedState;
    }

    private void OnDisable()
    {
        enemy.SetEnemyState -= OnChangedState;
    }
    private void OnChangedState(EnemyState state, IEnemyCombatTarget target)
    {
        foreach (var pair in pairs)
        {
            if (pair.State == state)
                animator.Play(pair.AnimatorStateName);
        }
    }
}

[System.Serializable]
public class EnemyStateAnimatorStateNamePairs
{
    public EnemyState State;
    public string AnimatorStateName;
}
