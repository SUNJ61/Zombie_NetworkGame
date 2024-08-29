using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class AmmoPack : MonoBehaviourPun,IItem
{//IItem�������̽��� ����ϰ� ������ �ش� ������Ʈ�� �������̶�� �˻��� �ʿ� x, ���� �ش� �������� ���� ������������ �˻����� �ʾƵ� �ȴ�.
    public int ammo = 30;
    public void Use(GameObject target)
    {
        PlayerShooter shooter = target.GetComponent<PlayerShooter>();
        if (shooter != null && shooter.gun != null) //��ȿ�� �˻� (�������� ����ϴ� ������Ʈ�� PlayerShooter�� ������ �ְ�, PlayerShooter �Ʒ��� gun�� ������������)
        {
            //shooter.gun.ammoRemain += ammo; //�÷��̾ ���� �Ѿ� �ѷ��� ammo(30��)��ŭ �ø���,  ��Ƽ���ӿ����� �ּ�ó�� (�̱۰��ӿ� �ڵ�)
            shooter.gun.photonView.RPC("AddAmmo",RpcTarget.All, ammo); //�������� ������ gun��ũ��Ʈ�� �ִ� AddAmmo�Լ��� �ҷ��� ȣ��Ʈ�� Ŭ���̾�Ʈ ��� ammoRemain ������ ammo(30��)��ŭ �ø���.
                                                                       //�ش� ��ũ��Ʈ�� �ҷ������� ������ AddAmmo�Լ��� [PunRPC]�� ����Ǿ��� �����̴�.
        }
        //target�� ź���� �߰��ϴ� ó��
        //Debug.Log("ź���� ���� �ߴ�: " + ammo);
        //Destroy(gameObject); //�Դ� ���� �ڱ� �ڽ�(ammo������Ʈ)�� �����ȴ�, �̱۰��ӿ� �ڵ� (��Ƽ�����̶� �ּ�ó��)
        PhotonNetwork.Destroy(gameObject); //��� Ŭ���̾�Ʈ (ȣ��Ʈ ����)���� ������ ����
    }
}
