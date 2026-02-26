using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Interfaces;

namespace Scripts.Weapons
{
    public class EnemyBullet : MonoBehaviour
    {
        public float speed = 10f;
        public float lifeTime = 3f;
        public float damage = 1f;

        public float timer = 0f;

        private void Update()
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);

            timer += Time.deltaTime;
            if (timer > lifeTime)
            {
                Destroy(gameObject);
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            IDamageable damageableTarget = other.GetComponent<IDamageable>();

            if (damageableTarget != null)
            {
                damageableTarget.TakeDamage(damage);

                Destroy(gameObject);
            }
        }
    }
}

