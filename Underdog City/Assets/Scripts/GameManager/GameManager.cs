using UnityEngine;
using System.Collections;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace UnderdogCity
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        [Header("UC Game Manager")]

        [HideInInspector]
        public Player LocalPlayer;

        [Header("Server Info")]
        public string serverOwner = "mr walid"; // تعيين المالك الأساسي للسيرفر

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
            
            // تسجيل صلاحيات المالك الأساسي في سجل السيرفر
            Debug.Log("Server Initialized. Owner & Highest Admin: " + serverOwner);

            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.InstantiateSceneObject("Car", new Vector3(0, 1, 20), Quaternion.identity);
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
