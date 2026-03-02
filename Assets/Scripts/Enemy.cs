using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Interfaces;
using Scripts.Data;

namespace Scripts.Enemies
{
    public class Enemy : MonoBehaviour, IDamageable
    {
        public enum State { Sleep, Run, Attack, Dead }

        [Header("데이터 연결")]
        public EnemyData enemyData;

        [Header("상태 및 무기 연결")]
        public State currentState = State.Sleep;
        public Transform firePoint;

        [Header("지면 체크 설정")]
        public LayerMask groundLayer;
        public float groundCheckDistance = 1.1f;

        [Header("피격 효과 설정")]
        public Renderer enemyRenderer;
        public Color hitColor = Color.white;
        public float flashDuration = 0.05f;

        private float currentHp;
        private float attackTimer = 0f;
        private Rigidbody rb;
        private bool isGrounded;
        private Color originalColor;
        private Coroutine flashCoroutine;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionZ;

            if (enemyData != null)
            {
                currentHp = enemyData.maxHp;
                attackTimer = enemyData.attackRate;
            }
            else
            {
                Debug.LogError("몬스터 데이터(EnemyData)가 할당되지 않았습니다!");
            }

            // 시작할 때 원래 색상을 저장해둡니다.
            if (enemyRenderer == null)
            {
                enemyRenderer = GetComponentInChildren<Renderer>();
            }

            if (enemyRenderer != null)
            {
                originalColor = enemyRenderer.material.color;
            }
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
            if (currentState == State.Dead || enemyData == null) return;

            if (transform.position.y < -15f)
            {
                Die();
                return;
            }

            isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

            if (!isGrounded) return;

            Transform targetPlayer = GetCurrentPlayer();
            if (targetPlayer == null) return;

            ArcadeAILogic(targetPlayer);
            UpdateFacingDirection(targetPlayer);
        }

        void ArcadeAILogic(Transform target)
        {
            float distanceX = Mathf.Abs(target.position.x - transform.position.x);

            switch (currentState)
            {
                case State.Sleep:
                    if (distanceX <= enemyData.wakeUpDistanceX) currentState = State.Run;
                    break;

                case State.Run:
                    if (distanceX <= enemyData.attackRangeX)
                    {
                        rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
                        currentState = State.Attack;
                    }
                    else
                    {
                        float dirX = Mathf.Sign(target.position.x - transform.position.x);
                        rb.velocity = new Vector3(dirX * enemyData.moveSpeed, rb.velocity.y, 0f);
                    }
                    break;

                case State.Attack:
                    if (distanceX > enemyData.attackRangeX)
                    {
                        currentState = State.Run;
                    }
                    else
                    {
                        rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
                        attackTimer += Time.deltaTime;
                        if (attackTimer >= enemyData.attackRate)
                        {
                            Attack();
                            attackTimer = 0f;
                        }
                    }
                    break;
            }
        }

        void UpdateFacingDirection(Transform target)
        {
            if (currentState == State.Sleep) return;

            if (target.position.x < transform.position.x)
                transform.rotation = Quaternion.Euler(0, 180, 0);
            else if (target.position.x > transform.position.x)
                transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        public void Attack()
        {
            if (enemyData.enemyBulletPrefab != null && firePoint != null)
            {
                Instantiate(enemyData.enemyBulletPrefab, firePoint.position, firePoint.rotation);
            }
        }

        public void TakeDamage(float damage)
        {
            if (currentState == State.Dead) return;

            currentHp -= damage;

            if (currentHp > 0f)
            {
                // 체력이 남아있다면 피격 반짝임 효과 실행
                if (flashCoroutine != null)
                {
                    StopCoroutine(flashCoroutine);
                }
                flashCoroutine = StartCoroutine(HitFlashRoutine());
            }
            else
            {
                Die();
            }
        }

        private IEnumerator HitFlashRoutine()
        {
            if (enemyRenderer != null)
            {
                // 지정된 피격 색상으로 변경
                enemyRenderer.material.color = hitColor;

                // 아주 짧은 시간 대기
                yield return new WaitForSeconds(flashDuration);

                // 원래 색상으로 복구
                enemyRenderer.material.color = originalColor;
            }
        }

        public void Die()
        {
            currentState = State.Dead;

            if (enemyData != null && enemyData.dropItemPrefab != null)
            {
                Instantiate(enemyData.dropItemPrefab, transform.position, Quaternion.identity);
                Debug.Log("몬스터가 아이템을 드랍했습니다.");
            }

            Destroy(gameObject);
        }

        // 에디터에서 지면 체크 레이캐스트 길이를 시각적으로 확인하기 위한 함수
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
        }
    }
}