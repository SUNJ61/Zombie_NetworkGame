using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
public class ItemSpawner : MonoBehaviourPun
{
    public GameObject[] items; //������ ������ (1. ź������, 2. hpȸ��)
    public Transform playerTr; //�÷��̾� ��ġ (�÷��̾� ��ġ ������ ������ ����)
    public float maxDist = 5.0f; //�÷��̾� ��ġ���� �������� ��ġ�� �ִ� �Ÿ�
    public float timeBetSpawnMax = 7.0f; //���� �ִ� ����
    public float timeBetSpawnMin = 2.0f; //���� �ּ� ����
    private float timeBetSpawn; //���� ����
    private float lastSpawnTime; //������ ���� ����
    void Start()
    {
        //playerTr = GameObject.FindWithTag("Player").transform; //��Ƽ �÷��̿����� �÷��̾� Tr�� ���� �ʴ´�.
        timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax); //�������� 2~7�� ���̿� ����
        lastSpawnTime = 0f;
    }
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return; //������ Ŭ���̾�Ʈ������ �������� �����Ѵ�.

        //if(Time.time >= lastSpawnTime + timeBetSpawn && playerTr != null) //�������ݸ��� �ߵ� / playerTr != null : ��ȿ�� �˻� �̱� ���ӿ��� if����
        if (Time.time >= lastSpawnTime + timeBetSpawn) //��Ƽ���ӿ����� �÷��̾� ��ġ�� ���� �� ���� ��������.
        {
            lastSpawnTime = Time.time;
            timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
            Spawn();
        }
    }
    void Spawn()
    {
        Vector3 spawnPos = GetRandomPointOnNavMesh(Vector3.zero, maxDist); // �÷��̾� ��ǥ �������� nav mesh���� ������ ��ȯ �޴´�. ������ PlayerTr.position ��Ƽ������ �߽��� �������� ������ ��ȯ
        spawnPos += Vector3.up * 0.5f; //���鿡�� �������� 0.5��ŭ �ø���.
        GameObject selectedItem = items[Random.Range(0, items.Length)]; //�������� �ϳ��� �������� ����.
        #region �̱۰��ӿ����� ������ ����,���� �ڵ�
        //GameObject item = Instantiate(selectedItem, spawnPos, Quaternion.identity); // ���������� ������ġ�� ��ȯ�Ѵ�.

        //Destroy(item, 5.0f);//������ �������� 5�� �Ŀ� �Ҹ��Ų��.
        #endregion
        #region ��Ƽ ���ӿ����� ������ ����,���� �ڵ�
        GameObject item = PhotonNetwork.Instantiate(selectedItem.name, spawnPos, Quaternion.identity); //��� Ŭ���̾�Ʈ���� ������ ��ȯ, Instantiate�� ù��° ���ڰ� string�̱⶧���� selectedItem.name�� ���
                                                                                                       //name�� �ش� ������Ʈ�� �̸��� ������Ƽ�� ��ȯ�� �ش�.
                                                                                                       //4��° ���ڴ� �׷����� ��� ��ȯ�ϴ� ���, ���⼭�� ������� �ʾ���.
        StartCoroutine(DestroyAfter(item, 5.0f));
        #endregion
    }
    IEnumerator DestroyAfter(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay); //5�� �Ŀ�
        if(target != null) //�ı����� �ʾҴٸ�
        {
            PhotonNetwork.Destroy(target); // ��� Ŭ���̾�Ʈ���� �������� �����Ѵ�.
        }
    }
    private Vector3 GetRandomPointOnNavMesh(Vector3 center, float distance) //nav mesh���� ������ ��ġ�� ��ȯ�ϴ� �޼���, center�� �߽����� �Ÿ� �ݰ� �ȿ��� ������ ��ġ�� ã��
    {
        Vector3 randomPos = Random.insideUnitSphere * distance + center; //insideUnitSphere�� �������� 1�� �� �ȿ��� ������ ���� ��ȯ�ϴ� ������Ƽ�̴�.
        NavMeshHit hit; //nav mesh���ø��� ��� ������ �����ϴ� ����
        NavMesh.SamplePosition(randomPos, out hit, distance, NavMesh.AllAreas); //randompos���� distance�ݰ� �ȿ� ���� ����� nav mesh���� ������ ã�´�.
        return hit.position; //������ ã�� ���� ��ȯ�Ѵ�.
    }
}
