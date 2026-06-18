using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DIStudy.WhackAMole
{
    
    public class MyMoleSpawner : MonoBehaviour
    {
        [SerializeField]
        private MyMole m_MolePrefab;

        [SerializeField]
        private Transform[] m_Holes;

        private IObjectResolver m_Resolver;
        private MyGameConfig m_Config;
        private MyGameDirector m_Director;

        private bool[] m_Occupied;
        private int m_ActiveCount;
        private float m_Timer;
        private int m_LastHole = -1;

         
        private readonly List<int> m_Candidates = new List<int>();

        [Inject]
        public void Construct(IObjectResolver resolver, MyGameConfig config, MyGameDirector director)
        {
            m_Resolver = resolver;
            m_Config = config;
            m_Director = director;
        }

        private void Start()
        {
            if (m_MolePrefab == null || m_Holes == null || m_Holes.Length == 0)
            {
                Debug.LogWarning("[MoleSpawner] Mole 프리팹 또는 구멍(Holes)이 연결되지 않았습니다.");
                enabled = false;
                return;
            }

            if (m_Director == null)
            {
                Debug.LogWarning("[MoleSpawner] 주입되지 않았습니다");
                enabled = false;
                return;
            }

            m_Occupied = new bool[m_Holes.Length];
            m_Director.RoundStarted += ResetField;
        }

        private void OnDestroy()
        {
            if (m_Director != null)
                m_Director.RoundStarted -= ResetField;
        }

        private void ResetField()
        {
            for (int i = 0; i < m_Holes.Length; i++)
                m_Occupied[i] = false;
            m_ActiveCount = 0;
            m_Timer = 0f;
            m_LastHole = -1;
        }

        private void Update()
        {
            if (!m_Director.IsRunning) return;

            m_Timer += Time.deltaTime;
            if (m_Timer < m_Config.SpawnInterval) return;

            m_Timer = 0f;
            if (m_ActiveCount >= m_Config.MaxActiveMoles) return;

            Spawn();
        }

        private void Spawn()
        {
            int hole = PickFreeHole();
            if (hole < 0) return;
                
            m_Occupied[hole] = true;
            m_ActiveCount++;

            MyMole mole = m_Resolver.Instantiate(m_MolePrefab);
            mole.Finished += OnMoleFinished;
            mole.Activate(hole, m_Holes[hole].position);
        }

        private int PickFreeHole()
        {
            
            m_Candidates.Clear();
            for (int i = 0; i < m_Holes.Length; i++)
            {
                if (!m_Occupied[i] && i != m_LastHole)
                {
                    m_Candidates.Add(i);
                }
            }

     
            if (m_Candidates.Count == 0)
            
                for (int i = 0; i < m_Holes.Length; i++)
                {
                    if (!m_Occupied[i])
                    {
                        m_Candidates.Add(i);
                    }
                }

            if (m_Candidates.Count == 0) return -1;
               

            int pick = m_Candidates[Random.Range(0, m_Candidates.Count)];
            m_LastHole = pick;
            return pick;
        }

        private void OnMoleFinished(MyMole mole)
        {
            mole.Finished -= OnMoleFinished;
            if (mole.HoleIndex >= 0 && mole.HoleIndex < m_Occupied.Length && m_Occupied[mole.HoleIndex])
            {
                m_Occupied[mole.HoleIndex] = false;
                m_ActiveCount--;
            }
        }
    }
}
