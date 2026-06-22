using System.Collections;
using UnityEngine;

namespace Venice
{
    public class Controller<T>
    {
        public T Context;
        public virtual void Init(T context)
        {
            Context = context;
        }

        public virtual void OnUpdate()
        {

        }


        public virtual void OnFixedUpdate()
        {

        }
    }
}