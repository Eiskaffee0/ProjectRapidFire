using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Players;

namespace Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance
        {
            get;
            private set;
        }

        [Header("플레이어 데이터")]
        public Player2 player;
        public GameObject PlayerPrefab;

        [Header("부활설정")]
        public float dropHeight = 10f;

        [Header("게임상태 관리")]
        public int currentLives = 3;


        [Header("스테이지 관리")]
        public bool isScrollLocked = false; //화면 잠금여부
        private Camera mainCam;

                
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            else
            {
                Destroy(gameObject);
            }

        }

        void Start()
        {
            mainCam = Camera.main;
        }

        public float GetScreenEdgeX(bool isRightEdge)
        {
            if (mainCam == null || player == null)
            {
                return 0f;
            }

            float zDistance = Mathf.Abs(mainCam.transform.position.z - player.transform.position.z);

            Vector3 edge = mainCam.ViewportToWorldPoint(new Vector3(isRightEdge ? 1f : 0f, 0, zDistance));
            return edge.x;
        }

        public void OnPlayerDied(Vector3 deathPosition)
        {
            currentLives--;
            Debug.Log($"플레이어 사망. 남은 목숨{currentLives}");

            if (currentLives > 0)
            {
                StartCoroutine(RespawnRoutine(deathPosition));
            }

            else
            {
                Debug.Log("Game Over");
                // 게임오버 UI 호출부를 넣어주자.
            }
        }

        public IEnumerator RespawnRoutine(Vector3 deathPosition)
        {
            yield return new WaitForSeconds(2f);

            Vector3 spawnPos = new Vector3(deathPosition.x, deathPosition.y + dropHeight, deathPosition.z);

            GameObject newPlayerObj = Instantiate(PlayerPrefab, spawnPos, Quaternion.identity);

            player = newPlayerObj.GetComponent<Player2>();

            player.StartInvincibility();
        }
    }

}


