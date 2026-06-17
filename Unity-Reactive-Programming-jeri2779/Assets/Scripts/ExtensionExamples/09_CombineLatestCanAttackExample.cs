using R3;
using UnityEngine;

namespace ExtensionExamples
{
    public class CombineLatestCanAttackExample : MonoBehaviour
    {
        private readonly ReactiveProperty<bool> hasWeapon = new ReactiveProperty<bool>(false);
        private readonly ReactiveProperty<bool> hasStamina = new ReactiveProperty<bool>(true);
        private readonly ReactiveProperty<bool> isStunned = new ReactiveProperty<bool>(false);

        private void Start()
        {
            hasWeapon
                .CombineLatest(hasStamina, isStunned, (weapon, stamina, stunned) => weapon && stamina && !stunned)
                .DistinctUntilChanged()
                .Subscribe(canAttack => Debug.Log($"[CanAttack] {canAttack}"))
                .AddTo(this);
        }

        [ContextMenu("Toggle Weapon")]
        public void ToggleWeapon()
        {
            hasWeapon.Value = !hasWeapon.Value;
        }

        [ContextMenu("Toggle Stamina")]
        public void ToggleStamina()
        {
            hasStamina.Value = !hasStamina.Value;
        }

        [ContextMenu("Toggle Stun")]
        public void ToggleStun()
        {
            isStunned.Value = !isStunned.Value;
        }

        private void OnDestroy()
        {
            hasWeapon.Dispose();
            hasStamina.Dispose();
            isStunned.Dispose();
        }
    }
}
