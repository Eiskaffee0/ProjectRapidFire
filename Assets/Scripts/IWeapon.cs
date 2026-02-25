using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Players;

namespace Scripts.Interfaces
{
    public interface IWeapon
    {
        void Fire(Player2 player, Transform firePoint, Player2.AimState aimState, float facingDirection);
    }
}
