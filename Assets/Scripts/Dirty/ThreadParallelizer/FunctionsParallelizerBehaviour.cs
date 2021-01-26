using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
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

        [Button("Get JSON async (client)")]
        public async Task TestClientAsync()
        {
            HttpClient client = new HttpClient();
            for (int i = 0; i < count; i++)
            {
                Debug.Log((await client.GetAsync(jsonUrl)).Content.ReadAsStringAsync().Result);
            }
        }
        
        [Button("Get image async")]
        public async Task TestRequestImageAsync()
        {
            for (int i = 0; i < count; i++)
            {
                await CreateImageAsync(await AsyncHttpRequest.GetByteArray(fileUrl));
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

        private void CreateImage(byte[] bytes)
        {
            InstantiateImage(bytes);
        }
        
        private async Task CreateImageAsync(byte[] bytes)
        {
            await Task.Run(() => InstantiateImage(bytes)).ConfigureAwait(false);
        }

        private void InstantiateImage(byte[] bytes)
        {
            Image image = Instantiate(imagePrefab, transform);

            Debug.Log("VAR");
            image.sprite = bytes.ToSprite();
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