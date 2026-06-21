using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles collision with objects and more. Courtesy of Strix
/// </summary>
namespace Venice
{
    public class RColCallback : Entity
    {
        public delegate void CollisionEvent(UnityEngine.Collision Col);
        public delegate void TriggerEvent(Collider Col);

        public event CollisionEvent COnEnter, COnStay, COnExit;
        public event TriggerEvent TOnEnter, TOnStay, TOnExit;

        public void OnCollisionEnter(UnityEngine.Collision collision) => COnEnter?.Invoke(collision);
        public void OnCollisionStay(UnityEngine.Collision collision) => COnStay?.Invoke(collision);
        public void OnCollisionExit(UnityEngine.Collision collision) => COnExit?.Invoke(collision);

        public void OnTriggerEnter(Collider collider) => TOnEnter?.Invoke(collider);
        public void OnTriggerStay(Collider collider) => TOnStay?.Invoke(collider);
        public void OnTriggerExit(Collider collider) => TOnExit?.Invoke(collider);
    }
}