using System;
using _WIP.Utilities;
using Evolutex.Evolunity.Utilities;
using UnityEngine;

namespace _WIP.Networking
{
    // // [Frame 1 (Rpc) -> (SyncVar) -> (Hook)] ---> [Frame 2 (Rpc) -> (SyncVar) -> (Hook)] ---> ...
    // // https://mirror-networking.gitbook.io/docs/guides/execution-order
    // [RequireComponent(typeof(Timer))]
    // public class ServerRoundTimer : NetworkBehaviour
    // {
    //     public float RoundTime = 300f;
    //     public float PreRoundTime = 15f;
    //     /// <summary>
    //     /// TODO:
    //     /// After moving invoking state events to OnCurrentStateSync it
    //     /// should be removed due to <see cref="NetworkBehaviour.syncInterval"/>
    //     /// </summary>
    //     [SerializeField]
    //     private float _syncDeltaTime = 5f;
    //     
    //     [Header("Debug Mode")]
    //     [Tooltip("Reduces timer time for convenient testing")]
    //     [SyncVar]
    //     [SerializeField]
    //     private bool _debugMode = false;
    //     [SerializeField]
    //     private float _debugTimerTime = 5f;
    //     
    //     [SyncVar(hook = nameof(OnRemainingTimeSync))]
    //     private float _syncedRemainingTime;
    //     private float _lastRemainingTimeSync;
    //     [SyncVar(hook = nameof(OnCurrentStateSync))]
    //     private State _syncedCurrentState = State.NotStarted;
    //     private Timer _timer;
    //
    //     public event Action PreRoundTimerStarted;
    //     public event Action<float> PreRoundTimeSynced;
    //     public event Action RoundStarted;
    //     public event Action<float> RoundTimeSynced;
    //     public event Action RoundCompleted;
    //
    //     public State CurrentState => _syncedCurrentState;
    //     public float RemainingTime => _syncedRemainingTime;
    //     public bool IsRoundStarted => _syncedCurrentState == State.RoundStarted;
    //     public bool IsPreRoundStarted => _syncedCurrentState == State.PreRoundStarted;
    //
    //     private void Awake()
    //     {
    //         _timer = GetComponent<Timer>();
    //     }
    //
    //     /// <summary>
    //     /// TODO:
    //     /// After moving invoking state events to OnCurrentStateSync it
    //     /// should be removed due to <see cref="NetworkBehaviour.syncInterval"/>
    //     /// </summary>
    //     private void OnValidate()
    //     {
    //         syncInterval = 0;
    //     }
    //
    //     [Server]
    //     public void StartPreRoundTimer()
    //     {
    //         _timer.Start(_debugMode ? _debugTimerTime : PreRoundTime,
    //             onStart: () =>
    //             {
    //                 // Remaining time should be up to date on start.
    //                 UpdateRemainingTime();
    //                 _syncedCurrentState = State.PreRoundStarted;
    //
    //                 // TODO: Invoke this method inside OnStateSync hook.
    //                 Delay.ForOneFrame(() => 
    //                     PreRoundTimerStarted?.Invoke());
    //             },
    //             onUpdate: deltaTime => TryUpdateRemainingTime(),
    //             onComplete: StartRoundTimer);
    //     }
    //
    //     [Server]
    //     public void StartRoundTimer()
    //     {
    //         _timer.Start(_debugMode ? _debugTimerTime : RoundTime,
    //             onStart: () =>
    //             {
    //                 // Remaining time should be up to date on start.
    //                 UpdateRemainingTime();
    //                 _syncedCurrentState = State.RoundStarted;
    //                 
    //                 // TODO: Invoke this method inside OnStateSync hook.
    //                 Delay.ForOneFrame(() => 
    //                     RoundStarted?.Invoke());
    //             },
    //             onUpdate: deltaTime => TryUpdateRemainingTime(),
    //             onComplete: () =>
    //             {
    //                 _syncedCurrentState = State.NotStarted;
    //                 
    //                 // TODO: Invoke this method inside OnStateSync hook.
    //                 Delay.ForOneFrame(() => 
    //                     RoundCompleted?.Invoke());
    //             });
    //     }
    //
    //     private void OnRemainingTimeSync(float oldTime, float newTime)
    //     {
    //         // Debug.Log("Remaining time updated: " + newTime);
    //         switch (_syncedCurrentState)
    //         {
    //             case State.PreRoundStarted:
    //                 PreRoundTimeSynced?.Invoke(newTime);
    //                 break;
    //             case State.RoundStarted:
    //                 RoundTimeSynced?.Invoke(newTime);
    //                 break;
    //             case State.NotStarted:
    //                 break;
    //             default:
    //                 throw new ArgumentOutOfRangeException();
    //         }
    //     }
    //
    //     private void OnCurrentStateSync(State oldState, State newState)
    //     {
    //         // Debug.Log("Current state updated: " + newState);
    //     }
    //
    //     /// <summary>
    //     /// TODO:
    //     /// After moving invoking state events to OnCurrentStateSync it
    //     /// should be removed due to <see cref="NetworkBehaviour.syncInterval"/>
    //     /// </summary>
    //     [Server]
    //     private void TryUpdateRemainingTime()
    //     {
    //         if (Time.time - _lastRemainingTimeSync > _syncDeltaTime)
    //         {
    //             UpdateRemainingTime();
    //             
    //             _lastRemainingTimeSync = Time.time;
    //         }
    //     }
    //
    //     [Server]
    //     private void UpdateRemainingTime()
    //     {
    //         _syncedRemainingTime = _timer.RemainingTime;
    //     }
    //
    //     public enum State : byte
    //     {
    //         NotStarted,
    //         PreRoundStarted,
    //         RoundStarted
    //     }
    // }
}