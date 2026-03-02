using UnityEngine;
using Scripts.Players;
using Scripts.Weapons;
using Scripts.Managers; // 사운드 매니저를 부르기 위해 추가!

namespace Scripts.Items
{
    public class WeaponItem : MonoBehaviour
    {
        public enum WeaponType
        {
            Pistol,
            HeavyMachinegun,
            Shotgun,
            RocketLauncher
        }

        [Header("아이템 설정")]
        public WeaponType weaponType = WeaponType.HeavyMachinegun;

        [Header("충돌 대상 설정")]
        public LayerMask playerLayer;

        [Header("사운드 설정")]
        public AudioClip pickupSound; // 인스펙터에서 넣을 획득 효과음

        private void OnTriggerEnter(Collider other)
        {
            // Tag 대신 LayerMask로 플레이어인지 검사
            if (((1 << other.gameObject.layer) & playerLayer) != 0)
            {
                Player player = other.GetComponent<Player>();

                if (player != null)
                {
                    GiveWeaponToPlayer(player);
                    Debug.Log($"플레이어가 {weaponType}을(를) 획득했습니다!");

                    // 상자가 파괴되기 직전에 사운드 매니저에게 효과음 재생을 명령
                    if (SoundManager.Instance != null && pickupSound != null)
                    {
                        SoundManager.Instance.PlaySFX(pickupSound);
                    }

                    Destroy(gameObject); // 상자 파괴
                }
            }
        }

        private void GiveWeaponToPlayer(Player player)
        {
            switch (weaponType)
            {
                case WeaponType.Pistol:
                    player.EquipWeapon(new Pistol());
                    break;
                case WeaponType.HeavyMachinegun:
                    player.EquipWeapon(new HMachinegun());
                    break;
            }
        }
    }
}