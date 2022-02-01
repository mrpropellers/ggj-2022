using System;
using Unity.VisualScripting;
using UnityEngine;

namespace GGJ
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioStopOnGameStop : MonoBehaviour
    {
        AudioSource m_AudioSource;

        [SerializeField]
        GlobalMethodsAsset m_GlobalMethodsAsset;

        void Start()
        {
            m_AudioSource = GetComponent<AudioSource>();
            m_GlobalMethodsAsset.OnGameStop.AddListener(m_AudioSource.Stop);
        }

        void OnDestroy()
        {
            m_GlobalMethodsAsset.OnGameStop.RemoveListener(m_AudioSource.Stop);
        }
    }
}
