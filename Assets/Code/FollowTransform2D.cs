using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[DefaultExecutionOrder(-2)] // Should run before SpriteSkin.
public class FollowTransform2D : MonoBehaviour
{
    public Transform target;
    public bool followPosition = true;
    public bool followRotation = true;
    public float rotationOffset = 0f;
    public Vector2 positionOffset = Vector3.zero;

    // Should happen after animation, but before SpriteSkins.
    void LateUpdate()
    {
        var rotation = target.rotation;
        if (followPosition)
        {
            var position = transform.position;
            position.x = target.position.x;
            position.y = target.position.y;
            transform.position = position + rotation * positionOffset;
        }
        if (followRotation)
            transform.rotation = Quaternion.Euler(0f, 0f, rotationOffset) * rotation;
    }
}
