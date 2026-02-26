using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Players;

namespace Scripts.Interfaces
{
    public interface IWeapon
    {
        void Fire(Player player, Transform firePoint, Player.AimState aimState, float facingDirection);
    }
}
