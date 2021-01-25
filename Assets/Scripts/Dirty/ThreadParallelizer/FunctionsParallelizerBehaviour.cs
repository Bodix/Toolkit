using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Evolutex.Evolunity.Extensions;
using NaughtyAttributes;
using Proyecto26;
using RSG;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Dirty.ThreadParallelizer
{
    public class FunctionsParallelizerBehaviour : MonoBehaviour
    {
        [SerializeField]
        private int count;
        [SerializeField]
        private Image imagePrefab;
        [SerializeField]
        private string fileUrl =
            "https://www.google.com.ua/images/branding/googlelogo/2x/googlelogo_color_160x56dp.png";
        [SerializeField]
        private Slider slider;
        [SerializeField]
        private Transform tr;

        private async void Start()
        {
            // await Test();

            // ThreadPool.QueueUserWorkItem();

            // var a = new Thread(new ParameterizedThreadStart(async o => await Test()));
            // a.Start();
        }

        private void Update()
        {
            tr.Translate(Time.deltaTime, 0, 0);
        }

        [Button("Get Async Client Image")]
        public async Task TestClientAsync()
        {
            HttpClient client = new HttpClient();
            for (int i = 0; i < count; i++)
            {
                CreateImage(await client.GetByteArrayAsync(fileUrl));
            }
        }
        
        [Button("Get Async Request Image")]
        public async Task TestRequestAsync()
        {
            for (int i = 0; i < count; i++)
            {
                CreateImage(await AsyncHttpRequest.GetByteArray(fileUrl));
            }
        }

        private void CreateImage(byte[] bytes)
        {
            Image image = Instantiate(imagePrefab, transform);

            Debug.Log("VAR");
            image.sprite = bytes.ToSprite();
        }

        public void TestCoroutine()
        {
            // IPromise promise = RestClient.Get(new RequestHelper {
            //     Uri = fileUrl,
            // }).Then(res =>
            // {
            //     Image image = Instantiate(imagePrefab, transform);
            //     // Texture2D texture = ((DownloadHandlerTexture)res.Request.downloadHandler).texture;
            //     //
            //     // image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
            //     //     new Vector2(texture.width / 2f, texture.height / 2f));
            //     image.sprite = res.Request.downloadHandler.data.ToSprite();
            // }).Catch(err => {
            //     EditorUtility.DisplayDialog ("Error", err.Message, "Ok");
            // }).Progress(progress =>
            // {
            //     slider.value = progress;
            // });
        }
    }
}