using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Scripts.Player2.Player2;

public class HMachinegun : IWeapon
{
    public GameObject HMBulletPrefab;
  public void Fire(Transform firePoint, AimState aimState, float facingDirection)
    {
        Debug.Log($"헤비 머신건 발사! 조준: {aimState}, 방향: {facingDirection}");
    }
}
  

