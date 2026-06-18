using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace DIStudy.WhackAMole
{
    /// <summary>
    /// 마우스 클릭을 레이캐스트해 두더지에게 전달한다. (CoinClicker의 클릭 라우터와 같은 역할)
    /// </summary>
    public class MyMoleClickRouter : MonoBehaviour
    {
        [SerializeField]
        private LayerMask m_ClickMask = ~0;

        private void Update()
        {
            if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame) return;

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            if (Camera.main == null) return;

            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, m_ClickMask)) return;

            MyMole mole = hit.collider.GetComponentInParent<MyMole>();
            if (mole != null) mole.Hit();
        }
    }
}
