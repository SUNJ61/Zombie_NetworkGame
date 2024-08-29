using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using Photon.Pun;
//PunPRC�� �۵������ ��� �߻��ϴ��� ���ذ� �ȵǸ� OnDamage�� ApplyUpdatedHealth �Լ��� ������ �����Ͽ� �����ϱ�.
public class LivingEntity : MonoBehaviourPun,IDamageable
{
    public float startingHealth = 100f;  //���� ü��
    public float health {  get; protected set; } //���� ü��
    public bool dead { get; protected set; } // ��� ����
    public event Action onDeath; //��� �� �ߵ� �� �̺�Ʈ 

    [PunRPC] //ȣ��Ʈ(����) -> ��� Ŭ���̾�Ʈ ���� �������� ������¸� ����ȭ �ϴ� �޼ҵ�. ȣ��Ʈ������ �ش� ������ �����Ѵ�.
    public void ApplyUpdatedHealth(float newhealth, bool newDead) // �� �Լ��� ������ Ŭ���̾�Ʈ ��ǻ�Ϳ��� �Ͼ��. �Ű������� ȣ��Ʈ ��ǻ�Ϳ��� ���� ����. 
    {
        health = newhealth; //ȣ��Ʈ���� �޾ƿ� �� newhealth
        dead = newDead; //ȣ��Ʈ���� �޾ƿ� �� newDead
    }

     //����ü�� Ȱ��ȭ �� �� ���¸� ����        
    protected virtual  void OnEnable()
    {             //���� ���� ���� �޼��� 
        dead = false; //������� ���� ���·�
        health = startingHealth; // ü���� ����ü������ �ʱ�ȭ

    }
    [PunRPC] //ȣ��Ʈ���� �ܵ� ���� �ǰ� ȣ��Ʈ�� ���� �ٸ� Ŭ���̾�Ʈ���� �ϰ� ����ȴ�.
    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal) //��, ȣ��Ʈ ȭ�鿡�� � ������Ʈ�� �ش� �Լ��� �ҷ��´ٸ� ȣ��Ʈ���� ���� ������ ����� Ŭ���̾�Ʈ�� �����ϴ� ����̴�.
    {
        if (PhotonNetwork.IsMasterClient) //�ڱ��ڽ��� �����̰ų� ȣ��Ʈ���
        {
            health -= damage; //ȣ��Ʈ ��ǻ�Ϳ��� ������ ��ŭ ü�� ���Ҹ� ����.
            photonView.RPC("ApplyUpdatedHealth", RpcTarget.Others, health, dead); //ȣ��Ʈ ��ǻ�Ϳ��� ApplyUpdatedHealth �Լ��� ������� ȣ��Ʈ�� �ִ� health, dead�� ���� Ŭ���̾�Ʈ�� ����ȭ ��Ų��.
            photonView.RPC("OnDamage", RpcTarget.Others, damage, hitPoint, hitNormal); //ȣ��Ʈ ��ǻ�Ͱ� Ŭ���̾�Ʈ���� OnDamage�Լ��� ȣ���϶�� ����Ѵ�. ������ ������Ʈ �� health, dead���� ���� �Ʒ��� if���� ����.
        } //RpcTarget.Others�� ���� ������ ��� Ŭ���̾�Ʈ�� �ǹ��Ѵ�. ���⼭�� ȣ��Ʈ�� �ش� �ڵ带 �߻���Ű�⿡ ȣ��Ʈ�� ������ ��� ��, �ٸ� ��� Ŭ���̾�Ʈ�� �ǹ��Ѵ�.
        if (health<=0 && !dead)
        {
            Die();
        }
      
    }
    //ü���� ȸ�� �ϴ� ���
    [PunRPC]
    public virtual void RestoreHealth(float newHealth)
    {
        if(dead)
        {     //�̹� �׾��ٸ� ü���� ȸ�� �� �� ����
            return;
        }

        if (PhotonNetwork.IsMasterClient) //ȣ��Ʈ�� �����
        {
            health += newHealth; //ȣ��Ʈ�� ü���� �߰��ȴ�.
            photonView.RPC("ApplyUpdatedHealth", RpcTarget.Others, health, dead); //�������� Ŭ���̾�Ʈ�� ����ȭ �Ѵ�.
            photonView.RPC("RestoreHealth", RpcTarget.Others, newHealth); //�ٸ� ��Ʈ��ũ Ŭ���̾�Ʈ�� RestoreHealth�Լ��� ȣ���ϵ��� �Ѵ�.
        }
    }
    public virtual void Die()
    {
        if(onDeath != null)
        {
            onDeath();
        }
        dead = true;
    }
}
//LivingEntityŬ������  IDamageable�� ����ϹǷ� OnDamage()�޼��带 �ݵ�� ����
