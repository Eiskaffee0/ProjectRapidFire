
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Scripts.Player2
{
    public class HMachinegun : IWeapon
    {
        public float fireRate = 0.06f;
        public int burstCount = 4;

        private bool isFiring = false;

        public void Fire(Player2 player, Transform firePoint, Player2.AimState aimState, float facingDirection)
        {
            if (isFiring)
            {
                return;
            }

            player.StartCoroutine(BurstFire(firePoint));
        }

        private IEnumerator BurstFire(Transform firePoint)
        {
            isFiring = true;

            for (int i = 0; i < burstCount; i++)
            {
                if (firePoint == null)
                {
                    break;
                }

                GameObject bullet = BulletPool.Instance.GetPistolBullet();

                bullet.transform.position = firePoint.position;
                bullet.transform.rotation = firePoint.rotation;

                Debug.Log($"헤비머신건 {i + 1}발째 발사");

                yield return new WaitForSeconds(fireRate);
            }

            isFiring = false;
        }
    }

}

