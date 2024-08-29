using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
�� ��ũ��Ʈ�� 2���� ������ �Ѵ�.
1. IK : �޼� �������� ���⿡ ��Ȯ�� �����ǰ� �Ѵ�.
2. �Ѿ��� �߻��Ѵ�.
*/
public class PlayerShooter : MonoBehaviour
{
    private PlayerInput playerInput;
    private Animator playerAnimator;

    private readonly int hashReload = Animator.StringToHash("Reload");

    public Gun gun; //����� ��
    public Transform gunPivot; //�� ��ġ ������
    public Transform leftHandMound; //���� ���� ������ ��ġ
    public Transform rightHandMound; //���� ������ ������ ��ġ
    private void OnEnable()
    { //���Ͱ� Ȱ��ȭ �� �� �ѵ� �Բ� Ȱ��ȭ �Ѵ�.
        gun.gameObject.SetActive(true); //��Ƽ������ �÷��̾ ������Ʈ Ǯ���ϱ� ������ �̷��� ���
    }
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerAnimator = GetComponent<Animator>();
    }
    void Update()
    {
        if(playerInput.fire) //���콺 ����Ŭ�� Ȥ�� ���� ��Ʈ��Ű�� ������ �����̸�
        {
            gun.Fire();
        }
        else if(playerInput.reload) //rŰ�� ������ ��
        {
            if(gun.Reload()) //�̹� ������ ���̰ų�, ���� �Ѿ��� ���ų�, źâ�� �̹� �Ѿ��� ������ ���� ������
            {
                playerAnimator.SetTrigger(hashReload); //���ε� �Ѵ�.
            }
        }
        UpdateUI();
    }
    void UpdateUI() //ź����, ź�˼� UI ����
    {

    }
    private void OnAnimatorIK(int layerIndex)
    { //���� �������� ��ġ�� �÷��̾� 3D �ƹ�Ÿ�� ������ �Ȳ�ġ ���������� �ű��.
        gunPivot.position = playerAnimator.GetIKHintPosition(AvatarIKHint.RightElbow); // AvatarIKHint : �ƹ�Ÿ�� ������ ��Ÿ��

        //IK�� ����Ͽ� �޼��� ��ġ�� ȸ���� ���� ���� ������ ȸ���� ��ġ�� �����.
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f); // SetIKPositionWeight : ����ġ�� �����Ѵ�.
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f); // �޼��� ���ڴ� = 1, �Ⱦ��ڴ� = 0

        playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandMound.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandMound.rotation);

        //IK�� ����Ͽ� �������� ��ġ�� ȸ���� ���� ������ ������ ȸ���� ��ġ�� �����.
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);

        playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, rightHandMound.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.RightHand, rightHandMound.rotation);
    }
    private void OnDisable()
    {
        gun.gameObject.SetActive(false);
    }
}
