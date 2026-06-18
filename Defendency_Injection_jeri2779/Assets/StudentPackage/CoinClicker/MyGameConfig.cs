using System;
using UnityEngine;

namespace DIStudy.CoinClicker.Student
{
    [Serializable]
    public class MyGameConfig
    {
        [SerializeField]
        private int m_CoinValue = 1;

        [SerializeField]
        private float m_AutoSaveInterval = 10f;

        [SerializeField]
        private int m_CoinCount = 3;

        [SerializeField]
        private float m_RespawnDelay = 1f;

        public int CoinValue => m_CoinValue;
        public float AutoSaveInterval => m_AutoSaveInterval;
        public int CoinCount => m_CoinCount;
        public float RespawnDelay => m_RespawnDelay;
    }
}
