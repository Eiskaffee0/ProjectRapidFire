using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Scripts.Player2
{
    public class DistructiableObj : MonoBehaviour, IDamageable
    {
        public float hp = 5f;

        public void TakeDamage(float damage)
        {
            hp -= damage;
            Debug.Log($"상자가 {damage}만큼 피해를 받았습니다. 남은 체력 {hp}");

            if (hp <= 0)
            {
                Die();
            }
        }

        public void Die()
        {
            Debug.Log("오브젝트 파괴");
            Destroy(gameObject);
        }
    }

}
