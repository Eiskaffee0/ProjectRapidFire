using System.Collections;
using UnityEngine;
using Scripts.Interfaces;

namespace Scripts.Enemies
{
    public class Boss : MonoBehaviour, IDamageable
    {
        [Header("보스 능력치 및 이동")]
        public float maxHp = 50f;
        public float moveSpeed = 2.5f;

        [Header("보스 전투 설정")]
        public float attackDistance = 1.5f;
        public float attackCooldown = 2.5f;
        public int burstCount = 4;
        public float burstInterval = 0.15f;

        [Header("무기 및 피격 효과")]
        public GameObject bulletPrefab;
        public Transform firePoint;
        public Renderer bossRenderer;
        public Color hitColor = Color.red;

        [Header("지면 체크 설정")]
        public LayerMask groundLayer;
        public float groundCheckDistance = 2.0f;

        private float currentHp;
        private bool isGrounded;
        private bool isAttacking = false;
        private Color originalColor;
        private Rigidbody rb;
        private Coroutine flashCoroutine;

        void Start()
        {
            currentHp = maxHp;
            rb = GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionZ;

            if (bossRenderer == null) bossRenderer = GetComponentInChildren<Renderer>();
            if (bossRenderer != null) originalColor = bossRenderer.material.color;
        }

        private Transform GetCurrentPlayer()
        {
            if (Scripts.Managers.GameManager.Instance != null && Scripts.Managers.GameManager.Instance.player != null)
            {
                return Scripts.Managers.GameManager.Instance.player.transform;
            }
            return null;
        }

        void Update()
        {
            if (currentHp <= 0) return;

            isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

            if (!isGrounded) return;

            Transform targetPlayer = GetCurrentPlayer();
            if (targetPlayer == null) return;

            UpdateFacingDirection(targetPlayer);

            // 공격 중이 아닐 때만 이동 및 거리 계산 수행
            if (!isAttacking)
            {
                float distanceX = Mathf.Abs(targetPlayer.position.x - transform.position.x);

                if (distanceX > attackDistance)
                {
                    // 거리가 멀면 플레이어 방향으로 이동
                    float dirX = Mathf.Sign(targetPlayer.position.x - transform.position.x);
                    rb.velocity = new Vector3(dirX * moveSpeed, rb.velocity.y, 0f);
                }
                else
                {
                    // 사거리 진입 시 제자리에 멈추고 공격 시작
                    rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
                    StartCoroutine(BossPatternRoutine());
                }
            }
        }

        private IEnumerator BossPatternRoutine()
        {
            isAttacking = true; // 이동 중지 및 중복 공격 방지 락 걸기

            for (int i = 0; i < burstCount; i++)
            {
                if (bulletPrefab != null && firePoint != null)
                {
                    Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                }
                yield return new WaitForSeconds(burstInterval);
            }

            // 사격 완료 후 인스펙터에서 설정한 쿨타임만큼 대기 (플레이어 딜링 타임)
            yield return new WaitForSeconds(attackCooldown);

            isAttacking = false; // 쿨타임 종료, 다시 추적 및 공격 가능
        }

        void UpdateFacingDirection(Transform target)
        {
            if (target.position.x < transform.position.x)
                transform.rotation = Quaternion.Euler(0, 180, 0);
            else if (target.position.x > transform.position.x)
                transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        public void TakeDamage(float damage)
        {
            if (currentHp <= 0) return;

            currentHp -= damage;

            if (currentHp > 0)
            {
                if (flashCoroutine != null) StopCoroutine(flashCoroutine);
                flashCoroutine = StartCoroutine(HitFlashRoutine());
            }
            else
            {
                Die();
            }
        }

        private IEnumerator HitFlashRoutine()
        {
            if (bossRenderer != null)
            {
                bossRenderer.material.color = hitColor;
                yield return new WaitForSeconds(0.05f);
                bossRenderer.material.color = originalColor;
            }
        }

        public void Die()
        {
            Destroy(gameObject);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
        }
    }
}