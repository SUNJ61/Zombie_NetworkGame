using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum State { READY = 0, EMPTY, RELOADING } // ���� ���¸� �߻��غ񰡵�/źâ�̺����./���������̴� 3���� ��������.
    public State state { get; private set; } //���¸� �� Ŭ�������� ������Ƽ�θ� ������Ʈ �����ϴ�.
    public Transform fireTransform;
    public ParticleSystem muzzleFlashEffect;
    public ParticleSystem shellEjectEffect;
    public AudioClip shotClip;
    public AudioClip reloadClip;

    private LineRenderer lineRenderer;
    private AudioSource gunAudioPlayer;

    private float fireDistance = 50f; //�����Ÿ�
    private float lastFireTime; //������ ���� �� �ð��� ��� ����

    public float damage = 25f; //���ݷ�
    public float timeBetFire = 0.12f; //�Ѿ� �߻� ����
    public float reloadTime = 1.8f; // ������ �ҿ� �ð�

    public int ammoRemain = 100; //������ �ִ� ���� �Ѿ� ��
    public int magCapacity = 25; //źâ �뷮
    public int magAmmo; //���� źâ�� ���� �Ѿ� ��
    void Awake()
    {
        fireTransform = transform.GetChild(3).GetComponent<Transform>();
        muzzleFlashEffect = transform.GetChild(4).GetComponent<ParticleSystem>();
        shellEjectEffect = transform.GetChild(5).GetComponent<ParticleSystem>();
        lineRenderer = transform.GetComponent<LineRenderer>();
        gunAudioPlayer = GetComponent<AudioSource>();

        lineRenderer.positionCount = 2; //����� ���� 2���� ����
        lineRenderer.enabled = false;
    }
    private void OnEnable()
    {
        magAmmo = magCapacity; //���� źâ�� �Ѿ˼��� źâ�� ũ��(�Ѿ� �ƽ���)���� �ٲ۴�.
        state = State.READY;
        lastFireTime = 0f;
    }
    public void Fire() //�߻� �õ�
    {
        if(state == State.READY && Time.time >= lastFireTime + timeBetFire) //�߻��غ� �Ϸ�Ǿ���, �߻��� 0.12�ʰ� ��������
        {
            lastFireTime = Time.time;
            Shot();
        }
    }
    void Shot() //���� �߻� ó��
    {
        //Ray ray = new Ray(fireTransform.position, fireTransform.forward); //ray���� ���
        RaycastHit hit;
        Vector3 hitPosition = Vector3.zero;
        if(Physics.Raycast(fireTransform.position, fireTransform.forward, out hit, fireDistance))
        {//�浹�� ���濡 ���Ե� ��ũ��Ʈ���� IDamageable ����� Ŭ���� ���۳�Ʈ�� �������⸦ �õ��Ѵ�.
            IDamageable target = hit.collider.GetComponent<IDamageable>(); 
            if(target != null)//������ ���۳�Ʈ �߿� IDamageable�� ��ӹ��� Ŭ������ �����Ѵٸ�
            {
                target.OnDamage(damage, hit.point, hit.normal); //������ OnDamage�� ������� �������� �ش�.
            }

            hitPosition = hit.point; //������ ���۳�Ʈ �߿� IDamageable�� ��ӹ��� Ŭ������ �������� ������ ��ġ�� �����Ѵ�.
        }
        else
        {//����ĳ��Ʈ�� �ٸ� ��ü�� ���� �ʾҴٸ�
            hitPosition = fireTransform.position + fireTransform.forward * fireDistance;
            //�ѱ� ��ġ���� �������� �ִ��Ÿ���ŭ ������ ��ġ�� ���� ��ġ�� ����Ѵ�.
        }
        //StartCoroutine(shotEffect(hitPosition)); �Ƹ� ���� ������?
    }
    IEnumerator shotEffect(Vector3 hitPosition)
    {
        muzzleFlashEffect.Play();
        shellEjectEffect.Play();
        gunAudioPlayer.PlayOneShot(shotClip);
        lineRenderer.SetPosition(0, fireTransform.position); //���η����� ���������� �ѱ��� ��ġ�� �ٲ۴�.
        lineRenderer.SetPosition(1, hitPosition); //���η������� ������ �Է����� ���� ���� ��ġ�� �ٲ۴�.
        lineRenderer.enabled = true;

        yield return new WaitForSeconds(0.03f);

        lineRenderer.enabled = false;
        muzzleFlashEffect.Stop();
        shellEjectEffect.Stop();
    }
    public bool Reload() //������ �õ�
    {
        if (state == State.RELOADING || ammoRemain <= 0 || magAmmo >= magCapacity)
        {//�̹� ������ ���̰ų�, ���� �Ѿ��� ���ų�, źâ�� �̹� �Ѿ��� ������ ������
            return false; // false�� ��ȯ�Ѵ�.
        }
        //������ �Լ��� Ż������ �ʾ����� ���ε� �Ѵ�.

        StartCoroutine(ReloadRoutine());
        return true;
    }
    IEnumerator ReloadRoutine() //���� ������ ó�� ����
    {
        state = State.RELOADING;
        gunAudioPlayer.PlayOneShot(reloadClip);

        yield return new WaitForSeconds(reloadTime);

        int ammoToFill = magCapacity - magAmmo; //źâ�� ä�� ź���� �Ի� (ä�� �Ѿ˼� = �ִ� �Ѿ˼� - ���� �� �Ѿ˼�)
        if(ammoRemain < ammoToFill)//���� ��ü �Ѿ��� ä�� �Ѿ˼����� ���� ��
        {
            ammoToFill = ammoRemain; //ä�� �Ѿ� ���� ���� ��ü �Ѿ� ���� �ȴ�.
        }
        magAmmo += ammoToFill; //���� �Ѿ˿� ä�� �Ѿ˼��� ���Ѵ�. (źâ�� ä��)
        ammoRemain -= ammoToFill; //���� ��ü �Ѿ� ������ ������ �Ѿ� ���� ����.

        state = State.READY;
    }
    void Update()
    {
        
    }
}
