using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour, IItem
{
    public float health = 50f;
    public void Use(GameObject target)
    {
        Debug.Log("ü���� ȸ�� �ߴ� :" + health);
    }
}
