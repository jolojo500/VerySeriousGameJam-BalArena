using System;
using System.Collections;
using UnityEngine;

namespace Venice
{
    [Serializable]
    public class ComboController : Controller<BallerinaEntity>
    {
        public BallerinaEntity BallerinaEntity => Context;

        public int ComboCount = 0;
        public float ComboTimer = 0f;
        public override void Init(BallerinaEntity ballerina)
        {
            Context = ballerina;
        }


        public void ConfirmHit()
        {
            ComboCount++;
            ComboTimer = .7f;

            if (BallerinaEntity == Player.Instance)
            {
                if(ComboCount >= 3)
                {
                    UIManager.Instance.ComboCountText.SetText(ComboCount+"!");
                    UIManager.Instance.ComboCountText.ApplySquashAndStretch(1.1f, .2f);
                }
            }
        }



        public override void OnUpdate()
        {
            if (BallerinaEntity.HandleTimer(ref ComboTimer))
            {
                ComboCount = 0;
                UIManager.Instance.ComboCountText.SetText("");
            }
        }


        public override void OnFixedUpdate()
        {

        }
    }
}