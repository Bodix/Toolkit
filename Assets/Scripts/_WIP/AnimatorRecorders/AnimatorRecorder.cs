using Bodix.Evolunity.Extensions;
using UnityEditor.Animations;
using UnityEngine;

namespace _WIP.AnimatorRecorders
{
    public class AnimatorRecorder : MonoBehaviour
    {
        [SerializeField]
        private Animator _animator;
        
        private void Awake()
        {
            AnimatorController controller = (AnimatorController)_animator.runtimeAnimatorController;
            AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
            Debug.Log(stateMachine.states.AsString());
        }
    }
}