using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
이 스크립트는 2가지 역할을 한다.
1. IK : 왼손 오른손이 무기에 정확히 부착되게 한다.
2. 총알을 발사한다.
*/
public class PlayerShooter : MonoBehaviour
{
    private PlayerInput playerInput;
    private Animator playerAnimator;

    private readonly int hashReload = Animator.StringToHash("Reload");

    public Gun gun; //사용할 총
    public Transform gunPivot; //총 배치 기준점
    public Transform leftHandMound; //총의 왼쪽 손잡이 위치
    public Transform rightHandMound; //총의 오른쪽 손잡이 위치
    private void OnEnable()
    { //슈터가 활성화 될 때 총도 함께 활성화 한다.
        gun.gameObject.SetActive(true); //멀티게임은 플레이어도 오브젝트 풀링하기 때문에 이렇게 사용
    }
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerAnimator = GetComponent<Animator>();
    }
    void Update()
    {
        if(playerInput.fire) //마우스 왼쪽클릭 혹은 왼쪽 컨트롤키를 누르는 동안이면
        {
            gun.Fire();
        }
        else if(playerInput.reload) //r키를 눌렀을 때
        {
            if(gun.Reload()) //이미 재장전 중이거나, 남은 총알이 없거나, 탄창에 이미 총알이 가득차 있지 않으면
            {
                playerAnimator.SetTrigger(hashReload); //리로드 한다.
            }
        }
        UpdateUI();
    }
    void UpdateUI() //탄알집, 탄알수 UI 갱신
    {

    }
    private void OnAnimatorIK(int layerIndex)
    { //총의 기준점의 위치를 플레이어 3D 아바타의 오른쪽 팔꿈치 포지션으로 옮긴다.
        gunPivot.position = playerAnimator.GetIKHintPosition(AvatarIKHint.RightElbow); // AvatarIKHint : 아바타의 부위를 나타냄

        //IK를 사용하여 왼손의 위치와 회전을 총의 왼쪽 손잡이 회전과 위치에 맞춘다.
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f); // SetIKPositionWeight : 가중치를 변경한다.
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f); // 왼손을 쓰겠다 = 1, 안쓰겠다 = 0

        playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandMound.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandMound.rotation);

        //IK를 사용하여 오른손의 위치롸 회전을 총의 오른쪽 손잡이 회전과 위치에 맞춘다.
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
