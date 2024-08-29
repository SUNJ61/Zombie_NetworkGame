using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager G_Instance; //���к��� ��ü ������ ����, ���� ����ϴ� �͵� ������
    public bool isGameOver = false;
    void Awake()
    {
        if (G_Instance == null)
            G_Instance = this;
        else if(G_Instance != this)
            Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);
    }
    void Update()
    {
        
    }
}
