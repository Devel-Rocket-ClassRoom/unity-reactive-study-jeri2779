namespace DIStudy.LifetimeLab
{
    /// <summary>
    /// Lifetime.Transient로 등록 — Resolve(주입)할 때마다 새 인스턴스를 만든다.
    /// </summary>
    public sealed class TransientService
    {
        public string Id { get; } = InstanceIdFactory.Next("Transient");
    }
}
