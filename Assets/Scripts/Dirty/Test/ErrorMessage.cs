// Evolunity for Unity
// Copyright © 2020 Bogdan Nikolayev <bodix321@gmail.com>

using System;
using System.Collections;
using Evolutex.Evolunity.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Dirty.Test
{
    public class ErrorMessage : MonoBehaviour
    {
        [SerializeField]
        private Text text = null;

        [Header("No internet")]
        [SerializeField]
        private float reconnectFrequency = 0.5f;
        [SerializeField]
        private string noInternetMessage = "No internet connection...";

        private void Awake()
        {
            gameObject.SetActive(false);

            DontDestroyOnLoad(gameObject);
        }

        public void ShowNoInternet(Action onInternetReached)
        {
            Show(noInternetMessage);

            StartCoroutine(CheckInternetConnection(onInternetReached));
        }

        public void Show(string errorMessage)
        {
            text.text = errorMessage;

            Show();
        }

        public void Show()
        {
            if (gameObject.activeInHierarchy)
                throw new InvalidOperationException("Error message is already shown");

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private IEnumerator CheckInternetConnection(Action onInternetReached)
        {
            while (!Validate.InternetConnection())
                yield return new WaitForSeconds(reconnectFrequency);

            Hide();

            onInternetReached?.Invoke();
        }
    }
}