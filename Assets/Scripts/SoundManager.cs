using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Managers
{
    public class SoundManager : MonoBehaviour
    {
        // 싱글톤 인스턴스
        public static SoundManager Instance { get; private set; }

        [Header("오디오 소스")]
        public AudioSource bgmSource; // 배경음악 전용
        public AudioSource sfxSource; // 효과음 전용

        void Awake()
        {
            // 싱글톤 패턴 설정 (씬이 넘어가도 파괴되지 않음)
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

        /// <summary>
        /// 배경음악을 재생합니다. (기존 음악은 끊기고 새 음악이 무한 반복됨)
        /// </summary>
        public void PlayBGM(AudioClip bgmClip, float volume = 1f)
        {
            if (bgmSource == null || bgmClip == null) return;

            bgmSource.clip = bgmClip;
            bgmSource.volume = volume;
            bgmSource.loop = true;
            bgmSource.Play();
        }

        /// <summary>
        /// 배경음악을 정지합니다.
        /// </summary>
        public void StopBGM()
        {
            if (bgmSource != null)
            {
                bgmSource.Stop();
            }
        }

        /// <summary>
        /// 효과음을 재생합니다. (여러 소리가 겹쳐서 재생됨)
        /// </summary>
        public void PlaySFX(AudioClip sfxClip, float volume = 1f)
        {
            if (sfxSource == null || sfxClip == null) return;

            // PlayOneShot은 소리가 끝날 때까지 기다리지 않고 겹쳐서 재생해 줌! (머신건 연사에 필수)
            sfxSource.PlayOneShot(sfxClip, volume);
        }
    }
}