using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
//IK : �޼� �������� ���⿡ ��Ȯ�ϰ� ���� �ǰ� 
// �Ѿ� �߻�
public class PlayerShooter : MonoBehaviourPun
{
    public Gun gun;
    public Transform gunPivot; //�ѹ�ġ ������
    public Transform leftHandMound; // ���� ���� ������ ��ġ
    public Transform rightHandMound; // ���� ������ ���� ��ġ
    private PlayerInput playerInput;
    private Animator playerAnimator;
    private readonly int hashReload = Animator.StringToHash("Reload");
    private void OnEnable()
    {  //���Ͱ� Ȱ��ȭ �ɶ� �ѵ� �Բ� Ȱ��ȭ
        gun.gameObject.SetActive(true);
    }
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
    }
    void Update()
    {
        if (!photonView.IsMine) return; //����䰡 �ڽ��� ���� �ƴ϶�� == ���� ������Ʈ�� �ƴ϶�� �Լ��� �����Ѵ�. (����Ʈ ������Ʈ�� ��ũ��Ʈ ��� ��Ȱ��ȭ�� ����.)

        if (playerInput.fire)
        {
            gun.Fire();
        }
        else if(playerInput.reload)
        {
            if (gun.Reload())
            {
                playerAnimator.SetTrigger(hashReload);
            }
        }
        UpdateUI();
    }
    void UpdateUI() //ź�� UI���� 
    {
        if(gun != null && UIManager.Instance!=null) 
        {
            UIManager.Instance.UpdateAmmoText(gun.magAmmo,gun.ammoRemain);
        }


    }
    private void OnAnimatorIK(int layerIndex)
    {    //���� ������ gunPivot�� 3D���� ������ �Ȳ�ġ ��ġ�� �̵� 
        gunPivot.position = playerAnimator.GetIKHintPosition(AvatarIKHint.RightElbow);

        //IK�� ��� �Ͽ� �޼��� ��ġ�� ȸ���� ���� ���� ������ ����
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f); //����ġ
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

        playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandMound.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand,leftHandMound.rotation);

       // gunPivot.position = playerAnimator.GetIKHintPosition(AvatarIKHint.LeftElbow);
        //IK�� ��� �Ͽ� �������� ��ġ�� ȸ���� ���� ������ ������ ����
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);

        playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, rightHandMound.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.RightHand, rightHandMound.rotation);


    }
    private void OnDisable()
    {
        //���Ͱ� ��Ȱ��ȭ �ɶ� �ѵ� �Բ� ��Ȱ��ȭ
        gun.gameObject.SetActive(false);
    }
}
