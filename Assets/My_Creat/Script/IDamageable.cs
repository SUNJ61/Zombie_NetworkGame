using UnityEngine;

public interface IDamageable
{ //인터페이스 자체가 느슨한 커플링이다.
  //이 매소드를 받는 클래스가 어떤 클래스인지 검사를 하지 않는다. (꼼꼼한 검사 x)
    void OnDamage(float damage,Vector3 hitPosition, Vector3 hitNormal); //누구라도 데미지를 입으면 발생하는 함수
}
