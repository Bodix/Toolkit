using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
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

        HttpClient client = new HttpClient();

        private async void Start()
        {
            // await Test();

            // ThreadPool.QueueUserWorkItem();

            // var a = new Thread(new ParameterizedThreadStart(async o => await Test()));
            // a.Start();

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                Debug.LogError($"UnobservedTaskException:\n{e.Exception.InnerException}");
            };
        }

        private void Update()
        {
            slider.value += Time.deltaTime / 10;
        }

        [Button("Get JSON async (client)")]
        public async Task GetJsonAsyncWithClient()
        {
            for (int i = 0; i < count; i++)
            {
                var content = (await client.GetAsync("http://webcode.me"));
                Debug.Log(content);

                Debug.Log((await client.GetAsync(fileUrl)).ToString());
            }
        }

        [Button("Get image async")]
        public async Task GetImageAsync()
        {
            for (int i = 0; i < count; i++)
            {
                try
                {
                    await AsyncHttpRequest.Get(fileUrl, InstantiateImage);
                }
                catch (Exception e)
                {
                    Debug.Log(e);

                    throw;
                }
            }
        }

        [Button("Get images parallel")]
        public void GetImagesInParallel()
        {
            List<Action> actions = new List<Action>();
            for (int i = 0; i < count; i++)
            {
                // actions.Add(async () => await AsyncHttpRequest.Get(fileUrl, InstantiateImage));
                actions.Add(async () =>
                {
                    try
                    {
                        InstantiateImage(await client.GetByteArrayAsync(fileUrl));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                });
            }

            Debug.Log(actions.Count);

            Parallel.Invoke(actions.ToArray());
        }

        [Button("Get JSON async")]
        public async Task GetJsonAsync()
        {
            for (int i = 0; i < count; i++)
            {
                Debug.Log(await AsyncHttpRequest.Get(jsonUrl));
            }
        }

        private void CreateImage(byte[] bytes)
        {
            InstantiateImage(bytes);
        }

        private async Task<Image> CreateImageAsync(byte[] bytes)
        {
            return await Task.Run(() => InstantiateImage(bytes));
        }

        private Image InstantiateImage(byte[] bytes)
        {
            Image image = Instantiate(imagePrefab, transform);

            image.sprite = bytes.ToSprite();

            Debug.Log("Instantiated");
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