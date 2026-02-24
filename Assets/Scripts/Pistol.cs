using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Scripts.Player2
{
    public class Pistol : IWeapon
    {
        public float fireRate = 0.1f;
        public float nextFireTime = 0f;
        public void Fire(Player2 player,Transform firePoint, Player2.AimState aimState, float facingDirection)
        {
            if (Time.time < nextFireTime)
            {
                return;
            }

            nextFireTime = Time.time + fireRate;

            GameObject bullet = BulletPool.Instance.GetPistolBullet();
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = firePoint.rotation;

            Debug.Log("피스톨 발사!");
        }
    }
}

