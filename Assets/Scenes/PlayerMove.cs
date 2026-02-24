using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerMove : MonoBehaviour
{
    float speed;
    public float jumpForce = 7f;      // 점프 힘
    public float verticalVelocity;   // y축 속도
    public bool isGrounded = true;   // 바닥 체크
    private Vector3 originalScale; // 원래 크기 저장
    private bool isCrouching = false; // 앉기 상태 체크

    void Start()
    {
        speed = 10f;
        originalScale = transform.localScale; // 시작 시 원래 크기 저장
    }

    void Update()
    {
        // 좌우 이동
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }

        // 점프 (바닥에 있을 때만)
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

            // 바닥에 닿았는지 간단 체크 (y <= 0 기준)
            if (transform.position.y <= 0f)
            {
                transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
                verticalVelocity = 0f;
                isGrounded = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.S) && !isCrouching)
        {
            transform.localScale = new Vector3(originalScale.x, originalScale.y * 0.5f, originalScale.z);
            speed = 5f; // 앉으면 속도 줄이기
            isCrouching = true;
        }
        else if (Input.GetKeyUp(KeyCode.S) && isCrouching)
        {
            transform.localScale = originalScale; // 원래 크기로 복구
            speed = 10f; // 원래 속도로 복구
            isCrouching = false;
        }





    }
}

