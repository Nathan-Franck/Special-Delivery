using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SubpixelMovement : MonoBehaviour
{
    public GameObject target;

    // Move the camera angle by sin waves that jiggle the view in sub-pixel increments
    // This is to test the "staircase" effect of pixelated lines
    public float Speed = 1;
    public float Amplitude = 1;

    void Update()
    {
        var rotation = transform.rotation.eulerAngles;
        rotation.x += Mathf.Sin(Time.time * Speed * Mathf.PI / 3) * Amplitude * 0.5f;
        rotation.y += Mathf.Sin(Time.time * Speed) * Amplitude;
        transform.rotation = Quaternion.Euler(rotation);

    }

}
