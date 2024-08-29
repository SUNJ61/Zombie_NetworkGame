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
        //FixedUpdate�Լ������� Time.fixedDeltaTime�� ������ϴµ� Time.deltaTime�� ���
        //����Ƽ���� �ڵ����� Time.fixedDeltaTime�� ������ �ش�.
        Vector3 moveDistance = playerInput.move * transform.forward * moveSpeed * Time.deltaTime;
        playerRb.MovePosition(playerRb.position + moveDistance);
        //����ٵ� ����� �ڵ����� �ش� ���� �������� �̵����ִ� �Լ�, playerRb�� �����ǿ��� �ش� ���͸�ŭ �̵�
    }
    private void Rotate()
    {
        float turn = playerInput.rotate * rotateSpeed * Time.deltaTime;
        playerRb.rotation = playerRb.rotation * Quaternion.Euler(0f, turn, 0f);
        //���� playerRb�� �����̼� ������ 3���� ���Ͱ�(0,turn,0)�� �������� ȸ���Ѵ�.
    }
}
