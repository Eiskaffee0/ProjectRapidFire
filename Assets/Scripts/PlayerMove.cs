using UnityEngine;

namespace Scripts.Players
{
    // Rigidbody 컴포넌트가 반드시 필요하도록 설정
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMove : MonoBehaviour
    {
        [Header("이동 및 점프")]
        public float moveSpeed = 8f;       // 기본 이동 속도
        public float crouchSpeed = 4f;     // 앉았을 때 이동 속도
        public float jumpForce = 12f;      // 점프 힘

        [Header("사격 설정")]
        public GameObject bulletPrefab;    // 발사할 총알 프리팹
        public Transform firePoint;        // 총알 발사 위치
        public float bulletSpeed = 20f;    // 총알 속도

        private Rigidbody rb;              // 플레이어의 Rigidbody
        private bool isGrounded;           // 땅에 닿아있는지 여부
        private bool isCrouching;          // 앉아있는지 여부
        private Vector3 shootDirection;    // 총알 발사 방향
        private Vector3 originalScale;     // 원래 캐릭터 크기 저장용

        void Start()
        {
            rb = GetComponent<Rigidbody>();          // Rigidbody 가져오기
            originalScale = transform.localScale;    // 원래 크기 저장

            // 3D 환경에서 횡스크롤 고정 설정
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
            // 회전과 Z축 이동을 막아 2D 횡스크롤처럼 동작
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            // 물리 보간으로 움직임을 부드럽게
        }

        void Update()
        {
            CheckGround();       // 땅에 닿아있는지 체크
            HandleMovement();    // 이동 처리
            HandleJump();        // 점프 처리
            HandleCrouch();      // 앉기 처리
            HandleAiming();      // 조준 방향 처리

            // 마우스 왼쪽 클릭 시 총알 발사
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }
        }

        void HandleMovement()
        {
            float moveInput = Input.GetAxisRaw("Horizontal"); // A/D 또는 ←/→ 입력
            float currentSpeed = isCrouching ? crouchSpeed : moveSpeed; // 앉으면 속도 줄이기

            // X축 이동 (속도 변경)
            rb.velocity = new Vector3(moveInput * currentSpeed, rb.velocity.y, 0);

            // 캐릭터 방향 회전 (오른쪽: 0도, 왼쪽: 180도)
            if (moveInput > 0)
                transform.rotation = Quaternion.Euler(0, 0, 0);
            else if (moveInput < 0)
                transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        void HandleJump()
        {
            // 스페이스 입력 + 땅에 있을 때만 점프 가능
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // 위로 힘을 가해 점프
            }
        }

        void HandleCrouch()
        {
            // S 키 누르면 앉기
            if (Input.GetKeyDown(KeyCode.S))
            {
                isCrouching = true;
                // 캐릭터 크기를 절반으로 줄여 앉은 상태 표현
                transform.localScale = new Vector3(originalScale.x, originalScale.y * 0.5f, originalScale.z);
            }
            // S 키 떼면 원래 크기로 복귀
            else if (Input.GetKeyUp(KeyCode.S))
            {
                isCrouching = false;
                transform.localScale = originalScale;
            }
        }

        void HandleAiming()
        {
            float moveInput = Input.GetAxisRaw("Horizontal");

            // W 키: 위로 조준
            if (Input.GetKey(KeyCode.W))
                shootDirection = Vector3.up;
            // S 키: 아래로 조준
            else if (Input.GetKey(KeyCode.S))
                shootDirection = Vector3.down;
            // 이동 방향에 따라 좌/우 조준
            else if (moveInput > 0)
                shootDirection = Vector3.right;
            else if (moveInput < 0)
                shootDirection = Vector3.left;
        }

        void Shoot()
        {
            // 총알 프리팹과 발사 위치가 설정되어 있을 때만 실행
            if (bulletPrefab != null && firePoint != null)
            {
                // 총알 생성
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
                Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

                if (bulletRb != null)
                {
                    bulletRb.useGravity = false; // 총알은 중력 영향 없음
                    bulletRb.velocity = shootDirection.normalized * bulletSpeed; // 지정된 방향으로 발사
                }

                Destroy(bullet, 3f); // 3초 후 총알 자동 삭제
            }
        }

        void CheckGround()
        {
            // 플레이어 발밑으로 레이를 쏴서 땅 체크
            // 시작 위치: 캐릭터 중심에서 약간 위 (0.1)
            // 방향: 아래쪽
            // 길이: 1f → 캐릭터 발밑까지 닿는지 확인
            isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 1f);
        }
    }
}

