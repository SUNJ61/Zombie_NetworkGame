using UnityEngine;

public interface IDamageable
{ //�������̽� ��ü�� ������ Ŀ�ø��̴�.
  //�� �żҵ带 �޴� Ŭ������ � Ŭ�������� �˻縦 ���� �ʴ´�. (�Ĳ��� �˻� x)
    void OnDamage(float damage,Vector3 hitPosition, Vector3 hitNormal); //������ �������� ������ �߻��ϴ� �Լ�
}
