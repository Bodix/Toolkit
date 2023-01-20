using System;
using Evolutex.Evolunity.Patterns;
using UnityEngine;

namespace Dirty.Test
{
    [Serializable]
    public class IdleState : State
    {
        public int a;

        protected override void OnEnter()
        {
            Debug.Log("Enter Idle");
        }

        protected override void OnExit()
        {
            Debug.Log("Exit Idle");
        }
    }

    [Serializable]
    public class JumpState : State
    {
        public int a;

        protected override void OnEnter()
        {
            Debug.Log("Enter Jump");
        }

        protected override void OnExit()
        {
            Debug.Log("Exit Jump");
        }
    }

    [Serializable]
    public class RunState : State
    {
        public int a;

        protected override void OnEnter()
        {
            Debug.Log("Enter Run");
        }

        protected override void OnExit()
        {
            Debug.Log("Exit Run");
        }
    }
}