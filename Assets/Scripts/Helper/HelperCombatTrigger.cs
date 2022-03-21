using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperCombatTrigger : MonoBehaviour
{
    public System.Action<Collider2D> TriggerEnter2D, TriggerExit2D;
    private void OnTriggerEnter2D(Collider2D collision) => TriggerEnter2D?.Invoke(collision);

    private void OnTriggerExit2D(Collider2D collision) => TriggerExit2D?.Invoke(collision);
}
