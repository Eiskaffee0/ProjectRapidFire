using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Scripts.Interfaces;
using Scripts.Weapons;
using UnityEngine.SceneManagement;
namespace Scripts.Players
{
    public class Player : MonoBehaviour, IDamageable
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
        [Header("플레이어 리스폰 관련")]
        public bool isInvincible = false;
        public float invincibilityDuration = 3f;
        public float blinkInterval = 0.15f;
        public Renderer[] playerRenderers;

        [Header("지면체크 설정")]
        public bool isGrounded;
        public Vector3 boxSize = new Vector3(0.8f, 0.1f, 0.5f); // 상자의 크기
        public Vector3 boxOffset = new Vector3(0f, -0.9f, 0f);  // 캐릭터 중심에서 발바닥까지의 오프셋
        public LayerMask groundLayer; // 바닥으로 인식할 레이어 (Ground)


        [Header("무기 설정")]
        public Transform firePoint;
       

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

        }

        void Start()
        {
            if (Scripts.Managers.GameManager.Instance != null)
            {
                Scripts.Managers.GameManager.Instance.player = this;
                Debug.Log("매니저에 플레이어 등록 완료");
            }
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
            ClampPositionToScreen();
        }

        void ClampPositionToScreen()
        {
            if (Scripts.Managers.GameManager.Instance == null)
            {
                return;
            }

            //플레이어의 두꼐를 고여한 여백(화면 끝에 완전 딱 붙기전에 멈추도록. 만약 필요 없다면 삭제
            float playerWidthOffset = 0.5f;

            //화면 왼쪽 끝 좌표.
            float leftEdge = Scripts.Managers.GameManager.Instance.GetScreenEdgeX(false) + playerWidthOffset;

            //현재 내 X좌표가 왼쪽 끝보다 작으면(뒤로 가려고한다면) 강제로 막음
            float clampedX = Mathf.Max(transform.position.x, leftEdge);

            //만약 보스전처럼 스크롤이 완전 잠긴 상황이라면 오른쪽으로도 못나가게 막기
            if (Scripts.Managers.GameManager.Instance.isScrollLocked)
            {
                float rightEdge = Scripts.Managers.GameManager.Instance.GetScreenEdgeX(true) - playerWidthOffset;
                clampedX = Mathf.Clamp(clampedX, leftEdge, rightEdge);
            }

            transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);


        }
        public void TakeDamage(float damage)
        {
            if (isInvincible)
            {
                return;
            }

            Die();
        }

        public void Die()
        {
            if (Scripts.Managers.GameManager.Instance != null)
            {
                Scripts.Managers.GameManager.Instance.OnPlayerDied(transform.position);
            }

            Destroy(gameObject);
        }

        public void StartInvincibility()
        {
            StartCoroutine(InvincibilityRoutine());
        }

        public IEnumerator InvincibilityRoutine()
        {
            isInvincible = true;
            Debug.Log("무적상태 진입");

            if (playerRenderers == null || playerRenderers.Length == 0)
            {
                playerRenderers = GetComponentsInChildren<Renderer>();
            }

            float timer = 0f;

            while (timer < invincibilityDuration)
            {
                foreach (Renderer r in playerRenderers)
                {
                    r.enabled = !r.enabled;
                }

                yield return new WaitForSeconds(blinkInterval);
                timer += blinkInterval;
            }

            foreach (Renderer r in playerRenderers)
            {
                r.enabled = true;
            }
            isInvincible = false;
            Debug.Log("무적 끝");
        }

        void CheckGround()
        {
            // 상자를 놔둘 중심 위치 (내 위치 + 발바닥 오프셋)
            Vector3 centerPos = transform.position + boxOffset;

            // CheckBox 발동! (중심위치, 상자크기의 절반(Half Extents), 캐릭터회전, 바닥레이어)
            // 지면(Ground Layer)과 1mm라도 겹쳐있으면 무조건 true를 반환함
            isGrounded = Physics.CheckBox(centerPos, boxSize / 2, transform.rotation, groundLayer);
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

        private void OnDrawGizmos()
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;

            // 박스가 그려질 중심 위치
            Vector3 centerPos = transform.position + boxOffset;

            // 크기 그대로 박스 딱 한 개만 그림!
            Gizmos.DrawWireCube(centerPos, boxSize);
        }
    }
}
