using System;
using Unity.VisualScripting;
using UnityEngine;

namespace GGJ
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioStopOnGameStart : MonoBehaviour
    {
        AudioSource m_AudioSource;

        [SerializeField]
        GlobalMethodsAsset m_GlobalMethodsAsset;

        void Start()
        {
            m_AudioSource = GetComponent<AudioSource>();
            m_GlobalMethodsAsset.OnGameStart.AddListener(m_AudioSource.Stop);
        }

        void OnDestroy()
        {
            m_GlobalMethodsAsset.OnGameStart.RemoveListener(m_AudioSource.Stop);
        }

        public void PlayIfNotPlaying()
        {
            if (!m_AudioSource.isPlaying)
            {
                m_AudioSource.Play();
            }
        }
    }
}
