using System;
using UnityEngine;

namespace DIStudy.WhackAMole
{
    
    [Serializable]
    public class MyGameConfig
    {
        [SerializeField] private float m_RoundDuration;

        [SerializeField] private int m_MaxActiveMoles;

        [SerializeField] private float m_MoleUpDuration;

        [SerializeField] private float m_SpawnInterval;

        [SerializeField] private int m_ScorePerHit;

        public float RoundDuration => m_RoundDuration;
        public int MaxActiveMoles => m_MaxActiveMoles;
        public float MoleUpDuration => m_MoleUpDuration;
        public float SpawnInterval => m_SpawnInterval;
        public int ScorePerHit => m_ScorePerHit;
    }
}
