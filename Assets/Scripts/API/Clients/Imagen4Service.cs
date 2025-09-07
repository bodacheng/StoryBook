using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class Imagen4Service
{
    private readonly GeminiConfig config;
    
    public Imagen4Service(GeminiConfig config)
    {
        this.config = config;
    }
    
    [Serializable] class ImagenInstances { public string prompt; }
    [Serializable] class ImagenParams { public int sampleCount = 1; public string aspectRatio = "1:1"; }
    [Serializable] class ImagenRequest { public ImagenInstances[] instances; public ImagenParams parameters; }
    [Serializable] class ImagenPrediction { public string bytesBase64Encoded; public string mimeType; public string prompt; }
    [Serializable] class ImagenResponse { public ImagenPrediction[] predictions; }

    public async System.Threading.Tasks.Task<Texture2D[]> GenerateImagesImagenAsync(string prompt, int count = 1, string aspect = "1:1", int? timeoutMs = null)
    {
        var actualTimeout = timeoutMs ?? config.ImageTimeoutMs;
        var actualCount = Mathf.Clamp(count, 1, 4);
        
        var body = new ImagenRequest {
            instances = new [] { new ImagenInstances { prompt = prompt } },
            parameters = new ImagenParams { sampleCount = actualCount, aspectRatio = aspect }
        };
        var json = JsonUtility.ToJson(body);
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/imagen-4.0-generate-preview-06-06:predict?key={config.ApiKey}";

        var req = new UnityWebRequest(url, "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        req.timeout = Mathf.CeilToInt(actualTimeout / 1000f);

        var tcs = new System.Threading.Tasks.TaskCompletionSource<Texture2D[]>();
        MonoBehaviourRunner.Instance.StartCoroutine(SendImagen(req, tcs));
        return await tcs.Task;
    }

    private IEnumerator SendImagen(UnityWebRequest req, System.Threading.Tasks.TaskCompletionSource<Texture2D[]> tcs)
    {
        yield return req.SendWebRequest();
    #if UNITY_2020_2_OR_NEWER
        bool hasError = req.result != UnityWebRequest.Result.Success;
    #else
        bool hasError = req.isHttpError || req.isNetworkError;
    #endif
        if (hasError) { 
            tcs.SetException(new Exception($"HTTP {(int)req.responseCode}: {req.error}\n{req.downloadHandler?.text}")); 
            yield break; 
        }

        var text = req.downloadHandler.text;
        try
        {
            var resp = JsonUtility.FromJson<ImagenResponse>(text);
            if (resp?.predictions == null || resp.predictions.Length == 0) 
                throw new Exception("No predictions.\nRaw: " + text);

            var list = new List<Texture2D>();
            foreach (var p in resp.predictions)
            {
                var bytes = Convert.FromBase64String(p.bytesBase64Encoded);
                var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                if (tex.LoadImage(bytes)) list.Add(tex);
            }
            if (list.Count == 0) throw new Exception("All images failed to decode.");
            tcs.SetResult(list.ToArray());
        }
        catch (Exception e) { 
            tcs.SetException(new Exception("Parse error: " + e.Message + "\nRaw: " + text)); 
        }
    }
}
