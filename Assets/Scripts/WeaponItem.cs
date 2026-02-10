
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Scripts
{
    public class WeaponItem : MonoBehaviour
    {
        public WeaponStrategy weaponToEquip; // 이 아이템이 줄 무기 프리팹

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                WeaponController player = other.GetComponent<WeaponController>();

                // 플레이어에게 무기 교체 요청
                player.EquipWeapon(weaponToEquip);

                // 아이템 사라짐 (Heavy Machine Gun!! 사운드 재생 후)
                Destroy(gameObject);
            }
        }
    }

}

