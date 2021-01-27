using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Dirty.Http;
using Evolutex.Evolunity.Extensions;
using NaughtyAttributes;
using UnityEngine;
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
        private string jsonUrl =
            "https://api.github.com/emojis";
        [SerializeField]
        private Slider slider;

        private async void Start()
        {
            // await Test();

            // ThreadPool.QueueUserWorkItem();

            // var a = new Thread(new ParameterizedThreadStart(async o => await Test()));
            // a.Start();
            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                Debug.Log($"Unobserved Task Exception : {e.Exception.Message}");
                //to actually observe the task, uncomment the below line of code
                //e.SetObserved();
            };
        }

        private void Update()
        {
            slider.value += Time.deltaTime / 10;
        }

        [Button("Get JSON async (client)")]
        public async Task TestClientAsync()
        {
            HttpClient client = new HttpClient();
            for (int i = 0; i < count; i++)
            {
                Debug.Log((await client.GetAsync(fileUrl)).ToString());
            }
        }

        [Button("Get image async")]
        public async Task TestRequestImageAsync()
        {
            for (int i = 0; i < count; i++)
            {
                try
                {
                    Image image = await AsyncHttpRequest.Get(fileUrl, CreateImageAsync);
                }
                catch (Exception e)
                {
                    Debug.Log(e);

                    throw;
                }
            }
        }

        [Button("Get JSON async")]
        public async Task TestJsonAsync()
        {
            for (int i = 0; i < count; i++)
            {
                Debug.Log(await AsyncHttpRequest.Get(jsonUrl));
            }
        }

        private Image CreateImage(byte[] bytes)
        {
            return InstantiateImage(bytes);
        }

        private async Task<Image> CreateImageAsync(byte[] bytes)
        {
            return await Task.Run(() => InstantiateImage(bytes));
        }

        private Image InstantiateImage(byte[] bytes)
        {
            Image image = Instantiate(imagePrefab, transform);

            image.sprite = bytes.ToSprite();

            return image;
        }

        private void LogJson(Dictionary<string, string> dicti)
        {
            Debug.Log(dicti.AsString(x => x.Key + " : " + x.Value));
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