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

        public enum MoveState { Idle, Run, Crouch, CrouchMove, Jump }
        public enum AimState { Forward, Up, Down }

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

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            CheckGround(); // 땅 체크 먼저
            HandleInput(); // 입력 처리 및 상태 결정
            UpdateAimState(); // 조준 상태 처리
        }

        // 물리 이동은 FixedUpdate에서 하는 것이 정석
        void FixedUpdate()
        {
            ApplyMovement();
        }

        // 1. 접지 체크 (가장 중요)
        void CheckGround()
        {
            // 발바닥보다 살짝 위(0.1f)에서 시작해서 아래로 쏜다.
            Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;

            // transform.up 대신 Vector3.down을 써야 캐릭터 회전과 상관없이 항상 아래로 쏨
            isGrounded = Physics.Raycast(rayOrigin, Vector3.down, groundCheckDistance, GroundLayer);

            // 디버그용 선 그리기
            Debug.DrawRay(rayOrigin, Vector3.down * groundCheckDistance, isGrounded ? Color.green : Color.red);
        }

        // 2. 입력 및 이동 상태 결정 (로직 정리)
        void HandleInput()
        {
            // 점프 중이라면 다른 상태로 변경 불가 (단, 착지하면 Idle로 초기화는 CheckGround나 여기서 처리)
            if (!isGrounded)
            {
                currentMoveState = MoveState.Jump;
                return; // 공중에서는 아래 로직 무시
            }

            // --- 지상 상태 로직 ---

            // 점프 입력
            if (Input.GetKeyDown(KeyCode.K))
            {
                Jump();
                return;
            }

            // 앉기 입력 (우선순위 높음)
            if (Input.GetKey(KeyCode.S))
            {
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                {
                    currentMoveState = MoveState.CrouchMove;
                }
                else
                {
                    currentMoveState = MoveState.Crouch;
                }
            }
            // 서서 이동
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                currentMoveState = MoveState.Run;
            }
            // 아무것도 안 누름
            else
            {
                currentMoveState = MoveState.Idle;
            }
        }

        // 3. 조준 상태 로직 (논리 오류 수정)
        void UpdateAimState()
        {
            // 앉거나 앉아 이동 중이 아닐 때만 위 조준 가능
            bool isCrouching = (currentMoveState == MoveState.Crouch || currentMoveState == MoveState.CrouchMove);

            if (!isCrouching && Input.GetKey(KeyCode.W))
            {
                currentAimState = AimState.Up;
            }
            // 점프 중에만 아래 조준 가능 (메탈슬러그 식) 혹은 그냥 아래 입력 시
            else if (Input.GetKey(KeyCode.S) && !isGrounded)
            {
                currentAimState = AimState.Down;
            }
            else
            {
                currentAimState = AimState.Forward;
            }
        }

        // 4. 물리 적용 (AddRelativeForce 대신 Velocity 사용 권장)
        void ApplyMovement()
        {
            // 이동 방향 결정 (-1: 왼쪽, 1: 오른쪽, 0: 정지)
            float xInput = 0;
            if (Input.GetKey(KeyCode.A)) xInput = -1;
            if (Input.GetKey(KeyCode.D)) xInput = 1;

            // 현재 속도 계산
            float currentSpeed = moveSpeed;
            if (currentMoveState == MoveState.CrouchMove) currentSpeed = moveSpeed * 0.5f; // 앉아 이동은 느리게

            // 이동이 없는 상태(Idle, Crouch)면 xInput을 0으로 만듦
            if (currentMoveState == MoveState.Idle || currentMoveState == MoveState.Crouch)
            {
                xInput = 0;
            }

            // [핵심] Rigidbody의 Velocity를 직접 조절해야 반응이 빠름 (Run & Gun 특성)
            // y축 속도(중력 영향)는 그대로 유지하고 x축만 변경
            rb.velocity = new Vector3(xInput * currentSpeed, rb.velocity.y, 0);
        }

        void Jump()
        {
            // 순간적인 힘! 기존 속도 초기화 후 점프
            rb.velocity = new Vector3(rb.velocity.x, 0, 0);
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            currentMoveState = MoveState.Jump;
        }
    }
}




