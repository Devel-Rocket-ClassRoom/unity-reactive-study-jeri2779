using System;
using R3;
using UnityEngine;

namespace ExtensionExamples
{
    public class SkillCooldownExample : MonoBehaviour
    {
        [SerializeField] private float cooldownSeconds = 1.5f;

        private readonly Subject<Unit> skillRequests = new Subject<Unit>();
        private int castCount;

        private void Start()
        {
            skillRequests
                .ThrottleFirst(TimeSpan.FromSeconds(cooldownSeconds))
                .Subscribe(_ =>
                {
                    castCount++;
                    Debug.Log($"[Skill] cast #{castCount}");
                })
                .AddTo(this);
        }

        [ContextMenu("Request Skill")]
        public void RequestSkill()
        {
            Debug.Log("[Skill] requested");
            skillRequests.OnNext(Unit.Default);
        }

        private void OnDestroy()
        {
            skillRequests.Dispose();
        }
    }
}
