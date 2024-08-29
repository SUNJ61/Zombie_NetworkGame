using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Coin : MonoBehaviourPun,IItem
{
    public int score = 200; //코인을 먹었을 때 증가할 점수
    public void Use(GameObject target)
    {
        GameManager.Instance.AddScore(score); //스코어를 증가시킨다.
        //Destroy(gameObject); //닿는 순간 코인을 삭제, 싱글 게임용 코드
        PhotonNetwork.Destroy(gameObject); //모든 클라이언트에서 코인 삭제
    }
}
