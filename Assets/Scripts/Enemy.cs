using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
public class Enemy : LivingEntity //부모 클래스가 이미 포톤 함수를 상속 받고있어 따로 선언 x
{
    public LayerMask whatIsTarget; //추적 대상 레이어
    private LivingEntity targetEntity; //추적 대상
    private NavMeshAgent pathFinder; //경로 계산 AI 에이전트

    public ParticleSystem hitEffect;
    public AudioClip deathSound;
    public AudioClip hitSound;
    private Animator enemyAnimator;
    private AudioSource enemyAudioPlayer;
    private Renderer enemyRenderer;
    public float damage = 20f;
    public float timeBetAttack = 0.5f; //공격 간격
    private float lastAttackTime; //마지막 공격 시점
    private  bool hasTarget
    {
        get 
        {    //추적할 대상이 존재 하고 대상이 사망 하지 않았다면
            if(targetEntity != null&& !targetEntity.dead)
            {
                return true;
            }
                
            return false;
        }
    }
    private readonly int hashHasTarget = Animator.StringToHash("HasTarget");
    private readonly int hashDie = Animator.StringToHash("Die");
    private void Awake()
    {
        pathFinder = GetComponent<NavMeshAgent>();
        enemyAnimator = GetComponent<Animator>();
        enemyAudioPlayer = GetComponent<AudioSource>();
        enemyRenderer = GetComponentInChildren<Renderer>();
        //whatIsTarget = LayerMask.NameToLayer("PLAYER");
    }
    [PunRPC] //예상, 클라이언트의 데이터를 업데이트 할 때 사용할 것 같음
    public void Setup(float newHealth,float newDamage, float newSpeed, Color skinColor)
    {   
        startingHealth = newHealth;
        health = newHealth;
        damage = newDamage;
        pathFinder.speed = newSpeed;
        enemyRenderer.material.color = skinColor;

    }
    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return; //마스터 클라이언트만 추적 코드를 실행하고, 추적 데이러를 클라이언트에 전파
       //StartCoroutine("UpdatePath");
        InvokeRepeating("UpdatePath", 0.01f, 0.25f); //UpdatePath함수를 0.01초 후에 실행하고 이후 0.25초의 간격으로 반복 실행 한다.
        // 게임오브젝트가 활성화에 동시에 AI 추적 루틴이 시작
    }
 
    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return; //마스터 클라이언트만 추적 애니메이션을 실행한다.
        enemyAnimator.SetBool(hashHasTarget, hasTarget);
    }
    void UpdatePath() // 주기적으로 추적할 대상의 위치를 찾아 
    {                // 업데이트 된다.
        if (!dead)
        {
            if (hasTarget) //추적 대상이 있다면
            {
                //Debug.Log("호출");
                pathFinder.isStopped = false;
                pathFinder.SetDestination(targetEntity.transform.position);
            }
            else //추적 대상이 없다면
            {
                pathFinder.isStopped = true;
                Collider[] colliders = Physics.OverlapSphere(transform.position, 20f, whatIsTarget);
                for (int i = 0; i < colliders.Length; i++)
                {
                    LivingEntity livingEntity = colliders[i].GetComponent<LivingEntity>();
                    if (livingEntity != null && !livingEntity.dead)
                    {
                        targetEntity = livingEntity;
                        //Debug.Log(targetEntity.name);
                        break;
                    }
                }
            }
            //yield return new WaitForSeconds(0.25f);
        }
    }
    [PunRPC]
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if(!dead)
        {
            //공격받은 지점과 방향으로 파티클 효과 재생
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitEffect.Play();
            enemyAudioPlayer.PlayOneShot(hitSound);
        }
        base.OnDamage(damage, hitPoint, hitNormal);
    }
    public override void Die()
    {
        base.Die(); //기본으로 사망처리 하고 
        Collider[] enemyColliders = GetComponents<Collider>();
        for (int i = 0; i < enemyColliders.Length; i++)
        {   //다른 AI의 방해를 받지 않도록 자신의 모든 콜라이더를 비활성화
            enemyColliders[i].enabled = false;
        }
        pathFinder.isStopped = true;
        pathFinder.enabled = false;
        enemyAudioPlayer.PlayOneShot(deathSound);
        enemyAnimator.SetTrigger(hashDie);
    }
    private void OnTriggerStay(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return; //마스터 클라이언트만 공격 데이터 업데이트를 진행한다.

         //트리거 충돌한 상대방 게임오브젝트가 추적 대상이라면 공격 실생 
        if(!dead && Time.time >= lastAttackTime + timeBetAttack) 
        { 
            // 상대방의 LivingEntity 타입을 가져오기 
            LivingEntity attackTarget = other.GetComponent<LivingEntity>();

            if(attackTarget != null&& attackTarget== targetEntity)
            {   // 상대방의 LivingEntity 가 자신의 추적 대상이라면 공격 실행 
                lastAttackTime = Time.time;

                Vector3 hitPoint = other.ClosestPoint(transform.position);
                   //상대방의 피격위치와 피격 방향을 근사값으로 계산
                Vector3 hitNormal = transform.position - other.transform.position;
                attackTarget.OnDamage(damage,hitPoint, hitNormal);
            }
        }
    }
}
