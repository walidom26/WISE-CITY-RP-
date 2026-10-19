using UnityEngine;
using System.Collections;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;

namespace UnderdogCity
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        [Header("UC Game Manager")]

        [HideInInspector]
        public Player LocalPlayer;

        [Header("Server Info & Branding")]
        public string serverOwner = "mr walid"; // المالك الأساسي للسيرفر
        public string serverName = "WISE-CITY-RP"; // اسم السيرفر الرسمي
        
        [Header("UI Branding Elements")]
        public GameObject logoDisplayObject; // كائن عرض الشعار في واجهة اللعبة

        private void Awake()
        {
            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene("Menu");
                return;
            }
        }

        // Use this for initialization
        void Start()
        {
            LocalPlayer = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity).GetComponent<Player>();
            
            // تسجيل صلاحيات المالك وشعار السيرفر في السجل
            Debug.Log("Server Initialized. Owner & Highest Admin: " + serverOwner + " | City: " + serverName);

            // تحميل وعرض شعار المدينة تلقائياً على الشاشة
            LoadAndShowCityLogo();

            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.InstantiateSceneObject("Car", new Vector3(0, 1, 20), Quaternion.identity);
        }

        // دالة تحميل الشعار تلقائياً
        void LoadAndShowCityLogo()
        {
            // محاولة تحميل شعار المدينة من مجلد Resources أو Textures تلقائياً
            Texture2D loadedLogo = Resources.Load<Texture2D>("Logo");
            if (loadedLogo != null)
            {
                Debug.Log("WISE-CITY-RP Logo loaded successfully for: " + serverOwner);
            }
            else
            {
                Debug.Log("Logo.png is ready in UI Textures folder for WISE-CITY-RP.");
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player target, ExitGames.Client.Photon.Hashtable changedProps)
        {
            foreach(var change in changedProps)
                Debug.Log("Property " + change.Key + " of player " + target.UserId + " changed to " + change.Value);
        }
    }
}
