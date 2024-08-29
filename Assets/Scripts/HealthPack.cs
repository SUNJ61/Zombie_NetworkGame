using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class HealthPack : MonoBehaviourPun, IItem
{
    public float health = 50f;
    public void Use(GameObject target)
    {
        LivingEntity life = target.GetComponent<LivingEntity>(); //아이템에 닿은 오브젝트의 LivingEntity를 컴포넌트를 가져온다. (상속된 스크립트 포함)

        if (life != null) //LivingEntity를 상속받은 오브젝트 라면
        {
            life.RestoreHealth(health); //체력을 회복 시킨다.
        }

        //Destroy(gameObject); //닿는 순간 자기자신 삭제, 싱글게임용 코드
        PhotonNetwork.Destroy(gameObject); //모든 클라이언트에서 삭제
    }
}
