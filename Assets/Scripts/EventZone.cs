using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Players;
using Scripts.Managers;

namespace Scripts.Events
{
    public class EventZone : MonoBehaviour
    {
        [Header("이벤트 설정")]
        public GameObject enemyPrefab;
        public Transform[] spawnPoints;

        public List<GameObject> activeEnemies = new List<GameObject>();
        public bool isTriggered = false;
        public bool isEventCleared = false;

        public void OnTriggerEnter(Collider other)
        {
            if (isTriggered || isEventCleared)
            {
                return;
            }

            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                StartEvent();
            }
        }

        public void StartEvent()
        {
            isTriggered = true;
            Debug.Log("이벤트 발동. 스크롤 락");

            //게임메니저에서 카메라 스크롤 잠금
            if (GameManager.Instance != null)
            {
                GameManager.Instance.isScrollLocked = true;
            }
            
            // 지정된 위치들에 적 스폰
            foreach (Transform sp in spawnPoints)
            {
                if (sp != null)
                {
                    GameObject enemy = Instantiate(enemyPrefab, sp.position, sp.rotation);
                    activeEnemies.Add(enemy); //살아있는 적을 감시, 추적하기위한 리스트 등록
                }
            }
        }

        private void Update()
        {
            // 이벤트가 진행 중일때만 작동
            if (!isTriggered || isEventCleared)
            {
                return;
            }

            // 살아있는 적 감시 로직
            // 리스트에 있는 적들 중, Destroy되어 null이 된 적들은 리스트에서 지워줌

            activeEnemies.RemoveAll(item => item == null);

            // 리스트에 남은 적이 0명이면 이벤트 종료

            if (activeEnemies.Count == 0)
            {
                EndEvent();
            }
        }

        public void EndEvent()
        {
            isEventCleared = true;
            Debug.Log("적 전멸. 스크롤 잠금 해제. Go!");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.isScrollLocked = false;
            }

            Destroy(gameObject);
        }
    }
}

