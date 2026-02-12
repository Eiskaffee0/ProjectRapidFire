using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Scripts.Player2
{
    public class Playser2 : MonoBehaviour
    {
        Rigidbody rb;
        public enum MoveState //달리면서 쏘기 위해 상 하체 상태를 나눈다. 하체의 이동 상태
        {
            Idle,
            Run,
            Crouch,
            CrouchMove,
            Jump,
        }

        public enum AimState //상체의 조준 상태
        {
            Forward,
            Up,
            Down,
        }

        public float moveSpeed = 3.0f;


        public MoveState currentMoveState = MoveState.Idle;
        public AimState currentAimState = AimState.Forward;


        void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }


        void Update()
        {
            MoveInput();
            StateAction();

        }

        void MoveInput() //이동입력에 따른 상체와 하체 상태 결정, 여기서는 상태만 결정.
        {
            if (currentMoveState != MoveState.Jump) //점프중이 아닐 때 앉기와 앉은자세 이동
            {
                if (Input.GetKey(KeyCode.S))
                {
                    currentMoveState = MoveState.Crouch;
                }

                else if (Input.GetKey(KeyCode.S) && (Input.GetKey(KeyCode.A)) || (Input.GetKey(KeyCode.D)))
                {
                    currentMoveState = MoveState.CrouchMove;


                }

                else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) && currentMoveState != MoveState.CrouchMove)
                {
                    currentMoveState = MoveState.Run;
                }

                else
                {

                    currentMoveState = MoveState.Idle;
                }
            }

            if (Input.GetKey(KeyCode.W))
            {
                currentAimState = AimState.Up;
            }
            else if (Input.GetKey(KeyCode.S))  //점프중일때 S로 아래조준, 점프중이 아닐때는 정면 조준 유지
            {
                if (currentMoveState == MoveState.Jump)
                {
                    currentAimState = AimState.Down;
                }
                else
                {
                    currentAimState = AimState.Forward;
                }
            }
            else
            {
                currentAimState = AimState.Forward;
            }

            if (Input.GetKeyDown(KeyCode.K) && currentMoveState != MoveState.Jump)
            {
                currentMoveState = MoveState.Jump;

            }
        }

        void StateAction() //입력받은 상태를 기반으로 실제 행동을 실행
        {
            if (currentMoveState == MoveState.Run || currentMoveState == MoveState.Jump && currentMoveState != MoveState.CrouchMove)
            {
                if (Input.GetKey(KeyCode.A))
                {
                    moveSpeed = 3.0f;
                    transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
                }

                if (Input.GetKey(KeyCode.D))
                {
                    moveSpeed = 3.0f;
                    transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
                }
            }

            if (currentMoveState == MoveState.CrouchMove)
            {
                if (Input.GetKey(KeyCode.S) && (Input.GetKey(KeyCode.A)))
                {
                    moveSpeed = 2.0f;
                    transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
                }

                if (Input.GetKey(KeyCode.S) && (Input.GetKey(KeyCode.D)))
                {
                    moveSpeed = 2.0f;
                    transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
                }
            }

            if (currentMoveState == MoveState.Jump)
            {
                if (Input.GetKeyDown(KeyCode.K))
                {
                    Jump();
                }

            }
        }

        public void Jump()
        {
            rb.AddForce(Vector3.up, ForceMode.Impulse);
        }

    }
}




