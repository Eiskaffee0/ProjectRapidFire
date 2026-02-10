using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Scripts
{
    public abstract class WeaponStrategy : MonoBehaviour
    {
        [Header("무기 설정")]
        public float fireRate;
        public float lastFireTime;
        public Transform firePoint;
       
        public abstract void Fire();
    }

    public class Pistol : WeaponStrategy // 기본 권총 구현부
    {
        public GameObject PistolBullet;

        public override void Fire()
        {
            Instantiate(PistolBullet, firePoint.position, firePoint.rotation);
            
        }

    }

    public class HeavyMachineGun : WeaponStrategy //헤비머신건 구현부
    {
        public GameObject HeavyBullet;
        public int ammo = 200;

        public override void Fire()
        {
            Instantiate(HeavyBullet, firePoint.position, firePoint.rotation);
        }
    }

    public class ShotGun : WeaponStrategy //샷건 구현부
    {
        public GameObject ShotGunBullet;
        public int ammo = 50;

        public override void Fire()
        {
            Instantiate(ShotGunBullet, firePoint.position, firePoint.rotation);
        }


    }
}

