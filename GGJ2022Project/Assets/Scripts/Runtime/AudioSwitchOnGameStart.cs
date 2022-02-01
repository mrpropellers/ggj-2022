using System;
using UnityEngine;

namespace GGJ
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSwitchOnGameStart : MonoBehaviour
    {
        AudioSource m_AudioSource;
        AudioClip m_OriginalAudio;

        [SerializeField]
        AudioClip m_AudioSwitchTo;

        [SerializeField]
        GlobalMethodsAsset m_GlobalMethodsAsset;

        void Start()
        {
            m_AudioSource = GetComponent<AudioSource>();
            m_OriginalAudio = m_AudioSource.clip;
            m_GlobalMethodsAsset.OnEnterApplication.AddListener(PlayIfNotPlaying);
            m_GlobalMethodsAsset.OnGameStart.AddListener(AudioSwitch);
            m_GlobalMethodsAsset.OnGameQuit.AddListener(AudioSwitchBack);
            m_GlobalMethodsAsset.OnEnterApplication.AddListener(PlayIfNotPlaying);
        }

        void PlayIfNotPlaying()
        {
            if (!m_AudioSource.isPlaying)
            {
                m_AudioSource.Play();
            }
        }

        void AudioSwitchBack()
        {
            SwitchTo(m_OriginalAudio);
        }

        void AudioSwitch()
        {
            SwitchTo(m_AudioSwitchTo);
        }

        void SwitchTo(AudioClip clip)
        {
            if (m_AudioSource.clip == clip)
            {
                return;
            }

            m_AudioSource.Stop();
            m_AudioSource.clip = clip;
            m_AudioSource.Play();
        }

        void OnDestroy()
        {
            m_GlobalMethodsAsset.OnGameStop.RemoveListener(m_AudioSource.Stop);
        }
    }
}
