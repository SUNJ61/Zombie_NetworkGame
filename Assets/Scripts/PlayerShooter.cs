using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
//IK : 왼손 오른손이 무기에 정확하게 부착 되게 
// 총알 발사
public class PlayerShooter : MonoBehaviourPun
{
    public Gun gun;
    public Transform gunPivot; //총배치 기준점
    public Transform leftHandMound; // 총의 왼쪽 손잡이 위치
    public Transform rightHandMound; // 총의 오른손 잡이 위치
    private PlayerInput playerInput;
    private Animator playerAnimator;
    private readonly int hashReload = Animator.StringToHash("Reload");
    private void OnEnable()
    {  //슈터가 활성화 될때 총도 함께 활성화
        gun.gameObject.SetActive(true);
    }
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
    }
    void Update()
    {
        if (!photonView.IsMine) return; //포톤뷰가 자신의 것이 아니라면 == 로컬 오브젝트가 아니라면 함수를 리턴한다. (리모트 오브젝트는 스크립트 기능 비활성화와 같음.)

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
    void UpdateUI() //탄알 UI갱신 
    {
        if(gun != null && UIManager.Instance!=null) 
        {
            UIManager.Instance.UpdateAmmoText(gun.magAmmo,gun.ammoRemain);
        }


    }
    private void OnAnimatorIK(int layerIndex)
    {    //총이 기준점 gunPivot을 3D모델의 오른쪽 팔꿈치 위치로 이동 
        gunPivot.position = playerAnimator.GetIKHintPosition(AvatarIKHint.RightElbow);

        //IK를 사용 하여 왼손의 위치와 회전을 총의 왼쪽 손잡이 맞춤
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f); //가중치
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

        playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandMound.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand,leftHandMound.rotation);

       // gunPivot.position = playerAnimator.GetIKHintPosition(AvatarIKHint.LeftElbow);
        //IK를 사용 하여 오른손의 위치와 회전을 총의 오른쪽 손잡이 맞춤
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);

        playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, rightHandMound.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.RightHand, rightHandMound.rotation);


    }
    private void OnDisable()
    {
        //슈터가 비활성화 될때 총도 함께 비활성화
        gun.gameObject.SetActive(false);
    }
}
