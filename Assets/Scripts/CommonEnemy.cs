using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Scripts.Player2
{
    public class CommonEnemy : MonoBehaviour, IDamageable
    {
        public float hp = 1f;
        public void TakeDamage(float damage)
        {
            hp -= damage;
            Debug.Log($"{damage}만큼 피해를 입었습니다. 남은 체력{hp}");
            if (hp <= 0f)
            {
                Die();
            }
        }

        public void Die()
        {
            Debug.Log("기본몹 사망");
            Destroy(gameObject);
        }
    }
}


