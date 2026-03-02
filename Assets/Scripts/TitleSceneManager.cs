using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환에 필수적인 네임스페이스

namespace Scripts.UI
{
    public class TitleSceneManager : MonoBehaviour
    {
        /// <summary>
        /// 게임을 시작합니다. (메인 스테이지 씬 로드)
        /// </summary>
        public void LoadGame()
        {
            // 빌드 세팅의 Scenes In Build 목록에서 인덱스 1번(MainStage)을 로드합니다.
            Debug.Log("게임 시작! 메인 스테이지로 이동합니다.");
            SceneManager.LoadScene(1);
        }

        /// <summary>
        /// 게임을 종료합니다.
        /// </summary>
        public void QuitGame()
        {
            Debug.Log("게임을 종료합니다.");
            // 실제 빌드된 게임(애플리케이션)을 종료합니다. (유니티 에디터에서는 작동하지 않음)
            Application.Quit();
        }
    }
}