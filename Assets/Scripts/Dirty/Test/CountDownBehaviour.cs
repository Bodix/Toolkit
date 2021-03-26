using System;
using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dirty.Test
{
    public class CountDownBehaviour : MonoBehaviour
    {
        public float Duration = 5f;
        public bool DisableOnComplete = true;
    
        [SerializeField] private Image circleImage;
        [SerializeField] private TextMeshProUGUI countText;

        private Coroutine countDownCoroutine;
    
        public event Action Started;
        public event Action Stopped;
        public event Action Completed;
    
        public bool IsPlaying { get; private set; }
    
        public Coroutine ShowAndStart()
        {
            gameObject.SetActive(true);
        
            return StartCountDown();
        }

        public void StopAndHide()
        {
            StopCountDown();
        
            gameObject.SetActive(false);
        }

        private Coroutine StartCountDown()
        {
            return countDownCoroutine = StartCoroutine(CountDownCoroutine());
        }

        private void StopCountDown()
        {
            if (countDownCoroutine != null)
            {
                StopCoroutine(countDownCoroutine);

                IsPlaying = false;
                Stopped?.Invoke();
            }
        }

        private IEnumerator CountDownCoroutine()
        {
            IsPlaying = true;
            Started?.Invoke();
        
            for (float fromTime = Duration; fromTime >= 0; fromTime -= Time.deltaTime)
            {
                circleImage.fillAmount = fromTime / Duration;

                string text = Mathf.Ceil(fromTime).ToString(CultureInfo.InvariantCulture);
                countText.text = text;

                yield return null;
            }
        
            if (DisableOnComplete)
                gameObject.SetActive(false);

            IsPlaying = false;
            Completed?.Invoke();
        }
    }
}
