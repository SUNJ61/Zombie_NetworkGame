using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; //����Ƽ�� ���� ��Ʈ��ũ ���۳�Ʈ
using Photon.Realtime; //���� ���� ���� ���̺귯��
using UnityEngine.UI;
/*
������ ����(���� ����)�� Mach Makin �� ���� ��� 
*/
public class LobbyManager : MonoBehaviourPunCallbacks //MonoBehaviour�� ����ϸ鼭 ����Ŭ���忡�� �����ϴ� �ݹ��Լ��� ���� ����Ͽ� ����Ѵ�.
{
    private string gameVersion = "1"; //���� ����
    public Text connectionInfoText; //��Ʈ��ũ ���� ǥ��
    public Button joinButton; //�� ���� ��ư, �游��� ��ư
    void Start()
    {
        PhotonNetwork.GameVersion = gameVersion; //���ӿ� �ʿ��� ����(���ӹ���) ����
        PhotonNetwork.ConnectUsingSettings(); //������ ������ ������ ���� ���� �õ�, ������ ���� ���� ���� ������ �ȴ�.

        joinButton.interactable = false; //�� ���ӹ�ư ��� ��Ȱ��ȭ (�� ������ �Ұ����� �����̱� ������ ��Ȱ��ȭ)
        connectionInfoText.text = "������ ������ ������........"; //���� �õ� ������ �ؽ�Ʈ�� ǥ��
    }
    public override void OnConnectedToMaster() //������ ���� ���� ������ �ڵ� ����Ǵ� �Լ�.
    {
        joinButton.interactable = true; // ������ ���� �ߴٸ� ��ư ��ȣ�ۿ��� Ȱ��ȭ
        connectionInfoText.text = "�¶��� : ������ ������ �����........";
    }
    public override void OnDisconnected(DisconnectCause cause) //������ ���� ���� ���н� �ڵ����� �Ǵ� �Լ�
    {
        joinButton.interactable = false; //�� ���ӹ�ư ��Ȱ��ȭ (�� ������ �Ұ����� �����̱� ������ ��Ȱ��ȭ)
        connectionInfoText.text = "�������� : ������ ������ ������� ����........";
    }
    public void Connect() //�� ������ �õ��Ѵ�. Join ��ư�� ������ �� ȣ��Ǵ� �Լ��̴�.
    {
        joinButton.interactable = false; //��ư�� �ѹ� ���� ��, �� ���ӹ�ư ��Ȱ��ȭ (��ư�� ������ ���� �ߺ� ������ ���� ���� ��Ȱ��ȭ)
        if(PhotonNetwork.IsConnected) //������ ������ ���� ���̶��
        {
            connectionInfoText.text = "�뿡 ����........";
            PhotonNetwork.JoinRandomRoom(); //�ƹ� �濡�� ���� -> �̰��� ���� ��ġ����ŷ�̶�� �θ���.
        }
        else //������ ������ ���� ���� �ƴ϶�� (������ ������ ���� �� �־� ��� �˻��Ѵ�.)
        {
            connectionInfoText.text = "�������� : ������ ������ ������� ����........";
            PhotonNetwork.ConnectUsingSettings(); //������ ������ ������ �õ��� �Ѵ�.
        }
    }
    public override void OnJoinRandomFailed(short returnCode, string message) //�� ���� ��� ���� �� ������ ������ ��� �ڵ� ����Ǵ� �Լ�
    {
        connectionInfoText.text = "�� ���� ����, ���ο� �� ����........";
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 4 }); //�ִ� 4�� ���� ������ �� ���� �����Ѵ�. ��������������� �����ϸ� ���� ������ Ŭ���̾�Ʈ�� ȣ��Ʈ ������ �ô´�.
    }//������ �� ����� Ȯ���ϴ� ����� ������ �����Ƿ� ���� �̸��� �Է����� �ʰ� null�� �Է�
    public override void OnJoinedRoom() //�뿡 ���� �Ϸ�� ��� �ڵ� ����Ǵ� �Լ�
    {
        connectionInfoText.text = "�� ���� ����";
        PhotonNetwork.LoadLevel("Main"); //��� �� �����ڰ� Main���� �ε��ϰ� �Ѵ�.
    }
}
