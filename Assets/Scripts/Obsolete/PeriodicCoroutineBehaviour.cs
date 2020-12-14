/*
 * Copyright (C) 2020 by Evolutex - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Bogdan Nikolaev <bodix321@gmail.com>
*/

using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

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