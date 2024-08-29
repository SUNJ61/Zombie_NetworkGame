using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class AmmoPack : MonoBehaviourPun,IItem
{//IItem인터페이스를 상속하고 있으면 해당 오브젝트를 아이템이라고 검사할 필요 x, 또한 해당 아이템이 무슨 아이템인지도 검사하지 않아도 된다.
    public int ammo = 30;
    public void Use(GameObject target)
    {
        PlayerShooter shooter = target.GetComponent<PlayerShooter>();
        if (shooter != null && shooter.gun != null) //유효성 검사 (아이템을 사용하는 오브젝트가 PlayerShooter를 가지고 있고, PlayerShooter 아래에 gun을 가지고있으면)
        {
            //shooter.gun.ammoRemain += ammo; //플레이어가 가진 총알 총량을 ammo(30발)만큼 늘린다,  멀티게임에서는 주석처리 (싱글게임용 코드)
            shooter.gun.photonView.RPC("AddAmmo",RpcTarget.All, ammo); //아이템을 먹으면 gun스크립트에 있는 AddAmmo함수를 불러와 호스트와 클라이언트 모두 ammoRemain 변수를 ammo(30발)만큼 늘린다.
                                                                       //해당 스크립트가 불러와지는 이유는 AddAmmo함수가 [PunRPC]로 선언되었기 때문이다.
        }
        //target에 탄알을 추가하는 처리
        //Debug.Log("탄알이 증가 했다: " + ammo);
        //Destroy(gameObject); //먹는 순간 자기 자신(ammo오브젝트)이 삭제된다, 싱글게임용 코드 (멀티게임이라 주석처리)
        PhotonNetwork.Destroy(gameObject); //모든 클라이언트 (호스트 포함)에서 아이템 삭제
    }
}
