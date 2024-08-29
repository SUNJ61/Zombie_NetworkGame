using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class HealthPack : MonoBehaviourPun, IItem
{
    public float health = 50f;
    public void Use(GameObject target)
    {
        LivingEntity life = target.GetComponent<LivingEntity>(); //�����ۿ� ���� ������Ʈ�� LivingEntity�� ������Ʈ�� �����´�. (��ӵ� ��ũ��Ʈ ����)

        if (life != null) //LivingEntity�� ��ӹ��� ������Ʈ ���
        {
            life.RestoreHealth(health); //ü���� ȸ�� ��Ų��.
        }

        //Destroy(gameObject); //��� ���� �ڱ��ڽ� ����, �̱۰��ӿ� �ڵ�
        PhotonNetwork.Destroy(gameObject); //��� Ŭ���̾�Ʈ���� ����
    }
}
