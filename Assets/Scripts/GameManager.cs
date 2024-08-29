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
    public GameObject playerPrefab; //������ �÷��̾� ������

    private int score = 0; //���� ���� ���� 
    public bool isGameOver
    {
        get; private set;
    }
    void Awake()
    {
        //���� �̱��� ������Ʈ�� �� �ٸ� GameManager ������Ʈ�� �ִٸ� (������ Ŭ���̾�Ʈ���� �Ͼ�� ���̴�.)
        if(instance != null)
        {
            Destroy(gameObject); //�ڽ��� �ı� 
        }
    }
    private void Start()
    {
        //FindObjectOfType<PlayerHealth>().onDeath += EndGame; //�÷��̾� ĳ������ ��� �̺�Ʈ �߻��� ���ӿ��� (��Ƽ���� �ڵ尡 ���� �̿ϼ��̶� �ϴ� �ּ�ó��)
        Vector3 randomSpawnPos = Random.insideUnitSphere * 5f; //�÷��̾ ������ ��ġ
        randomSpawnPos.y = 0f; //���� ��ǥ�� �������� �̵���Ų��.
        PhotonNetwork.Instantiate(playerPrefab.name, randomSpawnPos, Quaternion.identity); //��� Ŭ���̾�Ʈ���� �÷��̾� �������� ������ǥ�� ��ȯ�Ѵ�.
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
        if(stream.IsWriting) //������ �������� �����Ͱ� �۽� �Ǿ��ٸ�
        {
            stream.SendNext(score); //���� ������ �۽��Ѵ�. (���� ���� ������Ʈ�� ������ �Ծ ���ھ ������Ʈ �� ���� �۽��Ͽ� �ٸ� Ŭ���̾�Ʈ�� ����)
        }
        else //�ٸ� ��Ʈ��ũ ������ �������� ���ŵǾ��ٸ� 
        {
            score = (int)stream.ReceiveNext(); //�ٸ� �÷��̾��� ������ �����Ѵ�. (�ٸ� �÷��̾��� ���� ������Ʈ�� ������ �Ծ ���ھ ������Ʈ �� ���� ����)
            UIManager.Instance.UpdateScoreText(score); //����ȭ �ؼ� ���� ������ UI�� ǥ���Ѵ�.
        }
    }
}
