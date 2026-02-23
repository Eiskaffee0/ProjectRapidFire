using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Player2
{
    public class PistolBullet : MonoBehaviour
    {
        public float speed = 15f;
        public float lifeTime = 2f;
        public float damage = 1f;

        private float timer = 0f;

        private bool isReturned = false;
        void OnEnable()
        {
            timer = 0f;
            isReturned = false;
        }
        void Update()
        {
            if (isReturned)
            {
                return;
            }
            

            transform.Translate(Vector3.right * speed * Time.deltaTime);

            timer += Time.deltaTime;
            if (timer> lifeTime)
            {
                ReturnToPool();
            }
        }
        public void OnTriggerEnter(Collider other)
        {
            if (isReturned)
            {
                return;
            }

            IDamageable damageableTarget = other.GetComponent<IDamageable>();

            if (damageableTarget != null)
            {
                damageableTarget.TakeDamage(damage);
                ReturnToPool();

            }
        }

        public void ReturnToPool()
        {
            isReturned = true;
            BulletPool.Instance.ReturnPistolBullet(gameObject);
        }
    }

}

