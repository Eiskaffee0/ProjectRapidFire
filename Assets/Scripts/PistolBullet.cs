using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Player2
{
    public class PistolBullet : MonoBehaviour
    {
        public float speed = 15f;
        public float lifeTime = 2f;

        private float timer = 0f;

        void OnEnable()
        {
            timer = 0f;            
        }
        void Update()
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);

            timer += Time.deltaTime;
            if (timer> lifeTime)
            {
                BulletPool.Instance.ReturnPistolBullet(gameObject);
            }
        }
    }

}

