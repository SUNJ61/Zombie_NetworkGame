using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
��Ʈ��ũ ������ �÷��̾ ������ �� �� �̹Ƿ� �������� �ٸ� �� �ִ�.
������ �÷��̾ �Է��ϴ� ��ũ��Ʈ�� �׿� ���� �̵��ϴ� ��ũ��Ʈ�� ���� �����. ('�Է°� ���� ������' ��� �Ѵ�.)
�÷����� �ٲ���� ��ĥ �κ��� ������.
*/
public class PlayerInput : MonoBehaviour
{
    public string moveAxisName = "Vertical";
    public string rotateAxisName = "Horizontal";
    public string fireButtonName = "Fire1";
    public string reloadButtonName = "Reload";
    //Ű ���� �Ķ���Ϳ� �����ϴ� ������Ƽ�� �����.
    public float move { get; private set; } //������ ��𿡼��� �� �� ������, ������ �� Ŭ���������� ����.
    public float rotate { get; private set; } //������ ��𿡼��� �� �� ������, ������ �� Ŭ���������� ����.
    public bool fire { get; private set; } //������ ��𿡼��� �� �� ������, ������ �� Ŭ���������� ����.
    public bool reload { get; private set; } //������ ��𿡼��� �� �� ������, ������ �� Ŭ���������� ����.
    void Start()
    {
        
    }
    void Update()
    {
        if(GameManager.G_Instance != null && GameManager.G_Instance.isGameOver)
        { //���ӸŴ����� �ν��Ͻ��� �����ϰ� ���� ���� ���°� �Ǿ��ٸ�
            move = 0f;
            rotate = 0f;
            fire = false;
            reload = false;
            return;
        }
        if (Input.GetKey(KeyCode.LeftShift)) //if�� ���� �߰��Ѱ�
            move = Input.GetAxis(moveAxisName); // float���� ��� ������Ʈ
        else
            move = Input.GetAxis(moveAxisName) * 0.5f;

        rotate = Input.GetAxis(rotateAxisName);
        fire = Input.GetButton(fireButtonName); //��ư�� ������ ���� true �ƴϸ� false�� ��ȯ
        reload = Input.GetButtonDown(reloadButtonName); //��ư�� ���� �������� true �ƴϸ� false�� ��ȯ
    }
}
