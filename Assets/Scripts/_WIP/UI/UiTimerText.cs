using _WIP.Utilities;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace _WIP.UI
{
    public delegate void TimerTextHandler(TextMeshProUGUI text, float time);

    public delegate float TimeGetter();
    
    // TODO: Unsubscribe from timer on destroy.
    public class UiTimerText : UiText
    {
        public TimeUpdateMethod UpdateMethod;
        [SerializeField, ShowIf(nameof(IsTimersUpdate))]
        protected Timer _timer;

        public virtual TimerTextHandler TimerTextHandler { protected get; set; } = (text, time) =>
        {
            text.text = ToStringUtility.TimeToMinutesSeconds(time);
        };
        public virtual TimeGetter TimeGetter { protected get; set; }
        public Timer Timer => _timer;
        private bool IsTimersUpdate => UpdateMethod == TimeUpdateMethod.TimersUpdate;

        protected virtual void Start()
        {
            if (_timer)
                SubscribeToTimer(_timer);
        }

        protected virtual void Update()
        {
            if (UpdateMethod != TimeUpdateMethod.InternalUpdate)
                return;

            if (TimeGetter == null)
            {
                Debug.LogError("In order to use " + nameof(TimeUpdateMethod.InternalUpdate) + ", " +
                    "it is required that " + nameof(TimeGetter) + " is not null");

                return;
            }

            UpdateTimerText(TimeGetter.Invoke());
        }

        public void SubscribeToTimer(Timer timer)
        {
            _timer = timer;
            _timer.Updated += deltaTime =>
            {
                if (UpdateMethod == TimeUpdateMethod.TimersUpdate)
                    UpdateTimerText(_timer.RemainingTime);
            };

            UpdateMethod = TimeUpdateMethod.TimersUpdate;
        }

        public void UpdateTimerText(float time)
        {
            TimerTextHandler.Invoke(Text, time);
        }

        public enum TimeUpdateMethod
        {
            ExternalManual,
            TimersUpdate,
            InternalUpdate
        }
    }

    public static class ToStringUtility
    {
        public static string TimeToMinutesSeconds(float time)
        {
            float minutes = Mathf.Max(Mathf.FloorToInt(time / 60), 0);
            float seconds = Mathf.Max(Mathf.FloorToInt(time % 60), 0);

            return $"{minutes:00}:{seconds:00}";
        }
    }
    
    // Added TimeUpdateMethod.InternalFromTimer update method.
    // 
    // public class UiTimerText : UiElement
    // {
    //     public Text timerUi;
    //     public TimeUpdateMethod UpdateMethod;
    //
    //     [SerializeField, ShowIf(nameof(IsTimersUpdate))] protected Timer _timer;
    //
    //     public virtual TimerTextHandler TimerTextHandler { protected get; set; } = (textComponent, time) =>
    //     {
    //         textComponent.text = ToStringUtility.TimeToMinutesSeconds(time);
    //     };
    //     public virtual TimeGetter TimeGetter { protected get; set; }
    //     public Timer Timer => _timer;
    //     private bool IsTimersUpdate => UpdateMethod == TimeUpdateMethod.TimerUpdate 
    //         || UpdateMethod == TimeUpdateMethod.InternalFromTimer;
    //
    //     protected virtual void Start()
    //     {
    //         if (_timer)
    //             SubscribeToTimer(_timer);
    //     }
    //
    //     protected virtual void Update()
    //     {
    //         if (UpdateMethod == TimeUpdateMethod.InternalFromTimer)
    //         {
    //             if (Timer == null)
    //             {
    //                 Debug.LogError("In order to use " + nameof(TimeUpdateMethod.InternalFromTimer) + ", " +
    //                     "it is required that " + nameof(Timer) + " is not null. Use " + nameof(SubscribeToTimer) + 
    //                     "method to assign a timer");
    //
    //                 return;
    //             }
    //             
    //             UpdateTimerText(Timer.RemainingTime);
    //         }
    //         else if (UpdateMethod == TimeUpdateMethod.InternalFromGetter)
    //         {
    //             if (TimeGetter == null)
    //             {
    //                 Debug.LogError("In order to use " + nameof(TimeUpdateMethod.InternalFromGetter) + ", " +
    //                     "it is required that " + nameof(TimeGetter) + " is not null");
    //
    //                 return;
    //             }
    //
    //             UpdateTimerText(TimeGetter.Invoke());
    //         }
    //     }
    //
    //     public void SubscribeToTimer(Timer timer, bool useTimerUpdate = true)
    //     {
    //         _timer = timer;
    //         _timer.Updated += deltaTime =>
    //         {
    //             if (UpdateMethod == TimeUpdateMethod.TimerUpdate)
    //                 UpdateTimerText(_timer.RemainingTime);
    //         };
    //
    //         UpdateMethod = useTimerUpdate ? TimeUpdateMethod.TimerUpdate : TimeUpdateMethod.InternalFromTimer;
    //     }
    //
    //     public void UpdateTimerText(float time)
    //     {
    //         TimerTextHandler.Invoke(timerUi, time);
    //     }
    //
    //     public enum TimeUpdateMethod
    //     {
    //         ExternalManual,
    //         TimerUpdate,
    //         InternalFromTimer,
    //         InternalFromGetter
    //     }
    // }
}