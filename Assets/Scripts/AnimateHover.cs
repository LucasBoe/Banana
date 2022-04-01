using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateHover : MonoBehaviour
{
    [SerializeField] float min, max;
    [SerializeField] float speed;

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = transform.position;
        float lerp = ((Mathf.Sin(Time.time * speed) + 1f) / 2f);
        transform.position = new Vector3(pos.x, pos.y, Mathf.Lerp(min, max, lerp));
    }
}
