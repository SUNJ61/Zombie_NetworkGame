using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotSpeed = 60.0f;
    void Update()
    {
        transform.Rotate(0f, rotSpeed * Time.deltaTime, 0f); // rotSpeed만큼 y축 회전
    }
}
