using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace BattleJourney.Gameplay
{
using UnityEngine;

public class AnimatorRecorder2 : MonoBehaviour
{
    [System.Serializable]
    public struct AnimatorParamsData
    {
        public string paramName;
        public float paramValue;
    }

    [System.Serializable]
    public struct AnimatorStateData
    {
        public int layerIndex;
        public string stateName;
        public string transitionName;
        public float transitionDuration;
    }

    [System.Serializable]
    public struct AnimatorTransitionData
    {
        public int layerIndex;
        public string transitionName;
        public float transitionDuration;
    }

    public float recordInterval = 0.1f; // Interval at which to record data
    public Animator animator;

    private float recordTimer;
    private AnimatorParamsData[] paramsDataList;
    private AnimatorStateData[] stateDataList;
    private AnimatorTransitionData[] transitionDataList;

    private void Awake()
    {
        paramsDataList = new AnimatorParamsData[0];
        stateDataList = new AnimatorStateData[0];
        transitionDataList = new AnimatorTransitionData[0];
    }

    private void Start()
    {
        recordTimer = recordInterval;
    }

    private void Update()
    {
        recordTimer -= Time.deltaTime;

        if (recordTimer <= 0f)
        {
            RecordAnimator();
            recordTimer = recordInterval;
        }
    }

    private void RecordAnimator()
    {
        // Record animator parameters
        RecordAnimatorParameters();

        // Record animator states
        RecordAnimatorStates();

        // Record animator transitions
        RecordAnimatorTransitions();
    }

    private void RecordAnimatorParameters()
    {
        AnimatorControllerParameter[] parameters = animator.parameters;

        foreach (AnimatorControllerParameter param in parameters)
        {
            if (param.type == AnimatorControllerParameterType.Float)
            {
                float paramValue = animator.GetFloat(param.name);

                // Add parameter data to the list
                AddAnimatorParamData(param.name, paramValue);
            }
        }
    }

    private void AddAnimatorParamData(string paramName, float paramValue)
    {
        AnimatorParamsData paramData = new AnimatorParamsData
        {
            paramName = paramName,
            paramValue = paramValue
        };

        // Add parameter data to the list
        ArrayUtility.Add(ref paramsDataList, paramData);
    }

    private void RecordAnimatorStates()
    {
        int layerCount = animator.layerCount;

        for (int layerIndex = 0; layerIndex < layerCount; layerIndex++)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);

            string stateName = stateInfo.fullPathHash.ToString();

            // Add state data to the list
            AddAnimatorStateData(stateName, layerIndex, string.Empty, 0f);
        }
    }

    private void AddAnimatorStateData(string stateName, int layerIndex, string transitionName, float transitionDuration)
    {
        AnimatorStateData stateData = new AnimatorStateData
        {
            stateName = stateName,
            layerIndex = layerIndex,
            transitionName = transitionName,
            transitionDuration = transitionDuration
        };

        // Add state data to the list
        ArrayUtility.Add(ref stateDataList, stateData);
    }

    private void RecordAnimatorTransitions()
    {
        int layerCount = animator.layerCount;

        for (int layerIndex = 0; layerIndex < layerCount; layerIndex++)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
            AnimatorTransitionData transitionData = GetTransitionData(layerIndex);

            if (transitionData.transitionName != string.Empty)
            {
                // Add transition data to the list
                AddAnimatorTransitionData(layerIndex, transitionData.transitionName, transitionData.transitionDuration);
            }
        }
    }

    private AnimatorTransitionData GetTransitionData(int layerIndex)
    {
        AnimatorTransitionData transitionData = new AnimatorTransitionData
        {
            layerIndex = layerIndex,
            transitionName = string.Empty,
            transitionDuration = 0f
        };

        AnimatorControllerParameter[] parameters = animator.parameters;

        foreach (AnimatorControllerParameter param in parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger && animator.GetBool(param.name))
            {
                AnimatorTransitionInfo transitionInfo = animator.GetAnimatorTransitionInfo(layerIndex);

                if (transitionInfo.fullPathHash != 0)
                {
                    AnimatorController controller = (AnimatorController)animator.runtimeAnimatorController;
                    // AnimatorStateTransition transition = new ;
                    //
                    // if (transition != null)
                    // {
                    //     transitionData.transitionName = transition.name;
                    //     transitionData.transitionDuration = transition.duration;
                    // }
                }
            }
        }

        return transitionData;
    }

    private void AddAnimatorTransitionData(int layerIndex, string transitionName, float transitionDuration)
    {
        AnimatorTransitionData transitionData = new AnimatorTransitionData
        {
            layerIndex = layerIndex,
            transitionName = transitionName,
            transitionDuration = transitionDuration
        };

        // Add transition data to the list
        ArrayUtility.Add(ref transitionDataList, transitionData);
    }
}
}