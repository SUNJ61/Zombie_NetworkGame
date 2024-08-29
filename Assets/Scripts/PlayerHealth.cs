using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun; //이 클래스의 부모클래스에서 이미 마스터 클라이언트부터 데이터를 처리한다고 결정했다. 
public class PlayerHealth : LivingEntity
{
    public Slider healthSlider;  //체력을 표시할 슬라이더
    public AudioClip hitClip;  //피격 소리
    public AudioClip itemPickupClip; // 아이템 줍는 소리 
    public AudioClip deathClip;
    private AudioSource playerAudioPlayer; //플레이어 소리 재생기
    private Animator playerAnimator; //플레이어 애니메이터
    private PlayerMovement playerMovement;
    private PlayerShooter playerShooter;
    private readonly int hashDie = Animator.StringToHash("Die");
    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerAudioPlayer = GetComponent<AudioSource>();
        playerShooter = GetComponent<PlayerShooter>();
        
    }
    protected override void OnEnable() //오바라이드 다르게 쓸 예정
    {
        base.OnEnable();// 부모 클래스의 이벤트 함수 
        healthSlider.gameObject.SetActive(true);
        healthSlider.maxValue = startingHealth;
        healthSlider.value = health;
        playerMovement.enabled = true;
        playerShooter.enabled = true;
    }
    //체력 회복
    [PunRPC]
    public override void RestoreHealth(float newHealth)
    {
        base.RestoreHealth(newHealth);
        healthSlider.value = health;
        //업데이트 된 체력

    }
    //대미지 처리 
    [PunRPC]
    public override void OnDamage(float damage,Vector3 hitPoint,
        Vector3 hitDirection)
    {
        if(!dead)
        {
            playerAudioPlayer.PlayOneShot(hitClip);
             //사망하지 않은 경우에만 효과음 재생
        }

        //LivingEntity의 OnDamage 실행해서 대미지 적용
        base.OnDamage(damage, hitPoint, hitDirection);
        healthSlider.value = health;
    }
    public override void Die()
    {
        base.Die();
        healthSlider.gameObject.SetActive(false);
        playerAudioPlayer.PlayOneShot(deathClip, 1.0f);

        playerAnimator.SetTrigger(hashDie);
        playerMovement.enabled=false;
        playerShooter.enabled=false;
        Invoke("ReSpawn", 5.0f); //5초후 리스폰 함수를 불러온다.
    }
    public void ReSpawn() //플레이어가 사망후 5초후 부활
    {
        if(photonView.IsMine) //로컬 플레이어 라면 == 내 화면에서 조정하는 캐릭터라면
        {
            Vector3 randomSpawnPos = Random.insideUnitSphere * 5f; //원점에서 반지름이 5인 구 안에 랜덤한 좌표를 저장한다. 
            randomSpawnPos.y = 0f; //y축의 값을 0으로 만든다. == 지면 위로 이동 시킨다.
            transform.position = randomSpawnPos; //플레이어 위치를 해당 좌표로 이동시킨다.
        }

        gameObject.SetActive(false); //OnDisable 호출하기 위해 선언 ()
        gameObject.SetActive(true); //OnEnable을 호출하기 위해 선언 즉, 5초 후에 껏다가 다시 킨다. (체력바, 체력을 맥스값으로 초기화, 플레이어 이동 스크립트를 다시 활성화, 플레이어 총 스크립트를 다시 활성화)
    }
    private void OnTriggerEnter(Collider other)
    {       
        //아이템과 충돌한 경우 해당 아이템을 사용 하는 처리 
        if(!dead)
        {   
            //충돌한 상대방으로 부터 IItem 컴퍼넌트를 가지고 온다.
            IItem item = other.GetComponent<IItem>();
            if(item != null )
            {
                if(PhotonNetwork.IsMasterClient) //호스트라면 아이템을 사용 가능하다. 즉, 호스트에서 아이템 사용후 사용 효과를 모든 클라이언트에 동기화 시킨다.
                { //Use메소드를 실행하여 아이템 사용 (아이템 스크립트에서 포톤 적용을 바꾸어야한다. 로 추측중)
                    item.Use(gameObject);//아이템 습득 소리 재생
                }
                playerAudioPlayer.PlayOneShot(itemPickupClip, 1.0f);
            }
        }
    }
}
