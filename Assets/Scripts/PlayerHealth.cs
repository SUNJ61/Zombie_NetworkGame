using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun; //�� Ŭ������ �θ�Ŭ�������� �̹� ������ Ŭ���̾�Ʈ���� �����͸� ó���Ѵٰ� �����ߴ�. 
public class PlayerHealth : LivingEntity
{
    public Slider healthSlider;  //ü���� ǥ���� �����̴�
    public AudioClip hitClip;  //�ǰ� �Ҹ�
    public AudioClip itemPickupClip; // ������ �ݴ� �Ҹ� 
    public AudioClip deathClip;
    private AudioSource playerAudioPlayer; //�÷��̾� �Ҹ� �����
    private Animator playerAnimator; //�÷��̾� �ִϸ�����
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
    protected override void OnEnable() //���ٶ��̵� �ٸ��� �� ����
    {
        base.OnEnable();// �θ� Ŭ������ �̺�Ʈ �Լ� 
        healthSlider.gameObject.SetActive(true);
        healthSlider.maxValue = startingHealth;
        healthSlider.value = health;
        playerMovement.enabled = true;
        playerShooter.enabled = true;
    }
    //ü�� ȸ��
    [PunRPC]
    public override void RestoreHealth(float newHealth)
    {
        base.RestoreHealth(newHealth);
        healthSlider.value = health;
        //������Ʈ �� ü��

    }
    //����� ó�� 
    [PunRPC]
    public override void OnDamage(float damage,Vector3 hitPoint,
        Vector3 hitDirection)
    {
        if(!dead)
        {
            playerAudioPlayer.PlayOneShot(hitClip);
             //������� ���� ��쿡�� ȿ���� ���
        }

        //LivingEntity�� OnDamage �����ؼ� ����� ����
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
        Invoke("ReSpawn", 5.0f); //5���� ������ �Լ��� �ҷ��´�.
    }
    public void ReSpawn() //�÷��̾ ����� 5���� ��Ȱ
    {
        if(photonView.IsMine) //���� �÷��̾� ��� == �� ȭ�鿡�� �����ϴ� ĳ���Ͷ��
        {
            Vector3 randomSpawnPos = Random.insideUnitSphere * 5f; //�������� �������� 5�� �� �ȿ� ������ ��ǥ�� �����Ѵ�. 
            randomSpawnPos.y = 0f; //y���� ���� 0���� �����. == ���� ���� �̵� ��Ų��.
            transform.position = randomSpawnPos; //�÷��̾� ��ġ�� �ش� ��ǥ�� �̵���Ų��.
        }

        gameObject.SetActive(false); //OnDisable ȣ���ϱ� ���� ���� ()
        gameObject.SetActive(true); //OnEnable�� ȣ���ϱ� ���� ���� ��, 5�� �Ŀ� ���ٰ� �ٽ� Ų��. (ü�¹�, ü���� �ƽ������� �ʱ�ȭ, �÷��̾� �̵� ��ũ��Ʈ�� �ٽ� Ȱ��ȭ, �÷��̾� �� ��ũ��Ʈ�� �ٽ� Ȱ��ȭ)
    }
    private void OnTriggerEnter(Collider other)
    {       
        //�����۰� �浹�� ��� �ش� �������� ��� �ϴ� ó�� 
        if(!dead)
        {   
            //�浹�� �������� ���� IItem ���۳�Ʈ�� ������ �´�.
            IItem item = other.GetComponent<IItem>();
            if(item != null )
            {
                if(PhotonNetwork.IsMasterClient) //ȣ��Ʈ��� �������� ��� �����ϴ�. ��, ȣ��Ʈ���� ������ ����� ��� ȿ���� ��� Ŭ���̾�Ʈ�� ����ȭ ��Ų��.
                { //Use�޼ҵ带 �����Ͽ� ������ ��� (������ ��ũ��Ʈ���� ���� ������ �ٲپ���Ѵ�. �� ������)
                    item.Use(gameObject);//������ ���� �Ҹ� ���
                }
                playerAudioPlayer.PlayOneShot(itemPickupClip, 1.0f);
            }
        }
    }
}
