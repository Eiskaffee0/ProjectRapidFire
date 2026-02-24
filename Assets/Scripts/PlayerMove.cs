using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Scripts.PlayerMove
{
    public class PlayerMove : MonoBehaviour
    {
        float speed;
        public float jumpForce = 7f;
        public float verticalVelocity;
        public bool isGrounded = true;
        private Vector3 originalScale;
        private bool isCrouching = false;
        private bool facingRight = true;

        //  총알 관련 변수
        public GameObject bulletPrefab;   // 총알 프리팹
        public Transform firePoint;       // 총알 발사 위치
        public float bulletSpeed = 15f;   // 총알 속도

        private Vector3 shootDirection = Vector3.right; // 기본 방향

        void Start()
        {
            speed = 10f;
            originalScale = transform.localScale;
        }

        void Update()
        {
            // 좌우 이동
            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(Vector3.left * speed * Time.deltaTime);

                if (facingRight)
                {
                    facingRight = false;
                    transform.localScale = new Vector3(-originalScale.x, transform.localScale.y, transform.localScale.z);
                }
                shootDirection = Vector3.left; // 왼쪽 방향으로 총알
            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector3.right * speed * Time.deltaTime);

                if (!facingRight)
                {
                    facingRight = true;
                    transform.localScale = new Vector3(originalScale.x, transform.localScale.y, transform.localScale.z);
                }
                shootDirection = Vector3.right; // 오른쪽 방향으로 총알
            }

            // 위/아래 방향 입력
            if (Input.GetKey(KeyCode.W))
            {
                shootDirection = Vector3.up; // 위로 총알
            }
            else if (Input.GetKey(KeyCode.S) && !isCrouching)
            {
                shootDirection = Vector3.down; // 아래로 총알
            }

            // 점프
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                verticalVelocity = jumpForce;
                isGrounded = false;
            }

            // 중력 적용
            if (!isGrounded)
            {
                verticalVelocity += Physics.gravity.y * Time.deltaTime;
                transform.Translate(Vector3.up * verticalVelocity * Time.deltaTime);

                if (transform.position.y <= 0f)
                {
                    transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
                    verticalVelocity = 0f;
                    isGrounded = true;
                }
            }

            // 앉기
            if (Input.GetKeyDown(KeyCode.S) && !isCrouching)
            {
                transform.localScale = new Vector3(transform.localScale.x, originalScale.y * 0.5f, originalScale.z);
                speed = 5f;
                isCrouching = true;
            }
            else if (Input.GetKeyUp(KeyCode.S) && isCrouching)
            {
                transform.localScale = new Vector3(transform.localScale.x, originalScale.y, originalScale.z);
                speed = 10f;
                isCrouching = false;
            }

            //  총알 발사 (마우스 왼쪽 클릭)
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }
        }

        void Shoot()
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.velocity = shootDirection * bulletSpeed;
        }
    }
}




