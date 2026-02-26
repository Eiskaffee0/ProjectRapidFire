using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Pools;
using Scripts.Players;
using Scripts.Interfaces;
namespace Scripts.Weapons

{
    public class Pistol : IWeapon
    {
        public float fireRate = 0.1f;
        public float nextFireTime = 0f;
        public void Fire(Player player,Transform firePoint, Player.AimState aimState, float facingDirection)
        {
            if (Time.time < nextFireTime)
            {
                return;
            }

            nextFireTime = Time.time + fireRate;

            GameObject bullet = BulletPool.Instance.GetPistolBullet();
            bullet.transform.position = new Vector3(firePoint.position.x, firePoint.position.y, player.transform.position.z);
            bullet.transform.rotation = firePoint.rotation;

            Debug.Log("피스톨 발사!");
        }
    }
}

