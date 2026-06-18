using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace DIStudy.CoinClicker.Student
{
    public class MyCoinClickRouter : MonoBehaviour
    {
        [SerializeField]
        private LayerMask m_ClickMask = ~0;

        private void Update()
        {
            if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
                return;

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            if (Camera.main == null)
                return;

            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, m_ClickMask))
                return;

            MyCoin coin = hit.collider.GetComponentInParent<MyCoin>();
            if (coin != null)
                coin.Collect();
        }
    }
}
