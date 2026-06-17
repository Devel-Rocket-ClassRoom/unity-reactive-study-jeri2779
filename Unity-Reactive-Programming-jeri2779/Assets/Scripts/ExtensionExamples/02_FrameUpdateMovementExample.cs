using R3;
using UnityEngine;

namespace ExtensionExamples
{
    public class FrameUpdateMovementExample : MonoBehaviour
    {
        [SerializeField] private Vector3 direction = Vector3.right;
        [SerializeField] private float speed = 2f;

        private void Start()
        {
            Observable
                .EveryUpdate()
                .Subscribe(_ => transform.position += direction.normalized * speed * Time.deltaTime)
                .AddTo(this);
        }
    }
}
