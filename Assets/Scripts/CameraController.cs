using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Managers
{
    public class CameraController : MonoBehaviour
    {
        [Header("플레이어 추적 설정")]
        public float smoothSpeed = 5f;
        public float offsetX = 3f;
        private float maxCameraX = float.MinValue;

        private void LateUpdate()
        {
            if (GameManager.Instance == null || GameManager.Instance.player == null) return;
            if (GameManager.Instance.isScrollLocked) return;

            Transform player = GameManager.Instance.player.transform;
            float targetX = player.position.x + offsetX;

            if (targetX > maxCameraX)
            {
                maxCameraX = targetX;
            }

            Vector3 targetPosition = new Vector3(maxCameraX, transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        }

        //  [디버그용] 게임 화면에 카메라의 상태를 실시간 텍스트로 띄워줍니다!
        /*private void OnGUI()
        {
            if (GameManager.Instance == null) return;

            GUIStyle style = new GUIStyle();
            style.fontSize = 24;
            style.normal.textColor = Color.yellow;
            style.fontStyle = FontStyle.Bold;

            GUILayout.BeginArea(new Rect(10, 10, 500, 300));
            GUILayout.Label("===  카메라 디버그 패널 ===", style);

            if (GameManager.Instance.isScrollLocked)
                GUILayout.Label(" 상태: 스크롤 잠김 (이벤트 진행중!)", style);
            else
                GUILayout.Label(" 상태: 자유 이동 가능", style);

            if (GameManager.Instance.player != null)
            {
                float playerX = GameManager.Instance.player.transform.position.x;
                GUILayout.Label($" 플레이어 X 위치: {playerX:F2}", style);
                GUILayout.Label($" 카메라가 가야할 목표 X: {(playerX + offsetX):F2}", style);
            }
            GUILayout.Label($"벽(최대) X 위치: {maxCameraX:F2}", style);

            GUILayout.EndArea();
        }*/
    }
}