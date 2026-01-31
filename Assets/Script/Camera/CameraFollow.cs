using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform follow;
    private Vector3 offset;

    void Start()
    {
        offset = transform.position - follow.position;
    }

    void Update()
    {
        transform.position = offset + follow.position;
    }
}
