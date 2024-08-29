using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PlayerMovement : MonoBehaviourPun
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 180f;
    private PlayerInput playerInput;
    private Rigidbody playerRigidbody;
    private Animator playeranimator;
    private readonly int hashmove = Animator.StringToHash("Move");
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody>();
        playeranimator = GetComponent<Animator>();
    }
    void FixedUpdate()
    {
        if (!photonView.IsMine) return; //포톤뷰가 자신의 것이 아니라면 == 로컬 오브젝트가 아니라면 함수를 리턴한다. (리모트 오브젝트는 스크립트 기능 비활성화와 같음.)

        Rotate();
        Move();

        playeranimator.SetFloat(hashmove, playerInput.move);
    }
    private void Move()
    {
        Vector3 moveDistance =
            playerInput.move * transform.forward * moveSpeed * Time.deltaTime;
        //리지디 바디를 이용해서 게임오브젝트 위치 변경
        playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);
                       //리지디바디에서 지원한다.
    }
    private void Rotate()
    {
        float turn = playerInput.rotate * rotateSpeed * Time.deltaTime;
        playerRigidbody.rotation = playerRigidbody.rotation * Quaternion.Euler(0f, turn, 0f);
        
    }
}
