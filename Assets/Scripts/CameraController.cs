using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Managers
{
    public class CameraController : MonoBehaviour
    {
        [Header("플레이어 추적 설정")]
        public float smoothSpeed = 5f;
        public float offsetX = 3f; // 카메라가 플레이어보다 살짝 앞을 비추도록 오프셋을 잡아줌
        private float maxCameraX = float.MinValue; //플로트 민벨류를 이용해 카메라가 해당 값보다 왼쪽으로 갈 수 없게 막아줌.

        private void LateUpdate()
        {
            //매니저나 플레이어가 없으면 작동 안하게 안전장치(버그나 플레이어가 죽어서 리스폰 대기상황일 때)
            if (GameManager.Instance == null || GameManager.Instance.player == null)
            {
                return;
            }

            if (GameManager.Instance.isScrollLocked)
            {
                return;
            }

            Transform player = GameManager.Instance.player.transform;

            float targetX = player.position.x + offsetX;

            if (targetX > maxCameraX)
            {
                maxCameraX = targetX;
            }

            //일단은 카메라이동은 X축만. Z축은 당연히 안움직이겠지만 Y축은 3편 2스테이지 보스처럼 상승하면서 하단쏘기 해야하는 보스 구현할 때 해보자.
            Vector3 targetPosition = new Vector3(maxCameraX, transform.position.y, transform.position.z);

            //Lerp가 부드럽게 카메라 움직여주는 기능이라고함. 제미나이 최고.
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        }
    }

}

