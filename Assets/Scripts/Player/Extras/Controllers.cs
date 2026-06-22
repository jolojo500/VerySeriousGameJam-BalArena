using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace Venice
{
    [Serializable]
    public class Controllers<T>
    {
        T Context;
        public List<Controller<T>> ControllersList = new List<Controller<T>>();

        public void Init(T context)
        {
            Context = context;
            foreach (Controller<T> controller in ControllersList)
            {
                controller.Init(context);
            }
        }

        public void Update()
        {
            foreach (Controller<T> controller in ControllersList)
            {
                controller.OnUpdate();
            }
        }

        public void AddController(Controller<T> controller)
        {
            ControllersList.Add(controller);
            if (Context != null)
            {
                controller.Init(Context);
            }
        }


        public void FixedUpdate()
        {
            foreach (Controller<T> controller in ControllersList)
            {
                controller.OnFixedUpdate();
            }
        }

        public G GetController<G>() where G : Controller<T>
        {
            foreach (Controller<T> controller in ControllersList)
            {
                if (controller is G tController)
                {
                    return tController;
                }
            }
            return null;
        }
    }
}