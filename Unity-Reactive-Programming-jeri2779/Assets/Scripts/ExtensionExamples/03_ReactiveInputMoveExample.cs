using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ExtensionExamples
{
    public class ReactiveInputMoveExample : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;

        private void Start()
        {
            Observable
                .EveryUpdate()
                .Select(_ => ReadMoveInput())
                .Where(input => input.sqrMagnitude > 0f)
                .Subscribe(input =>
                {
                    Vector3 delta = new Vector3(input.x, 0f, input.y) * speed * Time.deltaTime;
                    transform.position += delta;
                })
                .AddTo(this);
        }

        private static Vector2 ReadMoveInput()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return Vector2.zero;
            }

            Vector2 input = Vector2.zero;
            if (keyboard.wKey.isPressed) input.y += 1f;
            if (keyboard.sKey.isPressed) input.y -= 1f;
            if (keyboard.dKey.isPressed) input.x += 1f;
            if (keyboard.aKey.isPressed) input.x -= 1f;

            return input.normalized;
        }
    }
}
