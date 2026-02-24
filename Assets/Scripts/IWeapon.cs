using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Player2
{
    public interface IWeapon
    {
        void Fire(Player2 player, Transform firePoint, Player2.AimState aimState, float facingDirection);
    }
}
