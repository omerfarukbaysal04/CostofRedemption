using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public float moveSpeed = 3f;

    void Update()
    {
        transform.position += Vector3.right * moveSpeed * Time.deltaTime;
    }
}
