using UnityEngine;

namespace Venice
{
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyBallerina : BallerinaEntity
    {
        [Header("Enemy Visual")]
        public BallerinaVisual EnemyVisual;

        [Header("Enemy Ground Check")]
        public LayerMask GroundMask;
        public float GroundCheckDistance = 0.18f;
        public float GroundCheckRadius = 0.28f;
        public Transform GroundCheckPoint;

        public override void Init()
        {
            base.Init();

            if (EnemyVisual == null)
                EnemyVisual = Visual;

            if (EnemyVisual == null)
                EnemyVisual = GetComponentInChildren<BallerinaVisual>();

            if (Attributes.MaxHealth <= 0)
                Attributes.MaxHealth = 100;

            if (Attributes.CurrentHealth <= 0)
                Attributes.CurrentHealth = Attributes.MaxHealth;
        }

        protected override void Update()
        {
            base.Update();
        }

        private void LateUpdate()
        {
            UpdateEnemyGrounded();
        }

        private void UpdateEnemyGrounded()
        {
            Vector3 origin;

            if (GroundCheckPoint != null)
            {
                origin = GroundCheckPoint.position;
            }
            else if (PlayerCollider != null)
            {
                Bounds b = PlayerCollider.bounds;
                origin = new Vector3(b.center.x, b.min.y + 0.05f, b.center.z);
            }
            else
            {
                origin = transform.position + Vector3.down * 0.9f;
            }

            bool grounded = Physics.CheckSphere(
                origin,
                GroundCheckRadius,
                GroundMask,
                QueryTriggerInteraction.Ignore
            );

            //Collision.Grounded = grounded;
            Attributes.Grounded = grounded;
        }

        public override void OnHit(HitInfo info)
        {
            BallerinaEntity attacker = info.SourceEntity;

            if (attacker != null && attacker.Team == Team)
            {
                Debug.Log($"Enemy ignored friendly hit from {attacker.name}");
                return;
            }

            base.OnHit(info);

            Debug.Log("Ouch, enemy ballerina was hit!");

            Attributes.Damaged = true;
            GetComponent<AIStateMachine>()?.Stun();
            Vector3 dir = transform.position - info.SourcePosition;
            dir.y = 0f;

            if (Rb != null && dir.sqrMagnitude > 0.001f)
                Rb.AddForce(dir.normalized * info.KnockbackForce, ForceMode.Impulse);

            if (Rb != null)
                Rb.AddForce(Vector3.up * info.KnockbackForce, ForceMode.Impulse);

            EnemyVisual?.ApplySquashAndStretch(1.1f, 0.2f);
        }
    }
}