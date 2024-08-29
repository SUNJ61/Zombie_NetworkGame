using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
네트워크 게임은 플레이어가 여러개 일 수 이므로 움직임이 다를 수 있다.
때문에 플레이어에 입력하는 스크립트와 그에 따라 이동하는 스크립트를 따로 만든다. ('입력과 액터 나누기' 라고 한다.)
플랫폼이 바뀌더라도 고칠 부분이 적어짐.
*/
public class PlayerInput : MonoBehaviour
{
    public string moveAxisName = "Vertical";
    public string rotateAxisName = "Horizontal";
    public string fireButtonName = "Fire1";
    public string reloadButtonName = "Reload";
    //키 관련 파라미터에 접근하는 프로퍼티를 만든다.
    public float move { get; private set; } //참조는 어디에서든 할 수 있지만, 설정은 이 클래스에서만 가능.
    public float rotate { get; private set; } //참조는 어디에서든 할 수 있지만, 설정은 이 클래스에서만 가능.
    public bool fire { get; private set; } //참조는 어디에서든 할 수 있지만, 설정은 이 클래스에서만 가능.
    public bool reload { get; private set; } //참조는 어디에서든 할 수 있지만, 설정은 이 클래스에서만 가능.
    void Start()
    {
        
    }
    void Update()
    {
        if(GameManager.G_Instance != null && GameManager.G_Instance.isGameOver)
        { //게임매니저의 인스턴스가 존재하고 게임 오버 상태가 되었다면
            move = 0f;
            rotate = 0f;
            fire = false;
            reload = false;
            return;
        }
        if (Input.GetKey(KeyCode.LeftShift)) //if문 내가 추가한것
            move = Input.GetAxis(moveAxisName); // float값을 계속 업데이트
        else
            move = Input.GetAxis(moveAxisName) * 0.5f;

        rotate = Input.GetAxis(rotateAxisName);
        fire = Input.GetButton(fireButtonName); //버튼을 누르는 동안 true 아니면 false를 반환
        reload = Input.GetButtonDown(reloadButtonName); //버튼을 누른 순간에만 true 아니면 false를 반환
    }
}
