using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Experimental.AI;

namespace Venice
{
    public class PlayerVisual : MonoBehaviour
    {
        public Player Player;
        public Animator Animator;
        public Transform PlayerSkin;
        public SpriteRenderer Skin;

        // Start is called before the first frame update
        private void Start()
        {
        }
        public void SkinTransformUpdate()
        {
            Skin.transform.forward = Player.PlayerCamera.transform.forward;
        }
        // Update is called once per frame
        private void Update()
        {
            SkinTransformUpdate();
            SetFloat("GroundSpeed", Mathf.Abs(Player.HorizontalVelocity.magnitude));
            SetBool("Grounded", Player.Attributes.Grounded);
            SetBool("Damaged", Player.Attributes.Damaged);
            SetFloat("XSpeed", Mathf.Abs(Player.XSpeed));
            SetFloat("YSpeed", Player.YSpeed);
            SetInteger("State", Player?.Machine?.CurrentState?.StateNumber ?? 0);

            if (Player.IsInvulnerable) HandleInvulnerabilityBlink();
            if (Skin.enabled == false && !Player.IsInvulnerable) Skin.enabled = true;
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
                Animator.SetTrigger(name);
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
    }

}