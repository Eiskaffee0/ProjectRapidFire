using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
namespace Scripts.Player2
{
    public class Player2 : MonoBehaviour
    {
        Rigidbody rb;

        public enum MoveState
        {
            Idle,
            Run,
            Crouch,
            CrouchMove,
            Jump
        }
        public enum AimState
        {
            Forward,
            Up,
            Down
        }

        [Header("플레이어 설정")]
        public float moveSpeed = 3.0f;
        public float jumpPower = 5f;

        [Header("지면체크")]
        public float groundCheckDistance = 1f;
        public LayerMask GroundLayer;
        public bool isGrounded;

        // 상태 변수
        public MoveState currentMoveState = MoveState.Idle;
        public AimState currentAimState = AimState.Forward;

        private float horizontalInput = 0f;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            CheckGround();
            StateInput();
            UpdateAimState();
        }

        void FixedUpdate()
        {
            ApplyMovement();
        }

        void CheckGround()
        {
            Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
            isGrounded = Physics.Raycast(rayOrigin, Vector3.down, groundCheckDistance, GroundLayer);
            Debug.DrawRay(rayOrigin, Vector3.down * groundCheckDistance, isGrounded ? Color.green : Color.red);
        }

        void StateInput()
        {
            horizontalInput = 0f;
            if (Input.GetKey(KeyCode.A)) horizontalInput = -1f;
            if (Input.GetKey(KeyCode.D)) horizontalInput = 1f;

            if (!isGrounded)
            {
                currentMoveState = MoveState.Jump;
                return;
            }

            // 점프 입력
            if (Input.GetKeyDown(KeyCode.K))
            {
                Jump();
                return;
            }

            // 앉기 입력 
            if (Input.GetKey(KeyCode.S))
            {
                if (horizontalInput != 0f)
                {
                    currentMoveState = MoveState.CrouchMove;
                }
                else
                {
                    currentMoveState = MoveState.Crouch;
                }
            }
            // 서서 이동
            else if (horizontalInput != 0f)
            {
                currentMoveState = MoveState.Run;
            }
            // 아무것도 안 누름
            else
            {
                currentMoveState = MoveState.Idle;
            }
        }

        void UpdateAimState()
        {
            bool isCrouching = (currentMoveState == MoveState.Crouch || currentMoveState == MoveState.CrouchMove);

            if (!isCrouching && Input.GetKey(KeyCode.W))
            {
                currentAimState = AimState.Up;
            }
            else if (Input.GetKey(KeyCode.S) && !isGrounded)
            {
                currentAimState = AimState.Down;
            }
            else
            {
                currentAimState = AimState.Forward;
            }
        }

        void ApplyMovement()
        {
            float currentSpeed = moveSpeed;
            if (currentMoveState == MoveState.CrouchMove) currentSpeed = moveSpeed * 0.5f;

            // 이동이 없는 상태면 입력값을 0으로 강제
            float finalInput = horizontalInput;
            if (currentMoveState == MoveState.Idle || currentMoveState == MoveState.Crouch)
            {
                finalInput = 0f;
            }

            rb.velocity = new Vector3(finalInput * currentSpeed, rb.velocity.y, 0);
        }

        void Jump()
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, 0);
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            currentMoveState = MoveState.Jump;
        }
    }
}
