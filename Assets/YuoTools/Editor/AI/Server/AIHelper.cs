using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace YuoTools.Extend.AI
{
    public static class AIHelper
    {
        public static string ApiKey => YuoToolsSettings.GetOrCreateSettings().AIApiKey;

        public static async Task<string> GenerateText(string prompt)
        {
            switch (YuoToolsSettings.GetOrCreateSettings().AIServer)
            {
                case "gemini":
                    return await GeminiApi.GenerateText(prompt);
                default:
                    return "没有配置正确的服务商";
            }
        }

        public static async IAsyncEnumerable<string> GenerateTextStream(string prompt)
        {
            switch (YuoToolsSettings.GetOrCreateSettings().AIServer)
            {
                case "gemini":
                    await foreach (var line in GeminiApi.GenerateStream(prompt))
                    {
                        yield return line;
                    }
                    break;
                default:
                    yield return "没有配置正确的服务商";
                    break;
            }
        }
    }
}