using System;
using UnityEngine;

namespace DIStudy.LifetimeLab
{
    /// <summary>
    /// Lifetime.Scoped로 등록 — 스코프(LifetimeScope)마다 인스턴스 하나.
    /// 자식 스코프에서 Resolve하면 새 Id, 스코프 Dispose 시 함께 Dispose된다.
    /// </summary>
    public sealed class ScopedService : IDisposable
    {
        public string Id { get; } = InstanceIdFactory.Next("Scoped");

        public void Dispose()
        {
            Debug.Log($"[ScopedService] {Id} Dispose — 소속 스코프가 폐기되었습니다.");
        }
    }
}
