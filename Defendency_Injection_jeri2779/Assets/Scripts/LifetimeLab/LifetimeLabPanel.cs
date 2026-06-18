using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace DIStudy.LifetimeLab
{
    /// <summary>
    /// Singleton/Scoped/Transient의 차이를 버튼으로 실험하는 관찰 도구.
    /// 주의: 이 패널은 시각화를 위해 Container.Resolve를 직접 호출한다.
    /// 실전 코드에서 이렇게 컨테이너를 들고 다니며 꺼내 쓰면 서비스 로케이터 안티패턴이다.
    /// </summary>
    public class LifetimeLabPanel : MonoBehaviour
    {
        private const int MaxLines = 14;

        [SerializeField]
        private LifetimeScope m_RootScope;

        [SerializeField]
        private TextMeshProUGUI m_LogText;

        [SerializeField]
        private Button m_ResolveRootButton;

        [SerializeField]
        private Button m_CreateChildButton;

        [SerializeField]
        private Button m_ResolveChildButton;

        [SerializeField]
        private Button m_DisposeChildButton;

        private LifetimeScope m_ChildScope;
        private readonly List<string> m_Lines = new List<string>();

        private void Start()
        {
            if (m_ResolveRootButton != null)
                m_ResolveRootButton.onClick.AddListener(ResolveInRoot);
            if (m_CreateChildButton != null)
                m_CreateChildButton.onClick.AddListener(CreateChild);
            if (m_ResolveChildButton != null)
                m_ResolveChildButton.onClick.AddListener(ResolveInChild);
            if (m_DisposeChildButton != null)
                m_DisposeChildButton.onClick.AddListener(DisposeChild);

            Log("버튼을 눌러 수명(Lifetime)별 인스턴스 생성을 관찰하세요.");
        }

        private void ResolveInRoot()
        {
            if (m_RootScope == null)
            {
                Log("<color=#ff5555>루트 스코프가 연결되지 않았습니다.</color>");
                return;
            }

            ResolveAndLog(m_RootScope.Container, "루트");
        }

        private void CreateChild()
        {
            if (m_RootScope == null)
            {
                Log("<color=#ff5555>루트 스코프가 연결되지 않았습니다.</color>");
                return;
            }

            if (m_ChildScope != null)
            {
                Log("이미 자식 스코프가 있습니다. 먼저 Dispose 하세요.");
                return;
            }

            // 자식은 부모의 등록을 그대로 물려받는다. 추가 등록 없이 비워 둔다.
            m_ChildScope = m_RootScope.CreateChild();
            m_ChildScope.name = "ChildScope (runtime)";
            Log("<color=#55ff55>자식 스코프 생성</color> — 부모 등록을 상속");
        }

        private void ResolveInChild()
        {
            if (m_ChildScope == null)
            {
                Log("자식 스코프가 없습니다. 먼저 생성하세요.");
                return;
            }

            ResolveAndLog(m_ChildScope.Container, "자식");
        }

        private void DisposeChild()
        {
            if (m_ChildScope == null)
            {
                Log("Dispose할 자식 스코프가 없습니다.");
                return;
            }

            m_ChildScope.Dispose();
            m_ChildScope = null;
            Log(
                "<color=#ffaa55>자식 스코프 Dispose</color> — 자식에서 Resolve했었다면 Console에 "
                    + "ScopedService Dispose 로그가 남는다 (Scoped는 첫 Resolve 때 생성되는 lazy)"
            );
        }

        private void ResolveAndLog(IObjectResolver container, string label)
        {
            var singleton = container.Resolve<SingletonService>();
            var scoped = container.Resolve<ScopedService>();
            var transient1 = container.Resolve<TransientService>();
            var transient2 = container.Resolve<TransientService>();

            Log($"[{label}] {singleton.Id} | {scoped.Id} | {transient1.Id}, {transient2.Id} (Transient는 매번 새로)");
        }

        private void Log(string line)
        {
            m_Lines.Add(line);
            if (m_Lines.Count > MaxLines)
                m_Lines.RemoveAt(0);

            if (m_LogText != null)
                m_LogText.text = string.Join("\n", m_Lines);
        }
    }
}
