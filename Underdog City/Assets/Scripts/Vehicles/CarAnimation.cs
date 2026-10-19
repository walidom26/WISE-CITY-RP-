using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnderdogCity
{
    public class CarAnimation : MonoBehaviour {

        [Header("إعدادات حركة الأصوات والأنيميشن - وايز سيتي")]
        [HideInInspector]
        public Animator Animator;
        [HideInInspector]
        public GameObject AnimEnterPosition;
        [HideInInspector]
        public GameObject AnimDrivePosition;
        [HideInInspector]
        public CarPhysics CarPhysics;
        [HideInInspector]
        public AudioSource AudioSource;
        
        [Header("مؤثرات الصوت")]
        public AudioClip OpenClip;  // صوت فتح باب السيارة
        public AudioClip CloseClip; // صوت إغلاق باب السيارة
        protected Rigidbody Rigidbody;

        // التهيئة عند تشغيل المركبة
        void Awake () {
            Animator = GetComponentInChildren<Animator>();
            
            Transform carMesh = transform.Find("CarMesh");
            if (carMesh != null)
            {
                Transform enterPos = carMesh.Find("AnimEnterPosition");
                if (enterPos != null) AnimEnterPosition = enterPos.gameObject;

                Transform drivePos = carMesh.Find("AnimDrivePosition");
                if (drivePos != null) AnimDrivePosition = drivePos.gameObject;
            }

            CarPhysics = GetComponent<CarPhysics>();
            AudioSource = GetComponent<AudioSource>();
            Rigidbody = GetComponent<Rigidbody>();

            Debug.Log("تم تفعيل نظام أنيميشن السيارات في مدينة WISE-CITY-RP بنجاح!");
        }

        void Update()
        {
            // تحديث صوت المحرك بناءً على سرعة المركبة
            if (AudioSource != null && Rigidbody != null)
            {
                AudioSource.pitch = 0.8f + Rigidbody.velocity.magnitude / 20f;
            }
        }
    }
}
