using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperAnimator : MonoBehaviour
{
    [SerializeField] Helper helper;
    [SerializeField] HelperStateAnimatorStateNamePairs[] pairs;
    [SerializeField] Animator animator;

    private void OnEnable()
    {
        helper.ChangedState += OnChangedState;
    }

    private void OnDisable()
    {
        helper.ChangedState -= OnChangedState;
    }
    private void OnChangedState(HelperState state)
    {
        Debug.Log("Changed State to " + state);

        foreach (var pair in pairs)
        {
            if (pair.State == state)
                animator.Play(pair.AnimatorStateName);
        }
    }
}

[System.Serializable]
public class HelperStateAnimatorStateNamePairs
{
    public HelperState State;
    public string AnimatorStateName;
}
