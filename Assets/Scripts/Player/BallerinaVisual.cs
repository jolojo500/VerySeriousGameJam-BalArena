using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Experimental.AI;

namespace Venice
{
    public class BallerinaVisual : MonoBehaviour
    {
        public BallerinaEntity Entity;
        public Transform parentObject;
        public Animator Animator;
        public Transform EntitySkin;
        public SpriteRenderer Skin;
        private int FacingDirection = 1;
        private Vector3 originalParentScale;
        // Start is called before the first frame update
        private void Start()
        {
            originalParentScale = parentObject.localScale;
        }
        public void SkinTransformUpdate()
        {
            Skin.transform.forward = Camera.main.transform.forward;
        }
        // Update is called once per frame
        private void Update()
        {
            SkinTransformUpdate();
            SetFloat("GroundSpeed", Mathf.Abs(Entity.HorizontalVelocity.magnitude));
            SetBool("isAttacking", Entity.Attributes.isAttacking);
            SetBool("Grounded", Entity.Attributes.Grounded);
            SetBool("Damaged", Entity.Attributes.Damaged);
            SetFloat("XSpeed", Mathf.Abs(Entity.XSpeed));
            SetFloat("YSpeed", Entity.YSpeed);
            SetInteger("State", Entity?.Machine?.CurrentState?.StateNumber ?? 0);
            UpdateFacing();
            if (Entity.IsInvulnerable) HandleInvulnerabilityBlink();
            if (Skin.enabled == false && !Entity.IsInvulnerable) Skin.enabled = true;

        }

        private void HandleInvulnerabilityBlink()
        {
            float ping = Mathf.PingPong(Time.time * 8, 1);
            Skin.enabled = (ping > .5f);
        }

        public void SetTrigger(string name)
        {
            if (Animator)
            {
                Animator?.SetTrigger(name);
            }
        }

        public void Play(string name)
        {
            Animator?.Play(name);
        }
        public void SetBool(string name, bool value)
        {
            Animator?.SetBool(name, value);
        }

        public void SetInteger(string name, int value)
        {
            Animator?.SetInteger(name, value);
        }
        public void SetFloat(string name, float value)
        {
            Animator?.SetFloat(name, value);
        }
        public bool IsPlaying(string idleAnim)
        {
            return Animator?.GetCurrentAnimatorStateInfo(0).IsName(idleAnim) ?? false;
        }
        private void UpdateFacing()
        {
            if (Entity.XSpeed > 0.3f)
                FacingDirection = 1;
            else if (Entity.XSpeed < -0.3f)
                FacingDirection = -1;

            Vector3 scale = parentObject.localScale;

            float targetX = Mathf.Abs(originalParentScale.x) * FacingDirection;

            scale.x = Mathf.Lerp(scale.x, targetX, Time.deltaTime * 20f);

            parentObject.localScale = scale;
        }


    }

}