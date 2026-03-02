using Scripts.Interfaces;
using Scripts.Managers;
using Scripts.Players;
using Scripts.Pools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Scripts.Weapons

{
    public class Pistol : IWeapon
    {
        public float fireRate = 0.1f;
        public float nextFireTime = 0f;

        private AudioClip fireSound;
        public void Fire(Player player,Transform firePoint, Player.AimState aimState, float facingDirection)
        {

            if (fireSound != null)
            {
                SoundManager.Instance.PlaySFX(fireSound);
            }

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

        public Pistol()
        {
            // Resources/Sounds 폴더 안에 있는 PistolFire 사운드 파일을 불러와서 저장
            fireSound = Resources.Load<AudioClip>("Sounds/PistolFire");
        }
    }
}

