using UnityEngine;

namespace Venice
{
    public class PlayerSpinAbility : MonoBehaviour
    {
        [SerializeField] private BallerinaEntity entity;
        [SerializeField] private string spinInputName = "Spin";

        private bool usedAirSpin;

        private void Awake()
        {
            if (entity == null)
                entity = GetComponent<BallerinaEntity>();
        }

        private void Update()
        {
            if (entity == null)
                return;

            if (Player.Instance == null || entity != Player.Instance)
                return;

            if (entity.Attributes.Grounded && !entity.Attributes.isSpin)
                usedAirSpin = false;

            if (!GameInput.GetButtonDown(spinInputName))
                return;

            if (!CanStartSpin())
                return;

            usedAirSpin = true;
            entity.Machine.Set<PS_Spin>();
        }

        private bool CanStartSpin()
        {
            if (entity.IsDead)
                return false;

            if (entity.Attributes.Grounded)
                return false;

            if (usedAirSpin)
                return false;

            if (entity.Attributes.isSpin)
                return false;

            if (entity.Attributes.isAttacking)
                return false;

            return true;
        }
    }
}