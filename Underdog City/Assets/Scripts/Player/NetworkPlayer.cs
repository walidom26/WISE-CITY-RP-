using Photon.Pun;
using UnityEngine;

namespace UnderdogCity
{
    public class NetworkPlayer : MonoBehaviourPun, IPunObservable
    {
        protected Player Player;
        protected Vector3 RemotePlayerPosition;
        protected float RemoteLookX;
        protected float RemoteLookZ;
        protected float LookXVel;
        protected float LookZVel;

        private void Awake()
        {
            Player = GetComponent<Player>();

            // حذف سكريبت التحكم إذا لم يكن اللاعب هو الشخصية الخاصة بك (التحكم باللاعبين الآخرين عبر الشبكة)
            if (!photonView.IsMine && GetComponent<Controller>() != null)
                Destroy(GetComponent<Controller>());
        }

        public void Update()
        {
            if (photonView.IsMine)
                return;

            var LagDistance = RemotePlayerPosition - transform.position;

            // إذا كانت المسافة كبيرة جداً، قم بنقل اللاعب فوراً للموقع الجديد لتجنب التأخير
            if (LagDistance.magnitude > 5f)
            {
                transform.position = RemotePlayerPosition;
                LagDistance = Vector3.zero;
            }

            // تجاهل المسافة في المحور الرأسي (Y)
            LagDistance.y = 0;

            if (LagDistance.magnitude < 0.11f)
            {
                // اللاعب وصل تقريباً إلى الموقع المطلوب
                Player.Input.RunX = 0;
                Player.Input.RunZ = 0;
            }
            else
            {
                // اللاعب بحاجة للتحرك نحو الموقع
                Player.Input.RunX = LagDistance.normalized.x;
                Player.Input.RunZ = LagDistance.normalized.z;
            }

            // القفز إذا كان اللاعب البعيد أعلى من اللاعب الحالي
            Player.Input.Jump = RemotePlayerPosition.y - transform.position.y > 0.2f;

            // سلاسة التوجيه والنظر
            Player.Input.LookX = Mathf.SmoothDamp(Player.Input.LookX, RemoteLookX, ref LookXVel, 0.2f);
            Player.Input.LookZ = Mathf.SmoothDamp(Player.Input.LookZ, RemoteLookZ, ref LookZVel, 0.2f);
        }

        // مزامنة البيانات أونلاين عبر شبكة Photon
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // إرسال بيانات موقعك واتجاهك وحالتك للآخرين
                stream.SendNext(transform.position);
                stream.SendNext(Player.Input.LookX);
                stream.SendNext(Player.Input.LookZ);
                stream.SendNext(Player.State);
            }
            else
            {
                // استقبال بيانات اللاعبين الآخرين
                RemotePlayerPosition = (Vector3)stream.ReceiveNext();
                RemoteLookX = (float)stream.ReceiveNext();
                RemoteLookZ = (float)stream.ReceiveNext();
                var state = (Player.PlayerState)stream.ReceiveNext();

                // دخول السيارة (Transition)
                if (state == Player.PlayerState.TRANSITION && Player.State == Player.PlayerState.NORMAL)
                    Player.StartCoroutine(Player.EnterCarAnimation());
                
                // الخروج من السيارة (Transition)
                if (state == Player.PlayerState.TRANSITION && Player.State == Player.PlayerState.IN_CAR)
                    Player.StartCoroutine(Player.ExitCarAnimation());

                Player.State = state;
            }
        }
    }
}
