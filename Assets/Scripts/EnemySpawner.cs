using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon; //
public class EnemySpawner : MonoBehaviourPun, IPunObservable
{
    public Enemy enemyPrefab;
    public Transform[] spawnPoints;

    public float damageMax = 40f; //�ִ� ���ݷ�
    public float damageMin = 20f; //�ּ� ���ݷ�

    public float healthMax = 200f;
    public float healthMin = 100f;

    public float speedMax = 3f;
    public float speedMin = 1.0f;
    public Color strongEnemyColor = Color.red; 
    //���� �� AI�� ������ �� �Ǻλ�
    private List<Enemy> enemies = new List<Enemy>();
    private int enemyCount = 0;
    private int wave; //���� ���̺�
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting) //���� ���� ���� ���̺��� �����͸� �۽� 
        {
            stream.SendNext(enemies.Count); //���� ���� ���� ���� �����͸� ��Ʈ��ũ�� ������. (���� ���� ĳ���Ͱ� ���� ���̸� ȣ��Ʈ�� ���� �� �������� ��ȭ�� ����Ƿ�)
            stream.SendNext(wave); //���� ���̺긦 ��Ʈ��ũ�� ������. (���� ���� ĳ���Ͱ� ������ ���� ���̸� ȣ��Ʈ�� ���̺� �������� ��ȭ�� ����Ƿ�)
        }
        else //���� ���� ���� ���̺��� �����͸� ����
        {
            enemyCount = (int)stream.ReceiveNext(); //���� ���� ���� ���� �����͸� ��Ʈ��ũ�� ���� �����Ѵ�. (�ٸ� Ŭ���̾�Ʈ�� ���� ĳ���Ͱ� ���� ���̸� ȣ��Ʈ�� ������ ��ȭ�� ����Ƿ�)
            wave = (int)stream.ReceiveNext(); //���� ���̺��� �����͸� ��Ʈ��ũ�� ���� �����Ѵ�. (�ٸ� Ŭ���̾�Ʈ�� ���� ĳ���Ͱ� ������ ���� ���̸� ȣ��Ʈ�� ������ ��ȭ�� ����Ƿ�)
        }
    }
    private void Awake()
    {
        PhotonPeer.RegisterType(typeof(Color), 128, ColorSerialization.SerializeColor, ColorSerialization.DeserializeColor); //���� ������ ����ȭ �Ǿ ������ ����Ǿ��ٰ� �ٽ� ������ȭ �Ǿ color������ �ǵ��ƿ´�. 
    }
    void Update()
    {
        if (PhotonNetwork.IsMasterClient) //������ Ŭ���̾�Ʈ�� ���� ���� �����Ѵ�. (�ٸ� Ŭ���̾�Ʈ�� ȣ��Ʈ�� ������ ����� ����ȭ �ؼ� �޾ƿ´�.)
        {
            if (GameManager.Instance != null && GameManager.Instance.isGameOver)
                return;
            //���� ��� ����ģ ��� ���� ������ ���� 
            if (enemies.Count <= 0)
            {
                SpawnWave();
            }
        }
        UpdateUI();
    }
    void UpdateUI()
    {
        if (PhotonNetwork.IsMasterClient) //������ Ŭ���̾�Ʈ��� 
        {
            UIManager.Instance.UpdateWaveText(wave, enemies.Count); //ȣ��Ʈ�� ����� �����͸� ���� ���� UI�� ����
        }
        else //Ŭ���̾�Ʈ ��� (ȣ��Ʈ�� �ƴ϶��)
        {
            UIManager.Instance.UpdateWaveText(wave, enemyCount); //Ŭ���̾�Ʈ�� �����͸� ������Ʈ �Ұ��� (����Ʈ ������ �پ���� ����.) ������ enemyCount�� Wave�� ���� UI������Ʈ
        }
    }
     void SpawnWave() //���� ���̺꿡 ���缭  �� ����
    {
        wave++;          //���� ���̺� * 1.5�� �ݿø� �� ����ŭ ����
        int spawnCount = Mathf.RoundToInt(wave * 1.5f);
        for (int i = 0; i < spawnCount; i++)
        {
            //���� ����(����)�� 0���� 100 ���̿��� �������� ���� 
            float enemyIntensity = Random.Range(0f, 1f);
            CreateEnemy(enemyIntensity);
        }
    }
    void CreateEnemy(float intensity) //���� �����ϰ� ������ ����� �Ҵ�
    {
        //intensity�� ������� ���� �ɷ�ġ�� ���� 
        float health = Mathf.Lerp(healthMin, healthMax, intensity);
        float damage = Mathf.Lerp(damageMin, damageMax, intensity);
        float speed = Mathf.Lerp(speedMin, speedMax, intensity);
        //intensity�� ��� ���� �Ͼ���� enemyStrength ���̿��� ���� �Ǻλ��� ���� 
        Color skinColor = Color.Lerp(Color.white, strongEnemyColor, intensity);
        //������ ��ġ�� �������� ���� 
        Transform spawnPoint = spawnPoints[Random.Range(0,spawnPoints.Length)];
        //�� ���������� ���� ������ 
        //Enemy enemy = Instantiate(enemyPrefab,spawnPoint.position,spawnPoint.rotation); //�̱۰��ӿ��� �� ����, ��Ƽ���� �ڵ�� �ٲٱ� ���� �ּ�ó��
        GameObject createdEnemy = PhotonNetwork.Instantiate(enemyPrefab.gameObject.name, spawnPoint.position, spawnPoint.rotation); //��Ƽ���ӿ��� �� ���� �ڵ�, ��� Ŭ���̾�Ʈ���� �� ��ȯ
        Enemy enemy = createdEnemy.GetComponent<Enemy>();
        enemy.photonView.RPC("Setup",RpcTarget.All,health,damage,speed,skinColor); //��� Ŭ���̾�Ʈ�� ������ ���� Ư���� �ش� ������� �ٲ۴�. �̰� �÷����� ������ �߻�..?

        //enemy.Setup(health,damage,speed,skinColor); //�̱� ���ӿ��� �� Ư�� ���� �ڵ�.
        enemies.Add(enemy); //������ ���� ����Ʈ�� �߰�
        enemy.onDeath += () => enemies.Remove(enemy);
        //enemy.onDeath += () => Destroy(enemy.gameObject, 10f); //�̱� ���ӿ��� ���� �׾��� �� �� ������Ʈ�� �����ϴ� �ڵ�
        enemy.onDeath += () => StartCoroutine(DestroyAfter(enemy.gameObject, 10.0f)); //��Ƽ ���ӿ��� ���� �׾��� �� �� ������Ʈ�� �����ϴ� �ڵ�
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
