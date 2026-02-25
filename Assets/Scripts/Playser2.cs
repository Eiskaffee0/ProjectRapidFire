using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Scripts.Interfaces;
using Scripts.Weapons;
namespace Scripts.Players
{
    public class Player2 : MonoBehaviour, IDamageable
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
        [Header("플레이어 목숨 관련")]
        public int startingLives = 3;
        public int currentLives;

        [Header("지면체크 설정")]
        public float groundCheckDistance = 1f;
        public LayerMask GroundLayer;
        public bool isGrounded;

        [Header("무기 설정")]
        public Transform firePoint;
        public GameObject pistolBulletPrefab;

        [Header("무기 위치 설정")]
        public Transform rightSocket; // 캐릭터가 오른쪽 방향일 때 무기위치
        public Transform leftSocket; // 캐릭터가 왼쪽 방향일 때 무기위치
        public Transform weaponArm; // 현재 무기를 들고있는 팔

        // 상태 변수
        public MoveState currentMoveState = MoveState.Idle;
        public AimState currentAimState = AimState.Forward;

        // 이동상태 플래그
        private float horizontalInput = 0f;

        // 기본 지향 방향 플래그
        private float facingDirection = 1f;

        // 현재 장착 무기
        private IWeapon currentWeapon;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            EquipWeapon(new Pistol());
            currentLives = startingLives;
        }

        void Update()
        {
            CheckGround();
            StateInput();
            UpdateFacingDirection();
            UpdateAimState();
            InputFire();
        }

        void FixedUpdate()
        {
            ApplyMovement();
        }

        public void TakeDamage(float damage)
        {
            Die();
        }

        public void Die()
        {
            currentLives--;

            Debug.Log($"사망, 남은 목숨: {currentLives}");

            if (currentLives > 0) //목숨이 남아있다면 당연히 새로운 플레이어를 스폰
            {
                Debug.Log("게임매니저에게 새로운 플레이어의 스폰을 요청");
                Destroy(gameObject);
            }

            else
            {
                Debug.Log("게임 오버");
                Destroy(gameObject);
            }
        }

        void CheckGround()
        {
            RaycastHit hit;
            isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance, GroundLayer);
            Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, isGrounded ? Color.green : Color.red);
        }

        void StateInput()
        {
            horizontalInput = 0f;
            if (Input.GetKey(KeyCode.A)) horizontalInput = -1f;
            if (Input.GetKey(KeyCode.D)) horizontalInput = 1f;

            if (horizontalInput != 0f) // 이동키와 바라보는 방향의 동기화.
            {
                facingDirection = horizontalInput;
            }

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

        void UpdateFacingDirection()
        {
            if (horizontalInput > 0f)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);

                if (weaponArm != null && rightSocket != null && weaponArm.parent != rightSocket)
                {
                    weaponArm.SetParent(rightSocket);
                    weaponArm.localPosition = Vector3.zero;
                }
            }

            else if (horizontalInput < 0f)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);

                if (weaponArm != null && leftSocket != null && weaponArm != leftSocket)
                {
                    weaponArm.SetParent(leftSocket);
                    weaponArm.localPosition = Vector3.zero;
                }
            }
        }
        void ApplyMovement()
        {
            float currentSpeed = moveSpeed;
            if (currentMoveState == MoveState.CrouchMove)
            {
                currentSpeed *= 0.5f;
            }

            // 이동이 없는 상태면 입력값을 0으로 강제
            float finalInput = horizontalInput;
            if (currentMoveState == MoveState.Idle || currentMoveState == MoveState.Crouch)
            {
                finalInput = 0f;
            }

            // 수평 입력이 -1이되면 X축의 음수 방면으로 현재 속도로 이동, 수평입력이 1이되면 X축의 양수 방면으로 현재 속도로 이동.
            rb.velocity = new Vector3(finalInput * currentSpeed, rb.velocity.y, 0);
        }

        void Jump()
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, 0);
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            currentMoveState = MoveState.Jump;
        }

        void InputFire()
        {
            if (Input.GetKey(KeyCode.J))
            {
                if (currentWeapon != null && firePoint != null)
                {
                    currentWeapon.Fire(this, firePoint, currentAimState, facingDirection);
                }

                else if (firePoint == null)
                {
                    Debug.Log("firePoint가 설정되지 않았습니다.");
                }
            }

            //테스트용 무기 교체기능
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                EquipWeapon(new Pistol());
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                EquipWeapon(new HMachinegun());
            }
        }

        public void EquipWeapon(IWeapon newWeapon)
        {
            currentWeapon = newWeapon;
            Debug.Log("무기교체 성공");
        }
    }
}
