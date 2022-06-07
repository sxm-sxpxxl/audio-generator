using System.IO;
using SFB;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CanvasSampleSaveFileImage : MonoBehaviour, IPointerDownHandler
{
    public Text output;

    private byte[] _textureBytes;

    private void Awake()
    {
        // Create red texture
        var width = 100;
        var height = 100;
        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        for (var i = 0; i < width; i++)
        for (var j = 0; j < height; j++)
            tex.SetPixel(i, j, Color.red);
        tex.Apply();
        _textureBytes = tex.EncodeToPNG();
        Destroy(tex);
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);

    // Broser plugin should be called in OnPointerDown.
    public void OnPointerDown(PointerEventData eventData) {
        DownloadFile(gameObject.name, "OnFileDownload", "sample.png", _textureBytes, _textureBytes.Length);
    }

    // Called from browser
    public void OnFileDownload() {
        output.text = "File Successfully Downloaded";
    }
#else
    //
    // Standalone platforms & editor
    //
    public void OnPointerDown(PointerEventData eventData)
    {
    }

    // Listen OnClick event in standlone builds
    private void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        var path = StandaloneFileBrowser.SaveFilePanel("Title", "", "sample", "png");
        if (!string.IsNullOrEmpty(path)) File.WriteAllBytes(path, _textureBytes);
    }
#endif
}