// Copyright © 2020 Bogdan Nikolayev <bodix321@gmail.com>
// All Rights Reserved

using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Obsolete.Components
{
    [Obsolete("Use PeriodicBehaviour instead.")]
    public class PeriodicCoroutineBehaviour : MonoBehaviour
    {
        public float Period = 1f;

        [Tooltip("If set to \"true\", then you don't need to wait for end of current period " +
            "or manually restart to apply the new period. Set to \"false\" to reduce allocations.")]
        [ShowIf("ShowAutoApplyInInspector")]
        public bool AutoApplyPeriodChange = false;

        [ShowIf("ShowEventInInspector")]
        public UnityEvent OnPeriod;

        private Coroutine periodCoroutine;
        private float lastPeriod;

        protected virtual bool ShowAutoApplyInInspector => GetType() == typeof(PeriodicCoroutineBehaviour);
        protected virtual bool ShowEventInInspector => GetType() == typeof(PeriodicCoroutineBehaviour);
        private bool IsPeriodChanged => Math.Abs(Period - lastPeriod) >= 0.001f;

        protected virtual void OnEnable()
        {
            periodCoroutine = StartCoroutine(PeriodCoroutine());
        }

        protected virtual void OnDisable()
        {
            StopCoroutine(periodCoroutine);
        }

        protected virtual void Update()
        {
            if (AutoApplyPeriodChange && IsPeriodChanged)
                Restart();
        }

        public void Restart()
        {
            if (!enabled)
                return;
        
            StopCoroutine(periodCoroutine);
            periodCoroutine = StartCoroutine(PeriodCoroutine());
        }

        private IEnumerator PeriodCoroutine()
        {
            WaitForSeconds waitForPeriod = new WaitForSeconds(Period);
            lastPeriod = Period;

            while (enabled)
            {
                if (IsPeriodChanged)
                {
                    waitForPeriod = new WaitForSeconds(Period);
                    lastPeriod = Period;
                }

                yield return waitForPeriod;

                OnPeriod?.Invoke();
            }
        }
    }
}