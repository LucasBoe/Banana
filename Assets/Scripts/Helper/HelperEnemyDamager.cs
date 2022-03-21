using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperEnemyDamager : MonoBehaviour, IDamager
{
    [SerializeField] float damageAmount;
    public bool IsEnabled => isEnabled;
    private bool isEnabled = false;
    private void OnEnable()
    {
        isEnabled = true;
    }

    public float Amount => damageAmount;

    public void Disable()
    {
        isEnabled = false;
    }
}
