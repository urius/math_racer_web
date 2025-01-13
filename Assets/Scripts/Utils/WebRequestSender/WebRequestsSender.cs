using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Utils.WebRequestSender
{
    public static class WebRequestsSender
    {
        private static readonly int[] RetryMsArray = { 0, 500, 1000, 5000 };

        public static async UniTask<WebRequestResult<T>> GetAsync<T>(string url)
        {
            var resultStr = await GetAsync(url);
            return HandleGenericResponse<T>(resultStr);
        }

        public static UniTask<WebRequestResult<string>> GetAsync(string url)
        {
            return SendRequestAsync(() => UnityWebRequest.Get(url));
        }

        public static async UniTask<WebRequestResult<T>> PostAsync<T>(string url, string postData)
        {
            var resultStr = await PostAsync(url, postData);
            return HandleGenericResponse<T>(resultStr);
        }

        public static UniTask<WebRequestResult<string>> PostAsync(string url, string postData)
        {
            return SendRequestAsync(Factory);

            UnityWebRequest Factory()
            {
                var form = new WWWForm();
                form.AddField("data", postData);
                form.AddField("hash", MD5Hash(postData));
                return UnityWebRequest.Post(url, form);
            }
        }
        
        public static async UniTask<WebRequestResult<T>> PostAsync<T>(string url, Dictionary<string, string> postData)
        {
            var resultStr = await PostAsync(url, postData);
            return HandleGenericResponse<T>(resultStr);
        }
        
        public static UniTask<WebRequestResult<string>> PostAsync(string url, Dictionary<string, string> postData)
        {
            return SendRequestAsync(Factory);

            UnityWebRequest Factory()
            {
                var form = new WWWForm();
                foreach (var kvp in postData)
                {
                    form.AddField(kvp.Key, kvp.Value);
                }

                return UnityWebRequest.Post(url, form);
            }
        }

        private static async UniTask<WebRequestResult<string>> SendRequestAsync(Func<UnityWebRequest> unityWebRequestFactory)
        {
            var retryPrefix = string.Empty;
            var lastError = string.Empty;
            for (var i = 0; i < RetryMsArray.Length; i++)
            {
                var unityWebRequest = unityWebRequestFactory();
                using (unityWebRequest)
                {
                    if (RetryMsArray[i] > 0)
                    {
                        await UniTask.Delay(RetryMsArray[i]);
                        retryPrefix = $"[ Retry #{i} ]";
                    }

                    try
                    {
                        Debug.Log($"{retryPrefix}[ Request ] -> {unityWebRequest.url}\n");
                        var result = await unityWebRequest.SendWebRequest();

                        Debug.Log($"[ Response ] -> {result.downloadHandler.text} (url: {unityWebRequest.url})\n");
                        if (result.error == null)
                        {
                            return new WebRequestResult<string>(result.downloadHandler.text);
                        }
                        else
                        {
                            lastError = result.error;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }

            return WebRequestResult<string>.FromError(lastError);
        }

        private static WebRequestResult<T> HandleGenericResponse<T>(WebRequestResult<string> resultStr)
        {
            if (resultStr.IsSuccess)
            {
                T result;
                try
                {
                    result = JsonUtility.FromJson<T>(resultStr.Result);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    return new WebRequestResult<T>();
                }
                return new WebRequestResult<T>(result);
            }
            else
            {
                return new WebRequestResult<T>();
            }
        }

        private static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(Encoding.ASCII.GetBytes(text));
            var result = md5.Hash;

            var strBuilder = new StringBuilder();
            foreach (var b in result)
            {
                strBuilder.Append(b.ToString("x2"));
            }

            return strBuilder.ToString();
        }
    }

    public struct WebRequestResult<T>
    {
        public readonly bool IsSuccess;
        public readonly T Result;
        public string Error;
        
        public WebRequestResult(T result)
        {
            IsSuccess = true;
            Result = result;
            Error = null;
        }

        public static WebRequestResult<T> FromError(string error)
        {
            var result = new WebRequestResult<T>();
            result.Error = error;
            
            return result;
        }
    }
}