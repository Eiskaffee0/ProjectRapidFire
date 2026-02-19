using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Scripts.Player2.Player2;
public class IWeapon : MonoBehaviour
{
    public interface IWeaponData
    {
        void Fire(Transform firePoint, AimState aimState, float facingDirection);
    }

}

