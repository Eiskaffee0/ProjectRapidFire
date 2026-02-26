using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Scripts.Interfaces;
namespace Scripts.Enemies
{
    public class Enemy : MonoBehaviour, IDamageable
    {
        public enum State
        {
            Idle,
            Ready,
            Attack,
            Dead
        }

        [Header("몬스터 상태 및 능력치")]
        public State currentState = State.Idle;
        public float hp = 1f;

        [Header("탐지 및 전투설정")]
        public float detectionRange = 10f;
        public float attackRange = 7f;
        public float readyTime = 1f;

        public float attackRate = 1.5f; //공격빈도 설정
        public float attackTimer = 100f;
        public float moveSpeed = 2f;


        public GameObject enemyBulletPrefab; //몬스터의 투사체 프리팹
        public Transform firePoint; // 몬스터 투사체의 발사 위치

        public Transform playerTransform; //플레이어를 추적하기 위한 플레이어 위치값
        public bool isPreparing = false;

        void Start()
        {
            if (Scripts.Managers.GameManager.Instance != null && Scripts.Managers.GameManager.Instance.player != null)
            {
                playerTransform = Scripts.Managers.GameManager.Instance.player.transform;
            }

            else
            {
                Debug.Log("게임메니저 또는 플레이어 참조가 비었습니다.");
            }
        }

        void Update()
        {
            if (currentState == State.Dead)
            {
                return;
            }

            if (playerTransform == null)
            {
                if (Scripts.Managers.GameManager.Instance != null && Scripts.Managers.GameManager.Instance.player != null)
                {
                    playerTransform = Scripts.Managers.GameManager.Instance.player.transform;
                }

                else
                {
                    return;
                }
            }

            UpdateFacingDirection();

            switch (currentState)
            {
                case State.Idle:
                    UpdateIdle();
                    break;
                case State.Ready:
                    UpdateReady();
                    break;
                case State.Attack:
                    UpdateAttack();
                    break;
            }
        }

        void UpdateFacingDirection()
        {
            if (playerTransform == null)
            {
                return;
            }

            if (playerTransform.position.x < transform.position.x)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }

            else if (playerTransform.position.x > transform.position.x)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }

        void UpdateIdle()
        {
            if (playerTransform == null)
            {
                return;
            }

            float distance = Vector3.Distance(transform.position, playerTransform.position); // 플레이어와 몬스터간의 거리 계산

            if (distance <= detectionRange)
            {
                ChangeState(State.Ready);
            }
        }

        public void UpdateReady()
        {
            if (!isPreparing)
            {
                StartCoroutine(ReadyRoutine());
            }
        }

        public IEnumerator ReadyRoutine()
        {
            isPreparing = true;
            Debug.Log($"몬스터: 전투준비 ({readyTime}초 대기");

            yield return new WaitForSeconds(readyTime);

            ChangeState(State.Attack);
            isPreparing = false;
        }

        public void UpdateAttack()
        {
            if (playerTransform == null)
            {
                return;
            }

            float distance = Vector3.Distance(transform.position, playerTransform.position);

            if (distance > attackRange)
            {
                Vector3 targetPos = new Vector3(playerTransform.position.x, transform.position.y, transform.position.z);
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            }

            else
            {
                attackTimer += Time.deltaTime;
                if (attackTimer >= attackRate)
                {
                    Attack();
                    attackTimer = 0f;
                }
            }
        }

        public void Attack()
        {
            Debug.Log("몬스터: 공격 개시");

            if (enemyBulletPrefab != null && firePoint != null)
            {
                GameObject bullet = Instantiate(enemyBulletPrefab, firePoint.position, firePoint.rotation);
            }
        }

        public void ChangeState(State newState)
        {
            currentState = newState;
        }
        public void TakeDamage(float damage)
        {
            if (currentState == State.Dead)
            {
                return;
            }

            hp -= damage;
            Debug.Log($"{damage}만큼 피해를 입었습니다. 남은 체력{hp}");
            if (hp <= 0f)
            {
                Die();
            }
        }

        public void Die()
        {
            Debug.Log("기본몹 사망");
            Destroy(gameObject);
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}


