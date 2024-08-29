using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; //유니티용 포톤 네트워크 컴퍼넌트
using Photon.Realtime; //포톤 서비스 관련 라이브러리
using UnityEngine.UI;
/*
마스터 서버(리슨 서버)와 Mach Makin 룸 접속 담당 
*/
public class LobbyManager : MonoBehaviourPunCallbacks //MonoBehaviour를 상속하면서 포톤클라우드에서 제공하는 콜백함수를 같이 상속하여 사용한다.
{
    private string gameVersion = "1"; //게임 버전
    public Text connectionInfoText; //네트워크 정보 표시
    public Button joinButton; //룸 접속 버튼, 방만들기 버튼
    void Start()
    {
        PhotonNetwork.GameVersion = gameVersion; //접속에 필요한 정보(게임버전) 설정
        PhotonNetwork.ConnectUsingSettings(); //설정한 정보로 마스터 서버 접속 시도, 게임의 버전 별로 따로 접속이 된다.

        joinButton.interactable = false; //룸 접속버튼 잠시 비활성화 (룸 접속이 불가능한 상태이기 때문에 비활성화)
        connectionInfoText.text = "마스터 서버에 접속중........"; //접속 시도 중임을 텍스트로 표시
    }
    public override void OnConnectedToMaster() //마스터 서버 접속 성공시 자동 실행되는 함수.
    {
        joinButton.interactable = true; // 서버에 접속 했다면 버튼 상호작용을 활성화
        connectionInfoText.text = "온라인 : 마스터 서버와 연결됨........";
    }
    public override void OnDisconnected(DisconnectCause cause) //마스터 서버 접속 실패시 자동실행 되는 함수
    {
        joinButton.interactable = false; //룸 접속버튼 비활성화 (룸 접속이 불가능한 상태이기 때문에 비활성화)
        connectionInfoText.text = "오프라인 : 마스터 서버와 연결되지 않음........";
    }
    public void Connect() //룸 접속을 시도한다. Join 버튼을 눌렀을 때 호출되는 함수이다.
    {
        joinButton.interactable = false; //버튼을 한번 누른 후, 룸 접속버튼 비활성화 (버튼을 여러번 눌러 중복 접속을 막기 위해 비활성화)
        if(PhotonNetwork.IsConnected) //마스터 서버에 접속 중이라면
        {
            connectionInfoText.text = "룸에 접속........";
            PhotonNetwork.JoinRandomRoom(); //아무 방에나 접속 -> 이것을 랜덤 매치메이킹이라고 부른다.
        }
        else //마스터 서버에 접속 중이 아니라면 (언제든 연결이 끊길 수 있어 계속 검사한다.)
        {
            connectionInfoText.text = "오프라인 : 마스터 서버와 연결되지 않음........";
            PhotonNetwork.ConnectUsingSettings(); //마스터 서버에 재접속 시도를 한다.
        }
    }
    public override void OnJoinRandomFailed(short returnCode, string message) //빈 룸이 없어서 랜덤 룸 참가에 실패한 경우 자동 실행되는 함수
    {
        connectionInfoText.text = "빈 방이 없음, 새로운 방 생성........";
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 4 }); //최대 4명 수용 가능한 빈 방을 생성한다. 리슨서버방식으로 동작하며 룸을 생성한 클라이언트가 호스트 역할을 맡는다.
    }//생성된 룸 목록을 확인하는 기능은 만들지 않으므로 룸의 이름을 입력하지 않고 null로 입력
    public override void OnJoinedRoom() //룸에 참가 완료된 경우 자동 실행되는 함수
    {
        connectionInfoText.text = "방 참가 성공";
        PhotonNetwork.LoadLevel("Main"); //모든 룸 참가자가 Main씬을 로드하게 한다.
    }
}
