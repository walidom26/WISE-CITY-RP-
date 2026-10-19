using DitzeGames.MobileJoystick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnderdogCity
{
    public class Controller : MonoBehaviour
    {
        [Header("عناصر التحكم بالموبايل - وايز سيتي")]
        protected Joystick Joystick;
        protected Button JumpButton;
        protected Button ShootButton;
        protected Button CarButton;
        protected TouchField TouchField;
        protected Player Player;

        [Header("إعدادات السرعة والتصوير")]
        protected const float RotationSpeed = 10;

        // التحكم بالكاميرا
        public Vector3 CameraPivot;
        public float CameraDistance;
        protected float InputRotationX;
        protected float InputRotationY;

        protected Vector3 CharacterPivot;
        protected Vector3 LookDirection;

        protected Vector3 CameraVelocity;

        // التهيئة عند بداية التشغيل
        void Start()
        {
            Joystick = FindObjectOfType<Joystick>();
            var buttons = new List<Button>(FindObjectsOfType<Button>());
            JumpButton = buttons.Find(b => b != null && b.gameObject.name == "btnJump");
            ShootButton = buttons.Find(b => b != null && b.gameObject.name == "btnShoot");
            CarButton = buttons.Find(b => b != null && b.gameObject.name == "btnCar");
            TouchField = FindObjectOfType<TouchField>();
            Player = GetComponent<Player>();

            if (TouchField != null)
            {
                TouchField.UseFixedUpdate = true;
            }

            Debug.Log("تم تفعيل نظام تحكم الشخصية والكاميرا لسيرفر WISE-CITY-RP!");
        }

        // تحديث الفيزياء والتحكم
        void FixedUpdate()
        {
            if (Player == null) return;

            // حساب التدوير واللمس على الشاشة
            if (TouchField != null)
            {
                InputRotationX = InputRotationX + TouchField.TouchDist.x * RotationSpeed * Time.deltaTime % 360f;
                InputRotationY = Mathf.Clamp(InputRotationY - TouchField.TouchDist.y * RotationSpeed * Time.deltaTime, -88f, 88f);
            }

            // اتجاهات الشخصية للأمام واليسار
            var characterForward = Quaternion.AngleAxis(InputRotationX, Vector3.up) * Vector3.forward;
            var characterLeft = Quaternion.AngleAxis(InputRotationX + 90, Vector3.up) * Vector3.forward;

            // حساب اتجاه الجري والنظر
            float inputV = Input.GetAxisRaw("Vertical") + (Joystick != null ? Joystick.AxisNormalized.y : 0f);
            float inputH = Input.GetAxisRaw("Horizontal") + (Joystick != null ? Joystick.AxisNormalized.x : 0f);

            var runDirection = characterForward * inputV + characterLeft * inputH;
            LookDirection = Quaternion.AngleAxis(InputRotationY, characterLeft) * characterForward;

            switch (Player.State)
            {
                case Player.PlayerState.NORMAL:

                    // تعيين قيم مدخلات اللاعب المشاة
                    Player.Input.RunX = runDirection.x;
                    Player.Input.RunZ = runDirection.z;
                    Player.Input.LookX = LookDirection.x;
                    Player.Input.LookZ = LookDirection.z;
                    Player.Input.Jump = (JumpButton != null && JumpButton.Pressed) || Input.GetButton("Jump");

                    // إطلاق النار / التصويب
                    if (ShootButton != null && ShootButton.Pressed)
                    {
                        ShootButton.Pressed = false;
                        if (Camera.main != null)
                        {
                            var ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
                            RaycastHit hitInfo;
                            if (Physics.Raycast(ray, out hitInfo))
                            {
                                var hitPlayer = hitInfo.collider.GetComponent<Player>();
                                if (hitPlayer != null)
                                    hitPlayer.OnHit(ray.direction);
                            }
                        }
                    }

                    CharacterPivot = Quaternion.AngleAxis(InputRotationX, Vector3.up) * CameraPivot;

                    // إظهار زر ركوب السيارة عند الاقتراب من سيارة
                    if (CarButton != null)
                    {
                        CarButton.gameObject.SetActive(Player.NearestCar != null);
                    }

                    break;

                case Player.PlayerState.IN_CAR:

                    // تعيين إدخالات قيادة السيارة عند التواجد بداخلها
                    if (Player.NearestCar != null && Player.NearestCar.CarPhysics != null)
                    {
                        float carV = Input.GetAxisRaw("Vertical") + (Joystick != null ? Joystick.InputVector.y : 0f);
                        float carH = Input.GetAxisRaw("Horizontal") + (Joystick != null ? Joystick.InputVector.x : 0f);

                        Player.NearestCar.CarPhysics.Input.Forward = carV;
                        Player.NearestCar.CarPhysics.Input.Steer = carH;
                    }

                    // إظهار زر النزول من السيارة
                    if (CarButton != null)
                    {
                        CarButton.gameObject.SetActive(true);
                    }

                    // تتبع كاميرا السيارة
                    if (Player.NearestCar != null && Camera.main != null)
                    {
                        var target = Player.NearestCar.transform.position + (Player.NearestCar.transform.forward * -1.5f + Player.NearestCar.transform.up) * CameraDistance;
                        Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, target, ref CameraVelocity, 0.3f);
                        Camera.main.transform.LookAt(Player.NearestCar.transform.position + Player.NearestCar.transform.forward * 1.5f * CameraDistance, Vector3.up);
                    }
                    break;
            }

            // عند الضغط على زر السيارة (ركوب / خروج)
            if (CarButton != null && CarButton.Pressed)
            {
                CarButton.Pressed = false;
                Player.EnterCar();
            }
        }

        private void LateUpdate()
        {
            if (Player == null || Camera.main == null) return;

            // ضبط الكاميرا في حالة المشاة
            switch (Player.State)
            {
                case Player.PlayerState.NORMAL:
                case Player.PlayerState.TRANSITION:
                    Camera.main.transform.position = (transform.position + CharacterPivot) - LookDirection * CameraDistance;
                    Camera.main.transform.rotation = Quaternion.LookRotation(LookDirection, Vector3.up);
                    break;

                case Player.PlayerState.IN_CAR:
                    break;
            }
        }
    }
}
