using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInput playerInput;
    private Rigidbody playerRb;
    private Animator playerAnimator;

    private readonly int hashmove = Animator.StringToHash("Move");

    public float moveSpeed = 5.0f;
    public float rotateSpeed = 180f;
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerRb = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
    }
    void FixedUpdate()
    {
        Move();
        Rotate();
        playerAnimator.SetFloat(hashmove, playerInput.move);
    }
    private void Move()
    {
        //FixedUpdate함수에서는 Time.fixedDeltaTime을 적어야하는데 Time.deltaTime을 적어도
        //유니티에서 자동으로 Time.fixedDeltaTime로 변경해 준다.
        Vector3 moveDistance = playerInput.move * transform.forward * moveSpeed * Time.deltaTime;
        playerRb.MovePosition(playerRb.position + moveDistance);
        //리깃바디에 내장된 자동으로 해당 백터 방향으로 이동해주는 함수, playerRb의 포지션에서 해당 벡터만큼 이동
    }
    private void Rotate()
    {
        float turn = playerInput.rotate * rotateSpeed * Time.deltaTime;
        playerRb.rotation = playerRb.rotation * Quaternion.Euler(0f, turn, 0f);
        //현재 playerRb의 로테이션 값에서 3차원 백터값(0,turn,0)의 방향으로 회전한다.
    }
}
