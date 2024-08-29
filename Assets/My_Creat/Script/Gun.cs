using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum State { READY = 0, EMPTY, RELOADING } // 총의 상태를 발사준비가됨/탄창이비었다./재장전중이다 3개로 나누었다.
    public State state { get; private set; } //상태를 이 클래스에서 프로퍼티로만 업데이트 가능하다.
    public Transform fireTransform;
    public ParticleSystem muzzleFlashEffect;
    public ParticleSystem shellEjectEffect;
    public AudioClip shotClip;
    public AudioClip reloadClip;

    private LineRenderer lineRenderer;
    private AudioSource gunAudioPlayer;

    private float fireDistance = 50f; //사정거리
    private float lastFireTime; //마지막 총을 쏜 시간을 담는 변수

    public float damage = 25f; //공격력
    public float timeBetFire = 0.12f; //총알 발사 간격
    public float reloadTime = 1.8f; // 재장전 소요 시간

    public int ammoRemain = 100; //가지고 있는 남은 총알 수
    public int magCapacity = 25; //탄창 용량
    public int magAmmo; //현재 탄창에 남은 총알 수
    void Awake()
    {
        fireTransform = transform.GetChild(3).GetComponent<Transform>();
        muzzleFlashEffect = transform.GetChild(4).GetComponent<ParticleSystem>();
        shellEjectEffect = transform.GetChild(5).GetComponent<ParticleSystem>();
        lineRenderer = transform.GetComponent<LineRenderer>();
        gunAudioPlayer = GetComponent<AudioSource>();

        lineRenderer.positionCount = 2; //사용할 점을 2개로 변경
        lineRenderer.enabled = false;
    }
    private void OnEnable()
    {
        magAmmo = magCapacity; //현재 탄창에 총알수를 탄창의 크기(총알 맥스량)으로 바꾼다.
        state = State.READY;
        lastFireTime = 0f;
    }
    public void Fire() //발사 시도
    {
        if(state == State.READY && Time.time >= lastFireTime + timeBetFire) //발사준비 완료되었고, 발사후 0.12초가 지났으면
        {
            lastFireTime = Time.time;
            Shot();
        }
    }
    void Shot() //실제 발사 처리
    {
        //Ray ray = new Ray(fireTransform.position, fireTransform.forward); //ray선언 방법
        RaycastHit hit;
        Vector3 hitPosition = Vector3.zero;
        if(Physics.Raycast(fireTransform.position, fireTransform.forward, out hit, fireDistance))
        {//충돌한 상대방에 삽입된 스크립트에서 IDamageable 상속한 클래스 컴퍼넌트를 가져오기를 시도한다.
            IDamageable target = hit.collider.GetComponent<IDamageable>(); 
            if(target != null)//상대방의 컴퍼넌트 중에 IDamageable를 상속받은 클래스가 존재한다면
            {
                target.OnDamage(damage, hit.point, hit.normal); //상대방의 OnDamage를 실행시켜 데미지를 준다.
            }

            hitPosition = hit.point; //상대방의 컴퍼넌트 중에 IDamageable를 상속받은 클래스가 존재하지 않으면 위치만 저장한다.
        }
        else
        {//레이캐스트가 다른 물체에 맞지 않았다면
            hitPosition = fireTransform.position + fireTransform.forward * fireDistance;
            //총구 위치에서 정면으로 최대사거리만큼 떨어진 위치를 맞은 위치로 사용한다.
        }
        //StartCoroutine(shotEffect(hitPosition)); 아마 하지 않을까?
    }
    IEnumerator shotEffect(Vector3 hitPosition)
    {
        muzzleFlashEffect.Play();
        shellEjectEffect.Play();
        gunAudioPlayer.PlayOneShot(shotClip);
        lineRenderer.SetPosition(0, fireTransform.position); //라인렌더러 시작지점을 총구의 위치로 바꾼다.
        lineRenderer.SetPosition(1, hitPosition); //라인렌더러의 끝점은 입력으로 들어온 맞은 위치로 바꾼다.
        lineRenderer.enabled = true;

        yield return new WaitForSeconds(0.03f);

        lineRenderer.enabled = false;
        muzzleFlashEffect.Stop();
        shellEjectEffect.Stop();
    }
    public bool Reload() //재장전 시도
    {
        if (state == State.RELOADING || ammoRemain <= 0 || magAmmo >= magCapacity)
        {//이미 재장전 중이거나, 남은 총알이 없거나, 탄창에 이미 총알이 가득차 있으면
            return false; // false를 반환한다.
        }
        //위에서 함수를 탈출하지 않았으면 리로드 한다.

        StartCoroutine(ReloadRoutine());
        return true;
    }
    IEnumerator ReloadRoutine() //실제 재장전 처리 진행
    {
        state = State.RELOADING;
        gunAudioPlayer.PlayOneShot(reloadClip);

        yield return new WaitForSeconds(reloadTime);

        int ammoToFill = magCapacity - magAmmo; //탄창에 채울 탄알을 게산 (채울 총알수 = 최대 총알수 - 장전 된 총알수)
        if(ammoRemain < ammoToFill)//가진 전체 총알이 채울 총알수보다 적을 때
        {
            ammoToFill = ammoRemain; //채울 총알 수는 가진 전체 총알 수가 된다.
        }
        magAmmo += ammoToFill; //현재 총알에 채울 총알수를 더한다. (탄창을 채움)
        ammoRemain -= ammoToFill; //가진 전체 총알 수에서 장전한 총알 수를 뺀다.

        state = State.READY;
    }
    void Update()
    {
        
    }
}
