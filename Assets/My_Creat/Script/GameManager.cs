using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager G_Instance; //무분별한 객체 생성을 막음, 많이 사용하는 것도 안좋음
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
