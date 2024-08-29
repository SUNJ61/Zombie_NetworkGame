using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
// 입력과 움직임과 분리해서 스크립트를 만든다. 
//입력과 액터 나누기 
public class PlayerInput : MonoBehaviourPun
{
    public string moveAxisName = "Vertical";
    public string rotateAxisName = "Horizontal";
    public string fireButtonName = "Fire1";
    public string reloadButtonName = "Reload";
    // 키관련  프로퍼티 만들기 
    public float move {  get; private set; }
    public float rotate { get; private set; }
    public bool fire { get; private set; }
    public bool reload { get; private set; }

    void Start()
    {
        
    }
    void Update()
    {
        if (!photonView.IsMine) return; //포톤뷰가 자신의 것이 아니라면 == 로컬 오브젝트가 아니라면 함수를 리턴한다. (리모트 오브젝트는 스크립트 기능 비활성화와 같음.)

        if(GameManager.Instance!=null&& GameManager.Instance.isGameOver)
        {
            move = 0f; rotate = 0f; fire = false; reload = false;
            return;
        }

        move =Input.GetAxis(moveAxisName);
        rotate =Input.GetAxis(rotateAxisName);
        fire =Input.GetButton(fireButtonName);
        reload =Input.GetButtonDown(reloadButtonName);
    }
}
