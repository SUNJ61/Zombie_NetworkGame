using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class CameraSetUp : MonoBehaviourPun //MonoBehaviour기능에서 포톤뷰를 추가한 프로퍼티이다.
{//포톤뷰와 시네머신을 동기화 하기 위해 스크립트 제작

    void Start()
    {
        if (photonView.IsMine) //포톤 네트워크상의 포톤뷰가 자기자신의 것이라면 == 만약 자신이 로컬 플레이어라면 (리모트 오브젝트는 발동 x, 로컬 오브젝트에만 발동)
        { 
            CinemachineVirtualCamera followCam = FindObjectOfType<CinemachineVirtualCamera>(); //전체 씬에서 시네머신 오브젝트를 찾는다. 다소 느리다. 
            followCam.Follow = transform; //가상카메라의 추적 대상을 자신의 트랜스 폼으로 변경
            followCam.LookAt = transform; //가상카메라를 자기자신 오브젝트를 바라보게 한다.
        }
    }
}
