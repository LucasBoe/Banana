using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth
{
    System.Action<float> ChangedHealth { get; set; }
}
