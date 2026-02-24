using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Scripts.Player2
{
    public class FireDirection : MonoBehaviour
    {
        public Player2 player;

        [Header("회전속도 설정")]
        public float rotationSpeed = 600f;
        public float currentAngle = 0f;
        void Update()
        {
            if (player == null)
            {
                return;
            }

            RotateArm();
        }

        void RotateArm()
        {
            float targetAngle = 0f;

            if (player.currentAimState == Player2.AimState.Up)
            {
                targetAngle = 180f;
            }

            else if (player.currentAimState == Player2.AimState.Down)
            {
                targetAngle = 0f;
            }

            else
            {
                targetAngle = 90f;
            }
            
            currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);

            transform.localRotation = Quaternion.Euler(0, 0, currentAngle);
         
        }
    }
}

