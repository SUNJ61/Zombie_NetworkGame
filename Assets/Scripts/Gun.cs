using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Gun : MonoBehaviourPun, IPunObservable //�������̽��� ��� �޴´�.
{                    //�߻��غ��, źâ�� �� , ������ ��
    public enum State { Ready, Empty,Reloading}
    public State state { get; private set; }
    public Transform fireTransform; //ź�� �߻� ��ġ
    public ParticleSystem muzzleFlashEffect;//�ѱ�ȭ�� ȿ��
    public ParticleSystem shellEjectEffect; //ź�ǹ��� ȿ��
    private LineRenderer lineRenderer;
    private AudioSource gunAudioPlayer;
    public AudioClip shotClip;
    public AudioClip reloadClip;
    public float damage = 25f; //���ݷ�
    private float fireDistance = 50f; //�����Ÿ� 
    public int ammoRemain = 100; //����  ��ü ź��
    public int magCapacity = 25; //źâ �뷮
    public int magAmmo;//���� źâ�� ���� �ִ� ź�� 
    public float timeBetFire = 0.12f; //ź�� �߻� ����
    public float reloadTime = 1.8f; // ������ �ҿ� �ð� 
    private float lastFireTime; // ���� ���������� �߻��� ����

    void Awake()
    {
        gunAudioPlayer = GetComponent<AudioSource>();
        lineRenderer = GetComponent<LineRenderer>();
        //��� �� ���� �ΰ��� ���� 
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
    }
    private void OnEnable()
    {
        magAmmo = magCapacity;
        state = State.Ready;
        lastFireTime = 0f;
    }
    public void Fire() //�߻� �õ�, shot�Լ��� �ҷ����� ���� ������ �ɾ�� �Լ�
    {
        if (state == State.Ready && Time.time >= lastFireTime + timeBetFire)
        {
            lastFireTime = Time.time;
            Shot();
        }
    }
    private void Shot() // ���� �߻� ó��
    {
        RaycastHit hit;
        //Ray ray = new Ray(fireTransform.position,fireTransform.forward);
        Vector3 hitPosition = Vector3.zero;
        if(Physics.Raycast(fireTransform.position, fireTransform.forward, out hit, fireDistance))
        {
            //�浹�� �������� ���� IDamageable ������Ʈ �������⸦ �õ� �Ѵ�.
            IDamageable target = hit.collider.GetComponent<IDamageable>();
            if(target != null)
            {
                //������ OnDamage  �Լ��� ������� ���濡 ����� �ֱ� 
                target.OnDamage(damage, hit.point, hit.normal);
            }
            //���̰� �浹�� ��ġ ���� 
            hitPosition = hit.point;
        }
        else
        {
            //���̰� �ٸ� ��ü�� �浹 ���� �ʾҴٸ�
            // ź���� �ִ� ���� �Ÿ����� ���ư��� ����  ��ġ�� �浹 ��ġ�� ���
            hitPosition = fireTransform.position + fireTransform.forward * fireDistance;
            //lineRenderer.SetPosition(1, ray.GetPoint(50f));
        }
        StartCoroutine(ShotEffect(hitPosition));
        photonView.RPC("ShotProcessOnServer", RpcTarget.MasterClient); //���� �߻�ó���� ȣ��Ʈ(������ Ŭ���̾�Ʈ)������ �Ͼ��.
                                                                       //��, ȣ��Ʈ�� ���� ��� �����͸� ��ȭ��Ű��, Ŭ���̾�Ʈ������ ui��ȭ, ���η������߻�, ȿ������ �����Ϳ� ���þ��� ó���� �Ѵ�.
        magAmmo--;
        if(magAmmo <= 0)
        {
            state = State.Empty;
        }

    }
    [PunRPC] //ȣ��Ʈ���� ������ �߻� ó��
    private void ShotProcessOnServer()
    {
        RaycastHit hit;
        Vector3 hitPosition = Vector3.zero;
        if (Physics.Raycast(fireTransform.position, fireTransform.forward, out hit, fireDistance))
        {
            IDamageable target = hit.collider.GetComponent<IDamageable>();
            if (target != null)
            {
                target.OnDamage(damage, hit.point, hit.normal);
            }
            hitPosition = hit.point;
        }
        else
        {
            hitPosition = fireTransform.position + fireTransform.forward * fireDistance;
        }
        photonView.RPC("ShotEffectProcessOnClient", RpcTarget.All, hitPosition); //���� �ش� �Լ� ������ �ʾ��� (����Ʈ ó���� Ŭ���̾�Ʈ �Ӹ��ƴ϶� ȣ��Ʈ ��� ��µǾ�� �ϹǷ� All�� ����ߴ�.)
    }
    [PunRPC]
    private void ShotEffectProcessOnClient(Vector3 hitPos) //ShotProcessOnServer�Լ����� hitPosition �����͸� ���� �޾� �ڷ�ƾ�� ������.
    {
        StartCoroutine(ShotEffect(hitPos));
    }
   IEnumerator ShotEffect(Vector3 hitPosition)
    {
       
        muzzleFlashEffect.Play();
        shellEjectEffect.Play();
        gunAudioPlayer.PlayOneShot(shotClip);
        //���� �������� �ѱ��� ��ġ 
        lineRenderer.SetPosition(0,fireTransform.position);
        //���� ������ �Է����� ���� �浹 ��ġ 
        lineRenderer.SetPosition(1,hitPosition);
        lineRenderer.enabled = true;
        yield return new WaitForSeconds(0.03f);

        lineRenderer.enabled = false;

    }
    public bool Reload() //������ �õ� 
    {
        if(state == State.Reloading|| ammoRemain <=0 || magAmmo >= magCapacity)
        {
            // �̹� ������ ���̰ų�  ���� ź���� ���ų� źâ�� ź���� �̹� ���� �� ��� 
            //������ �� �� ����. 
            return false;
        }
        StartCoroutine(ReloadRoutine());
        return true;
    }
    IEnumerator ReloadRoutine() //���� ������ ó���� ����
    {
        state = State.Reloading;
        gunAudioPlayer.PlayOneShot(reloadClip);


        yield return new WaitForSeconds(reloadTime);
        // źâ�� ä�� ź���� ��� 
        int ammoToFill = magCapacity - magAmmo;
        // źâ�� ä���� �� ź���� ���� ź�� ���� ��Ÿ��
        // ä���� �� ź�˼��� ���� ź�˼��� ���߾ ���� 
        if(ammoRemain<ammoToFill)
        {
            ammoToFill = ammoRemain;
        }
        //źâ�� ä��
        magAmmo += ammoToFill;
        //���� ź�˿��� źâ�� ä�� ��ŭ ź���� ��
        ammoRemain -= ammoToFill;

        state = State.Ready;
    }
    [PunRPC]
    public void AddAmmo(int ammo)
    {
        ammoRemain += ammo;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) //�ֱ������� �ڵ� ����Ǵ� ����ȭ �޼ҵ��̴�.  (stream = ��Ʈ��ũ , info = �߰��Ǵ� ����)
    {
        if(stream.IsWriting) //�۽�, ���� ������Ʈ�� �����͸� �۽��Ѵ�. (���⼭�� �Ѱ� ���õ� ������)
        {
            stream.SendNext(ammoRemain); //���� �������ִ� ���� �Ѿ��� ������ ������ ammoRemain�� ��Ʈ��ũ�� ���� ������. (�۽�)
            stream.SendNext(magAmmo); //���� ������ �Ѿ��� ������ ������ magAmmo�� ��Ʈ��ũ�� ���� ������. (�۽�)
            stream.SendNext(state); //���� ���� ���� (Ready, Empty,Reloading)�� �������� state�� ��Ʈ��ũ�� ���� ������. (�۽�)
        }
        if(stream.IsReading) //����, �ٸ� ��Ʈ��ũ ������ �����͸� ���� �޴´�. (���⼭�� �Ѱ� ���õ� ������)
        {
            ammoRemain = (int)stream.ReceiveNext(); //���� �������ִ� ���� �Ѿ��� ������ ������ ammoRemain�� ��Ʈ��ũ�� ���� �޴´�. (����)
            magAmmo = (int)stream.ReceiveNext(); //���� ������ �Ѿ��� ������ ������ magAmmo�� ��Ʈ��ũ�� ���� �޴´�. (����)
            state = (State)stream.ReceiveNext(); //���� ���� ���� (Ready, Empty,Reloading)�� �������� state�� ��Ʈ��ũ�� ���� �޴´�. (����)
        }
    }
}
