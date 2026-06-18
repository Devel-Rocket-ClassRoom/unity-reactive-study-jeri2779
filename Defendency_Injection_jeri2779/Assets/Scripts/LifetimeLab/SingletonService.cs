namespace DIStudy.LifetimeLab
{
    /// <summary>
    /// Lifetime.Singleton으로 등록 — 컨테이너 트리 전체에서 인스턴스 하나를 공유한다.
    /// 자식 스코프에서 Resolve해도 같은 Id가 나온다.
    /// </summary>
    public sealed class SingletonService
    {
        public string Id { get; } = InstanceIdFactory.Next("Singleton");
    }
}
