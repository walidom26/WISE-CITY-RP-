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
        [Header("أزرار الواجهة المعربة")]
        public Button BtnConnectMaster;
        public Button BtnConnectRoom;

        [Header("نصوص الأزرار والرسائل")]
        public Text txtMasterBtn; // نص زر الاتصال بالسيرفر
        public Text txtRoomBtn;   // نص زر دخول المدينة
        public Text txtStatus;    // نص حالة الاتصال

        [Header("إعدادات السيرفر والمالك")]
        public string defaultServerName = "وايز سيتي - WISE CITY RP";
        public string defaultOwnerName = "mr walid";

        protected bool TriesToConnectToMaster;
        protected bool TriesToConnectToRoom;

        void Start()
        {
            DontDestroyOnLoad(this);
            TriesToConnectToMaster = false;
            TriesToConnectToRoom   = false;

            // تحديث نصوص الأزرار باللغة العربية
            if (txtMasterBtn != null) txtMasterBtn.text = "الاتصال بالخادم الرئيسي";
            if (txtRoomBtn != null) txtRoomBtn.text = "دخول مدينة وايز سيتي";
            if (txtStatus != null) txtStatus.text = "مرحباً بك في مدينة وايز سيتي RP";
        }

        void Update()
        {
            if (BtnConnectMaster != null)
                BtnConnectMaster.gameObject.SetActive(!PhotonNetwork.IsConnected && !TriesToConnectToMaster);
            if (BtnConnectRoom != null)
                BtnConnectRoom.gameObject.SetActive(PhotonNetwork.IsConnected && !TriesToConnectToMaster && !TriesToConnectToRoom);
        }

        // الاتصال بالخادم الرئيسي
        public void OnClickConnectToMaster()
        {
            TriesToConnectToMaster = true;

            if (txtStatus != null) txtStatus.text = "جاري الاتصال بالخادم الرئيسي...";

            PhotonNetwork.OfflineMode = false;
            PhotonNetwork.NickName = defaultOwnerName; // اسم المالك
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = "v1.0";

            if(!PhotonNetwork.OfflineMode)
                PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            TriesToConnectToMaster = false;
            Debug.Log("تم الاتصال بنجاح بخادم وايز سيتي!");
            if (txtStatus != null) txtStatus.text = "تم الاتصال بنجاح! جاهز لدخول المدينة.";
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            TriesToConnectToMaster = false;
            TriesToConnectToRoom   = false;
            if (txtStatus != null) txtStatus.text = "انقطع الاتصال بالخادم. أعد المحاولة.";
        }

        // دخول غرف اللعب
        public void OnClickConnectToRoom()
        {
            if (!PhotonNetwork.IsConnected)
                return;

            TriesToConnectToRoom = true;
            if (txtStatus != null) txtStatus.text = "جاري البحث عن سيرفر متاح...";
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            base.OnJoinRandomFailed(returnCode, message);
            if (txtStatus != null) txtStatus.text = "جاري إنشاء سيرفر جديد باسم المدينة...";
            
            // إنشاء الغرفة بروابط وسعة 20 لاعب
            PhotonNetwork.CreateRoom(defaultServerName + " - السيرفر الرئيسي", new RoomOptions { MaxPlayers = 20 });
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        { base.OnCreateRoomFailed(returnCode, message);
            if (txtStatus != null) txtStatus.text = "فشل إنشاء السيرفر: " + message;
            TriesToConnectToRoom = false;
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            TriesToConnectToRoom = false;

            if (txtStatus != null) txtStatus.text = "تم الانضمام بنجاح! جاري تحميل المدينة...";

            if(PhotonNetwork.IsMasterClient && SceneManager.GetActiveScene().name != "Main")
                PhotonNetwork.LoadLevel("Main");
        }
    }
}
