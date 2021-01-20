using System.Threading.Tasks;
using Evolutex.Evolunity.Extensions;
using NaughtyAttributes;
using Proyecto26;
using ShcherbakUnityAwaiter;
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
        
        [Button("Get")]
        public void Test()
        {
            RestClient.Get(new RequestHelper {
                Uri = fileUrl,
                DownloadHandler = new DownloadHandlerTexture()
            }).Then(res =>
            {
                Image image = Instantiate(imagePrefab, transform);
                // Texture2D texture = ((DownloadHandlerTexture)res.Request.downloadHandler).texture;
                //
                // image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                //     new Vector2(texture.width / 2f, texture.height / 2f));
                image.sprite = res.Request.downloadHandler.data.ToSprite();
            }).Catch(err => {
                EditorUtility.DisplayDialog ("Error", err.Message, "Ok");
            });
            
            // for (int i = 0; i < 10; i++)
            // {
            //     Task downloadImageTask = HttpRequest.Get<byte[]>()
            // }
        }
    }
}