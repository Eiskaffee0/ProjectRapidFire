
using Scripts.Interfaces;
using Scripts.Managers;
using Scripts.Players;
using Scripts.Pools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Scripts.Weapons
{
    public class HMachinegun : IWeapon
    {
        public float fireRate = 0.06f;
        public int burstCount = 4;

        private bool isFiring = false;

        private AudioClip fireSound;

        public HMachinegun()
        {
            // 헤비머신건 사운드 파일을 불러옵니다.
            fireSound = Resources.Load<AudioClip>("Sounds/HMG_Fire");
        }

        public void Fire(Player player, Transform firePoint, Player.AimState aimState, float facingDirection)
        {
            if (isFiring)
            {
                return;
            }

            // 헤비머신건 총소리 재생
            if (fireSound != null)
            {
                SoundManager.Instance.PlaySFX(fireSound);
            }

            player.StartCoroutine(BurstFire(player, firePoint));
        }

        private IEnumerator BurstFire(Player player, Transform firePoint)
        {
            isFiring = true;

            for (int i = 0; i < burstCount; i++)
            {
                if (firePoint == null)
                {
                    break;
                }

                GameObject bullet = BulletPool.Instance.GetPistolBullet();

                bullet.transform.position = new Vector3(firePoint.position.x, firePoint.position.y, player.transform.position.z);
                bullet.transform.rotation = firePoint.rotation;

                Debug.Log($"헤비머신건 {i + 1}발째 발사");

                yield return new WaitForSeconds(fireRate);
            }

            isFiring = false;
        }
    }

}

