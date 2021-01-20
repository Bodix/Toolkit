using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public delegate void ResponseHandler(UnityWebRequest response);
public delegate void ResponseWWW(WWW www);
public delegate void boolResponse(bool success, string message = "");

namespace u57
{
    public class RequestManager : MonoBehaviour
    {
        protected string apiUrl = "https://api.pixl.antsylabs.com/api";
        protected int timeout = 100;

        protected IEnumerator SendPostRequestText(string url, Dictionary<string, string> keyValueData, ResponseHandler handleResponse)
        {
            string requestText = string.Empty;
            foreach (var keyValuePair in keyValueData)
                requestText += keyValuePair.Value + "&";

            requestText = requestText.TrimEnd('&');

            Debug.Log("POST data: " + requestText);

            using (UnityWebRequest request = UnityWebRequest.Put(url, Encoding.ASCII.GetBytes(requestText.ToCharArray())))
            {
                request.timeout = timeout;
                request.method = UnityWebRequest.kHttpVerbPOST;
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.Log("Code: " + request.responseCode);
                    Debug.Log("Output: " + request.downloadHandler.text);
                }
                else
                {
                    //Debug.Log("Request successful. Output: " + request.downloadHandler.text);
                }

                if (request != null && Application.internetReachability != NetworkReachability.NotReachable)
                    handleResponse(request);

                else Debug.Log("Response is empty");
            }
        }

        protected IEnumerator SendPostRequestForm(string url, Dictionary<string, string> formDataText, Dictionary<string, byte[]> formDataFiles, ResponseHandler handleResponse)
        {
            WWWForm wwwForm = new WWWForm();
            foreach (var keyValuePair in formDataText)
                wwwForm.AddField(keyValuePair.Key, keyValuePair.Value);
            foreach (var keyValuePair in formDataFiles)
                wwwForm.AddBinaryData(keyValuePair.Key, keyValuePair.Value);


            using (UnityWebRequest request = UnityWebRequest.Post(url, wwwForm))
            {
                request.timeout = timeout;
                request.SetRequestHeader("Authorization", "Bearer ");

                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.Log("Code: " + request.responseCode);
                    Debug.Log("Output: " + request.downloadHandler.text);
                }
                else
                {
                    Debug.Log("Code: " + request.responseCode);
                    Debug.Log("Request successful. Output: " + request.downloadHandler.text);
                }

                if (request != null && Application.internetReachability != NetworkReachability.NotReachable)
                    handleResponse(request);
                else Debug.Log("Response is empty");
            }
        }

        protected IEnumerator SendGetRequest(string url, Dictionary<string, string> keyValueData, bool auth, ResponseHandler handleResponse)
        {
            string form = "?";

            foreach (var keyValuePair in keyValueData)
                form += keyValuePair.Key + "=" + keyValuePair.Value + "&";

            form = form.TrimEnd('&');

            string fullUrl = url + form;

            using (UnityWebRequest request = UnityWebRequest.Get(fullUrl))
            {
                request.timeout = timeout;
                request.SetRequestHeader("Content-Type", "application/json");

                try
                {
                    if (auth)
                        request.SetRequestHeader("Authorization", "Bearer ");
                }
                catch (NullReferenceException ex)
                {
                    Debug.Log(ex.Message);
                }

                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.Log("Code: " + request.responseCode);
                    Debug.Log("Output: " + request.downloadHandler.text);
                }
                else
                {
                    //Debug.Log("Request successful. Output: " + request.downloadHandler.text);
                }

                if (request != null && Application.internetReachability != NetworkReachability.NotReachable)
                    handleResponse(request);

                else Debug.Log("Response is empty");
            }
        }

        protected IEnumerator SendDeleteRequest(string url, Dictionary<string, string> keyValueData, ResponseHandler handleResponse)
        {
            string requestText = string.Empty;
            foreach (var keyValuePair in keyValueData)
                requestText += keyValuePair.Value + "&";
            requestText = requestText.TrimEnd('&');

            using (UnityWebRequest request = UnityWebRequest.Put(url, Encoding.ASCII.GetBytes(requestText.ToCharArray())))
            {
                request.timeout = timeout;
                request.method = UnityWebRequest.kHttpVerbDELETE;
                request.SetRequestHeader("Authorization", "Bearer ");

                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.Log("Code: " + request.responseCode);
                    Debug.Log("Output: " + request.downloadHandler.text);
                }
                else
                {
                    //Debug.Log("Request successful. Output: " + request.downloadHandler.text);
                }

                if (request != null && Application.internetReachability != NetworkReachability.NotReachable)
                    handleResponse(request);

                else Debug.Log("Response is empty");
            }
        }
    }
}