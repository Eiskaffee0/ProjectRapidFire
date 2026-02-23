using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Scripts.Player2.Player2;


public interface IWeapon
{
    void Fire(Transform firePoint, AimState aimState, float facingDirection);
}



