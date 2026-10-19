using UnityEngine;
using System.Collections;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.SceneManagement;

namespace UnderdogCity
{
    public class NetworkConnectionManager : MonoBehaviourPunCallbacks
    {
        public Button BtnConnectMaster;
        public Button BtnConnectRoom;

        [Header("Server Info")]
        public string defaultServerName = "WISE-CITY-RP";
        public string defaultOwnerName = "mr walid";

        protected bool TriesToConnectToMaster;
        protected bool TriesToConnectToRoom;

        // Use this for initialization
        void Start()
        {
            DontDestroyOnLoad(this);
            TriesToConnectToMaster = false;
            TriesToConnectToRoom   = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (BtnConnectMaster != null)
                BtnConnectMaster.gameObject.SetActive(!PhotonNetwork.IsConnected && !TriesToConnectToMaster);
            if (BtnConnectRoom != null)
                BtnConnectRoom.gameObject.SetActive(PhotonNetwork.IsConnected && !TriesToConnectToMaster && !TriesToConnectToRoom);
        }

        public void OnClickConnectToMaster()
        {
            TriesToConnectToMaster = true;

            // ضبط إعدادات الاتصال واسم المالك للسيرفر
            PhotonNetwork.OfflineMode = false;
            PhotonNetwork.NickName = defaultOwnerName; // تعيين الاسم الافتراضي mr walid
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = "v1.0"; // إصدار اللعبة الخاصة بك

            if(!PhotonNetwork.OfflineMode)
                PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            TriesToConnectToMaster = false;
            Debug.Log("Connected to Master Server - WISE-CITY-RP!");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            TriesToConnectToMaster = false;
            TriesToConnectToRoom   = false;
            Debug.Log(cause);
        }

        public void OnClickConnectToRoom()
        {
            if (!PhotonNetwork.IsConnected)
                return;

            TriesToConnectToRoom = true;
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            base.OnJoinRandomFailed(returnCode, message);
            // إنشاء غرفة خاصة باسم المدينه WISE-CITY-RP وسعة 20 لاعب
            PhotonNetwork.CreateRoom(defaultServerName + " - Main Room", new RoomOptions { MaxPlayers = 20 });
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);
            Debug.Log(message);
            TriesToConnectToRoom = false;
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            TriesToConnectToRoom = false;

            Debug.Log("Connected to: " + defaultServerName + " | Owner: " + defaultOwnerName + " | Players: " + PhotonNetwork.CurrentRoom.PlayerCount);
            
            if(PhotonNetwork.IsMasterClient && SceneManager.GetActiveScene().name != "Main")
                PhotonNetwork.LoadLevel("Main");
        }
    }
}
