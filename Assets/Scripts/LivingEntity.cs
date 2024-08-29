using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using Photon.Pun;
//PunPRC의 작동방식이 어떻게 발생하는지 이해가 안되면 OnDamage와 ApplyUpdatedHealth 함수의 설명을 참고하여 이해하기.
public class LivingEntity : MonoBehaviourPun,IDamageable
{
    public float startingHealth = 100f;  //시작 체력
    public float health {  get; protected set; } //현재 체력
    public bool dead { get; protected set; } // 사망 상태
    public event Action onDeath; //사망 시 발동 할 이벤트 

    [PunRPC] //호스트(방장) -> 모든 클라이언트 순서 방향으로 사망상태를 동기화 하는 메소드. 호스트에서만 해당 변수를 관리한다.
    public void ApplyUpdatedHealth(float newhealth, bool newDead) // 이 함수의 실행은 클라이언트 컴퓨터에서 일어난다. 매개변수만 호스트 컴퓨터에서 전달 받음. 
    {
        health = newhealth; //호스트에서 받아온 값 newhealth
        dead = newDead; //호스트에서 받아온 값 newDead
    }

     //생명체가 활성화 될 때 상태를 리셋        
    protected virtual  void OnEnable()
    {             //물려 받을 가상 메서드 
        dead = false; //사망하지 않은 상태로
        health = startingHealth; // 체력을 시작체력으로 초기화

    }
    [PunRPC] //호스트에서 단독 실행 되고 호스트를 통해 다른 클라이언트에서 일괄 실행된다.
    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal) //즉, 호스트 화면에서 어떤 오브젝트가 해당 함수를 불러온다면 호스트에서 먼저 데미지 계산후 클라이언트에 전파하는 방식이다.
    {
        if (PhotonNetwork.IsMasterClient) //자기자신이 방장이거나 호스트라면
        {
            health -= damage; //호스트 컴퓨터에서 데미지 만큼 체력 감소를 실행.
            photonView.RPC("ApplyUpdatedHealth", RpcTarget.Others, health, dead); //호스트 컴퓨터에서 ApplyUpdatedHealth 함수를 실행시켜 호스트에 있는 health, dead의 값을 클라이언트로 동기화 시킨다.
            photonView.RPC("OnDamage", RpcTarget.Others, damage, hitPoint, hitNormal); //호스트 컴퓨터가 클라이언트에서 OnDamage함수를 호출하라고 명령한다. 위에서 업데이트 된 health, dead값을 통해 아래의 if문을 실행.
        } //RpcTarget.Others는 나를 제외한 모든 클라이언트를 의미한다. 여기서는 호스트만 해당 코드를 발생기키기에 호스트를 제외한 모두 즉, 다른 모든 클라이언트를 의미한다.
        if (health<=0 && !dead)
        {
            Die();
        }
      
    }
    //체력을 회복 하는 기능
    [PunRPC]
    public virtual void RestoreHealth(float newHealth)
    {
        if(dead)
        {     //이미 죽었다면 체력을 회복 할 수 없음
            return;
        }

        if (PhotonNetwork.IsMasterClient) //호스트인 경우라면
        {
            health += newHealth; //호스트의 체력이 추가된다.
            photonView.RPC("ApplyUpdatedHealth", RpcTarget.Others, health, dead); //서버에서 클라이언트로 동기화 한다.
            photonView.RPC("RestoreHealth", RpcTarget.Others, newHealth); //다른 네트워크 클라이언트도 RestoreHealth함수를 호출하도록 한다.
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
//LivingEntity클래스는  IDamageable을 상속하므로 OnDamage()메서드를 반드시 구현
