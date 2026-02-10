
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Scripts
{
    public class WeaponController : MonoBehaviour
    {
        public WeaponStrategy currentWeapon;
        public Transform weaponHolder;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.J) && currentWeapon != null)
            {
                currentWeapon.Fire();
            }
        }

        public void EquipWeapon(WeaponStrategy newWeaponPrefab)
        {
            if (currentWeapon != null)
            {
                Destroy(currentWeapon.gameObject);
            }

            // 2. 새 무기 생성 및 장착
            // (실제로는 오브젝트 풀링을 쓰는 게 좋지만 일단 Instantiate로 설명합니다)
            WeaponStrategy newWeapon = Instantiate(newWeaponPrefab, weaponHolder);

            // 3. 위치 및 회전 초기화
            newWeapon.transform.localPosition = Vector3.zero;
            newWeapon.transform.localRotation = Quaternion.identity;

            // 4. 현재 전략 교체
            currentWeapon = newWeapon;
        }
    }
}


