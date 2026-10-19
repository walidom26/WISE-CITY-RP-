using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnderdogCity
{
    public class Pickup : MonoBehaviour
    {
        [Header("إعدادات إعادة ظهور العنصر/السلاح - وايز سيتي")]
        public float TimeToRespawn; // الوقت اللازم لظهور العنصر مجدداً
        protected float TimeLeft;
        protected bool Pickable;

        // Start is called before the first frame update
        void Start()
        {
            Deactivate();
            Debug.Log("تم تفعيل نظام التقاط الأسلحة والعناصر بنجاح!");
        }

        // إلغاء تفعيل العنصر (إخفاؤه بعد التقاطه)
        private void Deactivate()
        {
            Pickable = false;
            TimeLeft = TimeToRespawn;
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(false);
        }

        // تفعيل العنصر (إظهاره مجدداً في الماب)
        private void Activate()
        {
            Pickable = true;
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(true);
        }

        // Update is called once per frame
        void Update()
        {
            // عد تنازلي لوقت إعادة ظهور العنصر
            if (TimeLeft > 0)
                TimeLeft -= Time.deltaTime;
            
            if (!Pickable && TimeLeft <= 0)
                Activate();
        }

        // عند دخول اللاعب منطقة التقاط العنصر
        void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponent<Player>();
            if (player != null && Pickable)
            {
                Deactivate();
                player.Pickup();
            }
        }

        void OnTriggerExit(Collider other)
        {
            
        }
    }
}
