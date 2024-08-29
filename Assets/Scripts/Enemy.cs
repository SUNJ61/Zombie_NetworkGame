using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
public class Enemy : LivingEntity //�θ� Ŭ������ �̹� ���� �Լ��� ��� �ް��־� ���� ���� x
{
    public LayerMask whatIsTarget; //���� ��� ���̾�
    private LivingEntity targetEntity; //���� ���
    private NavMeshAgent pathFinder; //��� ��� AI ������Ʈ

    public ParticleSystem hitEffect;
    public AudioClip deathSound;
    public AudioClip hitSound;
    private Animator enemyAnimator;
    private AudioSource enemyAudioPlayer;
    private Renderer enemyRenderer;
    public float damage = 20f;
    public float timeBetAttack = 0.5f; //���� ����
    private float lastAttackTime; //������ ���� ����
    private  bool hasTarget
    {
        get 
        {    //������ ����� ���� �ϰ� ����� ��� ���� �ʾҴٸ�
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
    [PunRPC] //����, Ŭ���̾�Ʈ�� �����͸� ������Ʈ �� �� ����� �� ����
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
        if (!PhotonNetwork.IsMasterClient) return; //������ Ŭ���̾�Ʈ�� ���� �ڵ带 �����ϰ�, ���� ���̷��� Ŭ���̾�Ʈ�� ����
       //StartCoroutine("UpdatePath");
        InvokeRepeating("UpdatePath", 0.01f, 0.25f); //UpdatePath�Լ��� 0.01�� �Ŀ� �����ϰ� ���� 0.25���� �������� �ݺ� ���� �Ѵ�.
        // ���ӿ�����Ʈ�� Ȱ��ȭ�� ���ÿ� AI ���� ��ƾ�� ����
    }
 
    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return; //������ Ŭ���̾�Ʈ�� ���� �ִϸ��̼��� �����Ѵ�.
        enemyAnimator.SetBool(hashHasTarget, hasTarget);
    }
    void UpdatePath() // �ֱ������� ������ ����� ��ġ�� ã�� 
    {                // ������Ʈ �ȴ�.
        if (!dead)
        {
            if (hasTarget) //���� ����� �ִٸ�
            {
                //Debug.Log("ȣ��");
                pathFinder.isStopped = false;
                pathFinder.SetDestination(targetEntity.transform.position);
            }
            else //���� ����� ���ٸ�
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
            //���ݹ��� ������ �������� ��ƼŬ ȿ�� ���
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitEffect.Play();
            enemyAudioPlayer.PlayOneShot(hitSound);
        }
        base.OnDamage(damage, hitPoint, hitNormal);
    }
    public override void Die()
    {
        base.Die(); //�⺻���� ���ó�� �ϰ� 
        Collider[] enemyColliders = GetComponents<Collider>();
        for (int i = 0; i < enemyColliders.Length; i++)
        {   //�ٸ� AI�� ���ظ� ���� �ʵ��� �ڽ��� ��� �ݶ��̴��� ��Ȱ��ȭ
            enemyColliders[i].enabled = false;
        }
        pathFinder.isStopped = true;
        pathFinder.enabled = false;
        enemyAudioPlayer.PlayOneShot(deathSound);
        enemyAnimator.SetTrigger(hashDie);
    }
    private void OnTriggerStay(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return; //������ Ŭ���̾�Ʈ�� ���� ������ ������Ʈ�� �����Ѵ�.

         //Ʈ���� �浹�� ���� ���ӿ�����Ʈ�� ���� ����̶�� ���� �ǻ� 
        if(!dead && Time.time >= lastAttackTime + timeBetAttack) 
        { 
            // ������ LivingEntity Ÿ���� �������� 
            LivingEntity attackTarget = other.GetComponent<LivingEntity>();

            if(attackTarget != null&& attackTarget== targetEntity)
            {   // ������ LivingEntity �� �ڽ��� ���� ����̶�� ���� ���� 
                lastAttackTime = Time.time;

                Vector3 hitPoint = other.ClosestPoint(transform.position);
                   //������ �ǰ���ġ�� �ǰ� ������ �ٻ簪���� ���
                Vector3 hitNormal = transform.position - other.transform.position;
                attackTarget.OnDamage(damage,hitPoint, hitNormal);
            }
        }
    }
}
