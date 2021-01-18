using UnityEngine;

namespace Dirty.Test
{
    public class StateMachineBehaviour : MonoBehaviour
    {
        // [TypeSelector(typeof(State))]
        // public State states;
        //
        // [TypeSelector(typeof(State))]
        // public Type type;
        //
        // [TypeSelector(typeof(State))]
        // public RunState run;
        //
        // private Patterns.StateMachine.StateMachine stateMachine;
        //
        // private void Awake()
        // {
        //     stateMachine = new Patterns.StateMachine.StateMachine(
        //         new IdleState(),
        //         new JumpState(),
        //         // new IdleState());
        //         // new IdleState(),
        //         new RunState());
        //     
        //     // stateMachine.
        //
        //     stateMachine.StateChanged += (previousState, currentState) => 
        //         Debug.Log(previousState.GetType().Name + " -> " + currentState.GetType().Name);
        //     
        //     stateMachine.EnterState<IdleState>();
        //     stateMachine.EnterState<RunState>();
        // }
        //
        // protected void Update()
        // {
        //     stateMachine.Update(Time.deltaTime);
        // }
    }
}