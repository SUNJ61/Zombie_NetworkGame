using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon; //
public class EnemySpawner : MonoBehaviourPun, IPunObservable
{
    public Enemy enemyPrefab;
    public Transform[] spawnPoints;

    public float damageMax = 40f; //최대 공격력
    public float damageMin = 20f; //최소 공격력

    public float healthMax = 200f;
    public float healthMin = 100f;

    public float speedMax = 3f;
    public float speedMin = 1.0f;
    public Color strongEnemyColor = Color.red; 
    //강한 적 AI가 가지게 될 피부색
    private List<Enemy> enemies = new List<Enemy>();
    private int enemyCount = 0;
    private int wave; //현재 웨이브
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting) //현재 적의 수와 웨이브의 데이터를 송신 
        {
            stream.SendNext(enemies.Count); //현재 남은 적의 수의 데이터를 네트워크로 보낸다. (나의 로컬 캐릭터가 적을 죽이면 호스트의 적의 수 데이터의 변화가 생기므로)
            stream.SendNext(wave); //현재 웨이브를 네트워크로 보낸다. (나의 로컬 캐릭터가 마지막 적을 죽이면 호스트의 웨이브 데이터의 변화가 생기므로)
        }
        else //현재 적의 수와 웨이브의 데이터를 수신
        {
            enemyCount = (int)stream.ReceiveNext(); //현재 남은 적의 수의 데이터를 네트워크를 통해 수신한다. (다른 클라이언트의 로컬 캐릭터가 적을 죽이면 호스트의 데이터 변화가 생기므로)
            wave = (int)stream.ReceiveNext(); //현재 웨이브의 데이터를 네트워크를 통해 수신한다. (다른 클라이언트의 로컬 캐릭터가 마지막 적을 죽이면 호스트의 데이터 변화가 생기므로)
        }
    }
    private void Awake()
    {
        PhotonPeer.RegisterType(typeof(Color), 128, ColorSerialization.SerializeColor, ColorSerialization.DeserializeColor); //좀비 색상이 직렬화 되어서 서버로 저장되었다가 다시 역직렬화 되어서 color값으로 되돌아온다. 
    }
    void Update()
    {
        if (PhotonNetwork.IsMasterClient) //마스터 클라이언트만 적을 직접 생성한다. (다른 클라이언트는 호스트가 생성한 결과를 동기화 해서 받아온다.)
        {
            if (GameManager.Instance != null && GameManager.Instance.isGameOver)
                return;
            //적을 모두 물리친 경우 다음 스판을 실행 
            if (enemies.Count <= 0)
            {
                SpawnWave();
            }
        }
        UpdateUI();
    }
    void UpdateUI()
    {
        if (PhotonNetwork.IsMasterClient) //마스터 클라이언트라면 
        {
            UIManager.Instance.UpdateWaveText(wave, enemies.Count); //호스트는 변경된 데이터를 통해 직접 UI를 갱신
        }
        else //클라이언트 라면 (호스트가 아니라면)
        {
            UIManager.Instance.UpdateWaveText(wave, enemyCount); //클라이언트는 데이터를 업데이트 불가능 (리스트 개수가 줄어들지 않음.) 보내준 enemyCount와 Wave를 통해 UI업데이트
        }
    }
     void SpawnWave() //현재 웨이브에 맞춰서  적 생성
    {
        wave++;          //현재 웨이브 * 1.5를 반올림 한 수만큼 생성
        int spawnCount = Mathf.RoundToInt(wave * 1.5f);
        for (int i = 0; i < spawnCount; i++)
        {
            //적의 강도(세기)를 0에서 100 사이에서 랜덤으로 결정 
            float enemyIntensity = Random.Range(0f, 1f);
            CreateEnemy(enemyIntensity);
        }
    }
    void CreateEnemy(float intensity) //적을 생성하고 추적할 대상을 할당
    {
        //intensity를 기반으로 적의 능력치가 결정 
        float health = Mathf.Lerp(healthMin, healthMax, intensity);
        float damage = Mathf.Lerp(damageMin, damageMax, intensity);
        float speed = Mathf.Lerp(speedMin, speedMax, intensity);
        //intensity를 기반 으로 하얀색과 enemyStrength 사이에서 적의 피부색이 결정 
        Color skinColor = Color.Lerp(Color.white, strongEnemyColor, intensity);
        //생성할 위치를 랜덤으로 결정 
        Transform spawnPoint = spawnPoints[Random.Range(0,spawnPoints.Length)];
        //적 프리팹으로 부터 적생성 
        //Enemy enemy = Instantiate(enemyPrefab,spawnPoint.position,spawnPoint.rotation); //싱글게임에서 적 생성, 멀티게임 코드로 바꾸기 위해 주석처리
        GameObject createdEnemy = PhotonNetwork.Instantiate(enemyPrefab.gameObject.name, spawnPoint.position, spawnPoint.rotation); //멀티게임에서 적 생성 코드, 모든 클라이언트에서 적 소환
        Enemy enemy = createdEnemy.GetComponent<Enemy>();
        enemy.photonView.RPC("Setup",RpcTarget.All,health,damage,speed,skinColor); //모든 클라이언트에 생성된 적의 특성을 해당 변수들로 바꾼다. 이곳 컬러에서 오류가 발생..?

        //enemy.Setup(health,damage,speed,skinColor); //싱글 게임에서 적 특성 적용 코드.
        enemies.Add(enemy); //생성한 적을 리스트에 추가
        enemy.onDeath += () => enemies.Remove(enemy);
        //enemy.onDeath += () => Destroy(enemy.gameObject, 10f); //싱글 게임에서 적이 죽었을 때 적 오브젝트를 삭제하는 코드
        enemy.onDeath += () => StartCoroutine(DestroyAfter(enemy.gameObject, 10.0f)); //멀티 게임에서 적이 죽었을 때 적 오브젝트를 삭제하는 코드
        enemy.onDeath += () => GameManager.Instance.AddScore(100);
    }
    IEnumerator DestroyAfter(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);
        if(target != null)
        {
            PhotonNetwork.Destroy(target);
        }
    }
}
