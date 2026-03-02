using UnityEngine;

namespace Scripts.Data
{
    
    [CreateAssetMenu(fileName = "New Enemy Data", menuName = "Game Data/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("기본 능력치")]
        public float maxHp = 1f;
        public float moveSpeed = 3f;

        [Header("탐지 및 전투")]
        public float wakeUpDistanceX = 15f;
        public float attackRangeX = 6f;
        public float attackRate = 1.5f;

        [Header("프리팹 세팅")]
        public GameObject enemyBulletPrefab; // 이 적이 쏠 총알

        [Header("아이템 드랍 (선택사항)")]
        public GameObject dropItemPrefab;    // 죽었을 때 떨굴 아이템 (없으면 안 떨굼!)
    }
}