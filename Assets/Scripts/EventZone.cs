using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scripts.Players;
using Scripts.Managers;

namespace Scripts.Events
{
    [System.Serializable]
    public struct SpawnData
    {
        public Transform spawnPoint;
        public GameObject enemyPrefab;
    }

    public class EventZone : MonoBehaviour
    {
        [Header("이벤트 설정")]
        public SpawnData[] spawnDatas;
        public bool isCameraLockZone = false;

        [Header("보스전 전용 설정")]
        public bool isBossZone = false;
        public GameObject missionCompleteUI; // 미션 컴플리트 텍스트 UI
        public AudioClip clearBGM; // 7초짜리 클리어 음악

        [Header("충돌 대상 설정")]
        public LayerMask playerLayer;
        public Scripts.UI.GoUI goUI;

        public List<GameObject> activeEnemies = new List<GameObject>();
        public bool isTriggered = false;
        public bool isEventCleared = false;

        public void OnTriggerEnter(Collider other)
        {
            if (isTriggered || isEventCleared) return;

            if (((1 << other.gameObject.layer) & playerLayer) != 0)
            {
                Player player = other.GetComponent<Player>();
                if (player != null)
                {
                    StartEvent();
                }
            }
        }

        public void StartEvent()
        {
            isTriggered = true;

            if (isCameraLockZone && GameManager.Instance != null)
            {
                GameManager.Instance.isScrollLocked = true;
            }

            foreach (SpawnData data in spawnDatas)
            {
                if (data.spawnPoint != null && data.enemyPrefab != null)
                {
                    GameObject enemy = Instantiate(data.enemyPrefab, data.spawnPoint.position, data.spawnPoint.rotation);
                    activeEnemies.Add(enemy);
                }
            }
        }

        private void Update()
        {
            if (!isTriggered || isEventCleared) return;

            int beforeCount = activeEnemies.Count;

            for (int i = activeEnemies.Count - 1; i >= 0; i--)
            {
                if (activeEnemies[i] == null)
                {
                    activeEnemies.RemoveAt(i);
                }
            }

            if (activeEnemies.Count == 0)
            {
                EndEvent();
            }
        }

        public void EndEvent()
        {
            isEventCleared = true;

            if (isBossZone)
            {
                // 보스전 클리어 연출 코루틴 시작
                StartCoroutine(GameClearRoutine());
            }
            else
            {
                if (isCameraLockZone)
                {
                    if (GameManager.Instance != null) GameManager.Instance.isScrollLocked = false;
                    if (goUI != null) goUI.ShowGoSign();
                }
                Destroy(gameObject);
            }
        }

        private IEnumerator GameClearRoutine()
        {
            // 1. 기존 BGM 정지 후 클리어 음악 재생
            if (SoundManager.Instance != null && clearBGM != null)
            {
                SoundManager.Instance.StopBGM();
                SoundManager.Instance.PlaySFX(clearBGM); // 효과음 채널로 재생하여 겹치지 않게 함
            }

            // 2. 미션 컴플리트 UI 활성화
            if (missionCompleteUI != null)
            {
                missionCompleteUI.SetActive(true);
            }

            // 3. 정확히 8초 대기
            yield return new WaitForSeconds(8f);

            // 4. 타이틀 씬(빌드 인덱스 0번)으로 강제 귀환
            SceneManager.LoadScene(0);
        }

        private void OnDrawGizmos()
        {
            if (spawnDatas == null) return;
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            foreach (SpawnData data in spawnDatas)
            {
                if (data.spawnPoint != null) Gizmos.DrawSphere(data.spawnPoint.position, 0.5f);
            }
        }
    }
}