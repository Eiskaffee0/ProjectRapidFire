using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class GoUI : MonoBehaviour
    {
        public GameObject goTextObject; // GoText ¿ÀºêÁ§Æ® ¿¬°á

        public void ShowGoSign()
        {
            StartCoroutine(BlinkRoutine());
        }

        private IEnumerator BlinkRoutine()
        {
            goTextObject.SetActive(true);

            // 3¹ø ±ôºýÀÓ
            for (int i = 0; i < 3; i++)
            {
                goTextObject.SetActive(false);
                yield return new WaitForSeconds(0.2f);
                goTextObject.SetActive(true);
                yield return new WaitForSeconds(0.2f);
            }

            yield return new WaitForSeconds(1f);
            goTextObject.SetActive(false); // ¿¬Ãâ ³¡³ª¸é ´Ù½Ã ¼û±è
        }
    }
}