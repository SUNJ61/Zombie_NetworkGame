using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<GameManager>();
            return instance;
        }
    }
    public GameObject playerPrefab; //생성할 플레이어 프리팹

    private int score = 0; //현재 게임 점수 
    public bool isGameOver
    {
        get; private set;
    }
    void Awake()
    {
        //씬에 싱글턴 오브젝트가 된 다른 GameManager 오브젝트가 있다면 (각자의 클라이언트에서 일어나는 일이다.)
        if(instance != null)
        {
            Destroy(gameObject); //자신을 파괴 
        }
    }
    private void Start()
    {
        //FindObjectOfType<PlayerHealth>().onDeath += EndGame; //플레이어 캐릭터의 사망 이벤트 발생시 게임오버 (멀티게임 코드가 아직 미완성이라 일단 주석처리)
        Vector3 randomSpawnPos = Random.insideUnitSphere * 5f; //플레이어가 생성할 위치
        randomSpawnPos.y = 0f; //랜덤 좌표를 지면으로 이동시킨다.
        PhotonNetwork.Instantiate(playerPrefab.name, randomSpawnPos, Quaternion.identity); //모든 클라이언트에서 플레이어 프리팹을 랜덤좌표에 소환한다.
    }
    public void AddScore(int newScore)
    {
        if(!isGameOver)
        {
            score += newScore;
            UIManager.Instance.UpdateScoreText(score);
        }
    }
    public void EndGame()
    {
        isGameOver = true;
        UIManager.Instance.SetActiveGameOverUI(true);

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting) //로컬의 움직임의 데이터가 송신 되었다면
        {
            stream.SendNext(score); //나의 점수를 송신한다. (나의 로컬 오브젝트가 코인을 먹어서 스코어가 업데이트 된 값을 송신하여 다른 클라이언트에 전송)
        }
        else //다른 네트워크 유저의 움직임을 수신되었다면 
        {
            score = (int)stream.ReceiveNext(); //다른 플레이어의 점수를 수신한다. (다른 플레이어의 로컬 오브젝트가 코인을 먹어서 스코어가 업데이트 된 값을 수신)
            UIManager.Instance.UpdateScoreText(score); //동기화 해서 받은 점수를 UI로 표시한다.
        }
    }
}
