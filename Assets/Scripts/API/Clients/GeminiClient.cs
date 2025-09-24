using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class GeminiClient
{
    private readonly GeminiConfig config;
    private readonly Imagen4Service imagenService;
    public Imagen4Service Imagen4Service => imagenService;
    
    private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models";
    
    public GeminiClient(GeminiConfig config)
    {
        this.config = config;
        this.imagenService = new Imagen4Service(config);
        Debug.Log("GeminiClient Generated:" + this.config);
    }
    
    [Serializable] class Part { public string text; }
    [Serializable] class Content { public string role = "user"; public Part[] parts; }
    [Serializable] class RequestBody { public Content[] contents; }
    // Response structure (only taking the part we need)
    [Serializable] class ResponsePart { public string text; }
    [Serializable] class ResponseContent { public ResponsePart[] parts; }
    [Serializable] class Candidate { public ResponseContent content; }
    [Serializable] class ResponseRoot { public Candidate[] candidates; }
    
    /// <summary>
    /// Send a "question" and return Gemini's text response
    /// </summary>
    public System.Threading.Tasks.Task<string> AskAsync(string question, int? timeoutMs = null)
    {
        var tcs = new System.Threading.Tasks.TaskCompletionSource<string>();
        var actualTimeout = timeoutMs ?? config.DefaultTimeoutMs;

        // Construct request body
        var body = new RequestBody {
            contents = new [] {
                new Content {
                    role = "user",
                    parts = new [] { new Part { text = question } }
                }
            }
        };
        var json = JsonUtility.ToJson(body);
        var url = $"{BaseUrl}/{config.Model}:generateContent?key={config.ApiKey}";

        var req = new UnityWebRequest(url, "POST");
        byte[] raw = Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(raw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        req.timeout = Mathf.CeilToInt(actualTimeout / 1000f);

        // Use MonoBehaviourRunner to execute coroutine
        MonoBehaviourRunner.Instance.StartCoroutine(Send(req, tcs));
        return tcs.Task;
    }

    private System.Collections.IEnumerator Send(UnityWebRequest req, System.Threading.Tasks.TaskCompletionSource<string> tcs)
    {
        yield return req.SendWebRequest();

#if UNITY_2020_2_OR_NEWER
        bool hasError = req.result != UnityWebRequest.Result.Success;
#else
        bool hasError = req.isHttpError || req.isNetworkError;
#endif
        if (hasError)
        {
            tcs.SetException(new Exception($"HTTP {(int)req.responseCode}: {req.error}\n{req.downloadHandler?.text}"));
            yield break;
        }

        var text = req.downloadHandler.text;
        try
        {
            var resp = JsonUtility.FromJson<ResponseRoot>(text);
            string answer = "(empty)";
            if (resp?.candidates != null && resp.candidates.Length > 0 &&
                resp.candidates[0].content?.parts != null && resp.candidates[0].content.parts.Length > 0)
            {
                answer = resp.candidates[0].content.parts[0].text;
            }
            tcs.SetResult(answer);
        }
        catch (Exception e)
        {
            tcs.SetException(new Exception("Parse error: " + e.Message + "\nRaw: " + text));
        }
    }
    
    public async System.Threading.Tasks.Task<Texture2D[]> GeneratePic(string prompt, int? count = null, string aspectRatio = null)
    {
        var actualCount = count ?? config.DefaultImageCount;
        var actualAspectRatio = aspectRatio ?? config.DefaultAspectRatio;
        
        var data = await imagenService.GenerateImagesImagenAsync(prompt, actualCount, actualAspectRatio);
        return data;
    }
}
