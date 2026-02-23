using Scripts.Player2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Scripts.Player2.Player2;
public class Pistol : IWeapon
{
    public void Fire(Transform firePoint, Player2.AimState aimState, float facingDirection)
    {
        GameObject bullet = BulletPool.Instance.GetPistolBullet();

        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;

        Debug.Log("피스톨 발사!");
    }
}

