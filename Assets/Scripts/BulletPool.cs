using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Scripts.Player2
{
    public class BulletPool : MonoBehaviour
    {
        public static BulletPool Instance;

        [Header("ÃÑ¾Ë ÇÁ¸®ÆÕµé")]
        public GameObject PistolBulletPrefab;
        public GameObject HMBulletPrefab;
        public GameObject SgBulletPrefab;

        private Queue<GameObject> pistolPool = new Queue<GameObject>();

        private void Awake()
        {
            Instance = this;
            InitializePool(10);
        }

        void InitializePool(int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject obj = Instantiate(PistolBulletPrefab, transform);
                obj.SetActive(false);
                pistolPool.Enqueue(obj);
            }
        }

        public GameObject GetPistolBullet()
        {
            if (pistolPool.Count > 0)
            {
                GameObject obj = pistolPool.Dequeue();
                obj.SetActive(true);
                return obj;
            }
            else
            {
                GameObject obj = Instantiate(PistolBulletPrefab, transform);
                return obj;
            }
        }

        public void ReturnPistolBullet(GameObject obj)
        {
            obj.SetActive(false);
            pistolPool.Enqueue(obj);
        }
    }
}
