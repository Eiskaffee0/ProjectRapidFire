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

        [Header("참조 데이터")]
        public Player2 player;

        [Header("스테이지 관리")] //추후 구현예정
        public float currentScrollLimitX = 0f;

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
    }

}


