using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class CameraSetUp : MonoBehaviourPun //MonoBehaviour��ɿ��� ����並 �߰��� ������Ƽ�̴�.
{//������ �ó׸ӽ��� ����ȭ �ϱ� ���� ��ũ��Ʈ ����

    void Start()
    {
        if (photonView.IsMine) //���� ��Ʈ��ũ���� ����䰡 �ڱ��ڽ��� ���̶�� == ���� �ڽ��� ���� �÷��̾��� (����Ʈ ������Ʈ�� �ߵ� x, ���� ������Ʈ���� �ߵ�)
        { 
            CinemachineVirtualCamera followCam = FindObjectOfType<CinemachineVirtualCamera>(); //��ü ������ �ó׸ӽ� ������Ʈ�� ã�´�. �ټ� ������. 
            followCam.Follow = transform; //����ī�޶��� ���� ����� �ڽ��� Ʈ���� ������ ����
            followCam.LookAt = transform; //����ī�޶� �ڱ��ڽ� ������Ʈ�� �ٶ󺸰� �Ѵ�.
        }
    }
}
