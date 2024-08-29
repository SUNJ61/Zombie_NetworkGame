using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Gun : MonoBehaviourPun, IPunObservable //인테페이스를 상속 받는다.
{                    //발사준비됨, 탄창이 빔 , 재장전 중
    public enum State { Ready, Empty,Reloading}
    public State state { get; private set; }
    public Transform fireTransform; //탄알 발사 위치
    public ParticleSystem muzzleFlashEffect;//총구화염 효과
    public ParticleSystem shellEjectEffect; //탄피배출 효과
    private LineRenderer lineRenderer;
    private AudioSource gunAudioPlayer;
    public AudioClip shotClip;
    public AudioClip reloadClip;
    public float damage = 25f; //공격력
    private float fireDistance = 50f; //사정거리 
    public int ammoRemain = 100; //남은  전체 탄알
    public int magCapacity = 25; //탄창 용량
    public int magAmmo;//현재 탄창에 남아 있는 탄알 
    public float timeBetFire = 0.12f; //탄알 발사 간격
    public float reloadTime = 1.8f; // 재장전 소요 시간 
    private float lastFireTime; // 총을 마지막으로 발사한 시점

    void Awake()
    {
        gunAudioPlayer = GetComponent<AudioSource>();
        lineRenderer = GetComponent<LineRenderer>();
        //사용 할 점을 두개로 변경 
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
    }
    private void OnEnable()
    {
        magAmmo = magCapacity;
        state = State.Ready;
        lastFireTime = 0f;
    }
    public void Fire() //발사 시도, shot함수를 불러오기 위한 조건을 걸어둔 함수
    {
        if (state == State.Ready && Time.time >= lastFireTime + timeBetFire)
        {
            lastFireTime = Time.time;
            Shot();
        }
    }
    private void Shot() // 실제 발사 처리
    {
        RaycastHit hit;
        //Ray ray = new Ray(fireTransform.position,fireTransform.forward);
        Vector3 hitPosition = Vector3.zero;
        if(Physics.Raycast(fireTransform.position, fireTransform.forward, out hit, fireDistance))
        {
            //충돌한 상대방으로 부터 IDamageable 오브젝트 가져오기를 시도 한다.
            IDamageable target = hit.collider.GetComponent<IDamageable>();
            if(target != null)
            {
                //상대방의 OnDamage  함수를 실행시켜 상대방에 대미지 주기 
                target.OnDamage(damage, hit.point, hit.normal);
            }
            //레이가 충돌한 위치 저장 
            hitPosition = hit.point;
        }
        else
        {
            //레이가 다른 물체와 충돌 하지 않았다면
            // 탄알이 최대 사정 거리까지 날아갔을 때의  위치를 충돌 위치로 사용
            hitPosition = fireTransform.position + fireTransform.forward * fireDistance;
            //lineRenderer.SetPosition(1, ray.GetPoint(50f));
        }
        StartCoroutine(ShotEffect(hitPosition));
        photonView.RPC("ShotProcessOnServer", RpcTarget.MasterClient); //실제 발사처리는 호스트(마스터 클라이언트)에서만 일어난다.
                                                                       //즉, 호스트가 총을 쏘는 데이터를 변화시키고, 클라이언트에서는 ui변화, 라인렌더러발생, 효과음등 데이터와 관련없는 처리를 한다.
        magAmmo--;
        if(magAmmo <= 0)
        {
            state = State.Empty;
        }

    }
    [PunRPC] //호스트에서 실제로 발사 처리
    private void ShotProcessOnServer()
    {
        RaycastHit hit;
        Vector3 hitPosition = Vector3.zero;
        if (Physics.Raycast(fireTransform.position, fireTransform.forward, out hit, fireDistance))
        {
            IDamageable target = hit.collider.GetComponent<IDamageable>();
            if (target != null)
            {
                target.OnDamage(damage, hit.point, hit.normal);
            }
            hitPosition = hit.point;
        }
        else
        {
            hitPosition = fireTransform.position + fireTransform.forward * fireDistance;
        }
        photonView.RPC("ShotEffectProcessOnClient", RpcTarget.All, hitPosition); //아직 해당 함수 만들진 않았음 (이펙트 처리는 클라이언트 뿐만아니라 호스트 모두 출력되어야 하므로 All을 사용했다.)
    }
    [PunRPC]
    private void ShotEffectProcessOnClient(Vector3 hitPos) //ShotProcessOnServer함수에서 hitPosition 데이터를 전달 받아 코루틴을 돌린다.
    {
        StartCoroutine(ShotEffect(hitPos));
    }
   IEnumerator ShotEffect(Vector3 hitPosition)
    {
       
        muzzleFlashEffect.Play();
        shellEjectEffect.Play();
        gunAudioPlayer.PlayOneShot(shotClip);
        //선의 시작점은 총구의 위치 
        lineRenderer.SetPosition(0,fireTransform.position);
        //선의 끝점은 입력으로 들어온 충돌 위치 
        lineRenderer.SetPosition(1,hitPosition);
        lineRenderer.enabled = true;
        yield return new WaitForSeconds(0.03f);

        lineRenderer.enabled = false;

    }
    public bool Reload() //재장전 시도 
    {
        if(state == State.Reloading|| ammoRemain <=0 || magAmmo >= magCapacity)
        {
            // 이미 재장전 중이거나  남은 탄알이 없거나 탄창에 탄알이 이미 가득 한 경우 
            //재장전 할 수 없다. 
            return false;
        }
        StartCoroutine(ReloadRoutine());
        return true;
    }
    IEnumerator ReloadRoutine() //실제 재장전 처리를 진행
    {
        state = State.Reloading;
        gunAudioPlayer.PlayOneShot(reloadClip);


        yield return new WaitForSeconds(reloadTime);
        // 탄창에 채울 탄알을 계산 
        int ammoToFill = magCapacity - magAmmo;
        // 탄창에 채워야 할 탄알이 남은 탄알 보다 많타면
        // 채워야 할 탄알수를 남은 탄알수에 맞추어서 줄임 
        if(ammoRemain<ammoToFill)
        {
            ammoToFill = ammoRemain;
        }
        //탄창을 채움
        magAmmo += ammoToFill;
        //남은 탄알에서 탄창에 채운 만큼 탄알을 뺌
        ammoRemain -= ammoToFill;

        state = State.Ready;
    }
    [PunRPC]
    public void AddAmmo(int ammo)
    {
        ammoRemain += ammo;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) //주기적으로 자동 실행되는 동기화 메소드이다.  (stream = 네트워크 , info = 추가되는 정보)
    {
        if(stream.IsWriting) //송신, 로컬 오브젝트의 데이터를 송신한다. (여기서는 총과 관련된 데이터)
        {
            stream.SendNext(ammoRemain); //현재 가지고있는 남은 총알의 개수의 데이터 ammoRemain을 네트워크를 통해 보낸다. (송신)
            stream.SendNext(magAmmo); //현재 장전된 총알의 개수의 데이터 magAmmo를 네트워크를 통해 보낸다. (송신)
            stream.SendNext(state); //현재 총의 상태 (Ready, Empty,Reloading)의 데이터인 state를 네트워크를 통해 보낸다. (송신)
        }
        if(stream.IsReading) //수신, 다른 네트워크 유저의 데이터를 수신 받는다. (여기서는 총과 관련된 데이터)
        {
            ammoRemain = (int)stream.ReceiveNext(); //현재 가지고있는 남은 총알의 개수의 데이터 ammoRemain을 네트워크를 통해 받는다. (수신)
            magAmmo = (int)stream.ReceiveNext(); //현재 장전된 총알의 개수의 데이터 magAmmo를 네트워크를 통해 받는다. (수신)
            state = (State)stream.ReceiveNext(); //현재 총의 상태 (Ready, Empty,Reloading)의 데이터인 state를 네트워크를 통해 받는다. (수신)
        }
    }
}
