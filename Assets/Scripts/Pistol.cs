using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Scripts.Player2.Player2;
public class Pistol : IWeapon
{
    public GameObject PistolBulletPrefab;
    public void Fire(Transform firePoint, AimState aimState, float facingDirection)
    {
        Instantiate(PistolBulletPrefab, firePoint.position, firePoint.rotation);
        Debug.Log($"권총 발사! 조준: {aimState}, 방향: {facingDirection}");
    }
}

