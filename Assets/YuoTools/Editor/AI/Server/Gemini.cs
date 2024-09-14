using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace YuoTools.Extend.AI
{
    public class GeminiApi
    {
        static async Task GetAwaiter(AsyncOperation asyncOperation)
        {
            var task = new TaskCompletionSource<bool>();
            asyncOperation.completed += _ => { task.SetResult(true); };
            await task.Task;
        }

        // 流式获取响应的方法，替换了API链接
        static async IAsyncEnumerable<string> GetResponseStream(string body, List<(string, string)> headers = null)
        {
            if (AIHelper.ApiKey.IsNullOrSpace())
            {
                Debug.LogError("请输入ApiKey");
                yield break;
            }

            var url =
                $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:streamGenerateContent?alt=sse&key={AIHelper.ApiKey}";

            UnityWebRequest webRequest = new UnityWebRequest(url, "POST");

            headers ??= new List<(string, string)>
            {
                ("Content-Type", "application/json")
            };
            foreach (var header in headers)
            {
                webRequest.SetRequestHeader(header.Item1, header.Item2);
            }

            byte[] jsonToSend = new UTF8Encoding().GetBytes(body);
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.disposeDownloadHandlerOnDispose = true;
            webRequest.disposeUploadHandlerOnDispose = true;

            $"Sending request to: {webRequest.url} with body: {body} and headers: ".Log();
            // 发送请求并等待响应
            // await GetAwaiter(webRequest.SendWebRequest());

            var asyncOption = webRequest.SendWebRequest();
            var messageDelay = 100;
            int safeCount = 1000;
            string result = "";
            while (!asyncOption.isDone)
            {
                await Task.Delay(messageDelay);
                result = OperateMessage(webRequest.downloadHandler.text,
                    json => json.candidates[0].content.parts[0].text);
                result = RemoveStartSpace(result);
                yield return result;
                if (safeCount-- < 0)
                {
                    Debug.LogError("请求超时");
                    break;
                }
            }

            webRequest.Dispose();
        }

        // 流式生成内容的方法
        public static async IAsyncEnumerable<string> GenerateStream(string prompt)
        {
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            string body = JsonConvert.SerializeObject(requestBody);
            await foreach (var line in GetResponseStream(body))
            {
                // dynamic lineJson = JsonConvert.DeserializeObject(line);

                if (line != null) yield return line;
            }
        }

        // 原有的同步生成方法，保留
        public static async Task<GeminiApiResponse> Generate(string prompt)
        {
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            string body = JsonConvert.SerializeObject(requestBody);
            var responseJson = await GetResponse(body);

            // 解析返回的 JSON，提取出主要内容
            if (string.IsNullOrEmpty(responseJson)) return null;
            return JsonConvert.DeserializeObject<GeminiApiResponse>(responseJson);
        }

        // 原有的同步生成文本方法，保留
        public static async Task<string> GenerateText(string prompt)
        {
            var parsedJson = await Generate(prompt);
            if (parsedJson == null)
            {
                return "发生错误";
            }

            return parsedJson.candidates[0].content.parts[0].text;
        }

        // 原有的同步获取响应方法，保留
        static async Task<string> GetResponse(string body, List<(string, string)> headers = null)
        {
            if (AIHelper.ApiKey.IsNullOrSpace())
            {
                Debug.LogError("请输入ApiKey");
                return null;
            }

            var url =
                $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash-latest:generateContent?key={AIHelper.ApiKey}";

            UnityWebRequest webRequest = new UnityWebRequest(url, "POST");

            headers ??= new List<(string, string)>
            {
                ("Content-Type", "application/json")
            };
            foreach (var header in headers)
            {
                webRequest.SetRequestHeader(header.Item1, header.Item2);
            }

            byte[] jsonToSend = new UTF8Encoding().GetBytes(body);
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.disposeDownloadHandlerOnDispose = true;
            webRequest.disposeUploadHandlerOnDispose = true;

            await GetAwaiter(webRequest.SendWebRequest());

            string result = "";
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    result = webRequest.downloadHandler.text;
                    break;
            }

            webRequest.Dispose();
            return result;
        }

        public static string RemoveStartSpace(string str)
        {
            int i = 0;
            for (; i < str.Length; i++)
            {
                if (str[i] != ' ' && str[i] != '\n')
                    break;
            }

            return str.Substring(i);
        }

        public static string OperateMessage(string str, StringAction<dynamic> onJson)
        {
            string split = "data:";
            var lines = str.Split(new[] { split }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder result = new StringBuilder();
            foreach (var line in lines)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        var trimmedLine = line.Trim();
                        if (trimmedLine.StartsWith("{") && trimmedLine.EndsWith("}"))
                        {
                            dynamic json = JsonConvert.DeserializeObject(trimmedLine);
                            if (json != null)
                            {
                                result.Append(onJson?.Invoke(json));
                            }
                        }
                    }
                }
                catch (JsonReaderException e)
                {
                    Debug.LogError($"JSON解析错误: {e.Message} - Line: {line}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"处理错误: {e.Message}");
                }
            }

            return result.ToString();
        }


        [Serializable]
        public class GeminiApiResponse
        {
            public List<Candidate> candidates;
            public UsageMetadata usageMetadata;
        }

        [Serializable]
        public class Candidate
        {
            public Content content;
            public string finishReason;
            public int index;
            public List<SafetyRating> safetyRatings;
        }

        [Serializable]
        public class Content
        {
            public List<Part> parts;
            public string role;
        }

        [Serializable]
        public class Part
        {
            public string text;
        }

        [Serializable]
        public class SafetyRating
        {
            public string category;
            public string probability;
        }

        [Serializable]
        public class UsageMetadata
        {
            public int promptTokenCount;
            public int candidatesTokenCount;
            public int totalTokenCount;
        }
    }
}