using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
public class ItemSpawner : MonoBehaviourPun
{
    public GameObject[] items; //생성할 아이템 (1. 탄약증가, 2. hp회복)
    public Transform playerTr; //플레이어 위치 (플레이어 위치 근접한 곳에서 생성)
    public float maxDist = 5.0f; //플레이어 위치에서 아이템이 배치될 최대 거리
    public float timeBetSpawnMax = 7.0f; //생성 최대 간격
    public float timeBetSpawnMin = 2.0f; //생성 최소 간격
    private float timeBetSpawn; //생성 간격
    private float lastSpawnTime; //마지막 생성 시점
    void Start()
    {
        //playerTr = GameObject.FindWithTag("Player").transform; //멀티 플레이에서는 플레이어 Tr을 잡지 않는다.
        timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax); //생성간격 2~7초 사이에 생성
        lastSpawnTime = 0f;
    }
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return; //마스터 클라이언트에서만 아이템을 생성한다.

        //if(Time.time >= lastSpawnTime + timeBetSpawn && playerTr != null) //생성간격마다 발동 / playerTr != null : 유효성 검사 싱글 게임에서 if조건
        if (Time.time >= lastSpawnTime + timeBetSpawn) //멀티게임에서는 플레이어 위치를 잡을 수 없어 제외했음.
        {
            lastSpawnTime = Time.time;
            timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
            Spawn();
        }
    }
    void Spawn()
    {
        Vector3 spawnPos = GetRandomPointOnNavMesh(Vector3.zero, maxDist); // 플레이어 좌표 기준으로 nav mesh위의 한점을 반환 받는다. 원래는 PlayerTr.position 멀티게임은 중심을 기준으로 아이템 소환
        spawnPos += Vector3.up * 0.5f; //지면에서 아이템을 0.5만큼 올린다.
        GameObject selectedItem = items[Random.Range(0, items.Length)]; //아이템중 하나를 무작위로 고른다.
        #region 싱글게임에서의 아이템 생성,삭제 코드
        //GameObject item = Instantiate(selectedItem, spawnPos, Quaternion.identity); // 고른아이템을 랜덤위치에 소환한다.

        //Destroy(item, 5.0f);//생성된 아이템을 5초 후에 소멸시킨다.
        #endregion
        #region 멀티 게임에서의 아이템 생성,삭제 코드
        GameObject item = PhotonNetwork.Instantiate(selectedItem.name, spawnPos, Quaternion.identity); //모든 클라이언트에서 아이템 소환, Instantiate의 첫번째 인자가 string이기때문에 selectedItem.name을 사용
                                                                                                       //name은 해당 오브젝트의 이름을 프로퍼티로 반환해 준다.
                                                                                                       //4번째 인자는 그룹으로 묶어서 소환하는 기능, 여기서는 사용하지 않았음.
        StartCoroutine(DestroyAfter(item, 5.0f));
        #endregion
    }
    IEnumerator DestroyAfter(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay); //5초 후에
        if(target != null) //파괴되지 않았다면
        {
            PhotonNetwork.Destroy(target); // 모든 클라이언트에서 아이템을 삭제한다.
        }
    }
    private Vector3 GetRandomPointOnNavMesh(Vector3 center, float distance) //nav mesh위에 랜덤한 위치를 반환하는 메서드, center를 중심으로 거리 반경 안에서 랜덤한 위치를 찾음
    {
        Vector3 randomPos = Random.insideUnitSphere * distance + center; //insideUnitSphere는 반지름이 1인 구 안에서 랜덤한 점을 반환하는 프로퍼티이다.
        NavMeshHit hit; //nav mesh샘플링의 결과 정보를 저장하는 변수
        NavMesh.SamplePosition(randomPos, out hit, distance, NavMesh.AllAreas); //randompos에서 distance반경 안에 가장 가까운 nav mesh위의 한점을 찾는다.
        return hit.position; //위에서 찾은 점을 반환한다.
    }
}
