using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TensorFlowLite;
using Cysharp.Threading.Tasks;

public class PhoneCamera : MonoBehaviour
{
    [SerializeField, FilePopup("*.tflite")]
    string fileName = default;
    private WebCamTexture cameraTexture;
    public RawImage cameraPreviewView;
    public AspectRatioFitter ratioFilter;
    private MoveNet moveNet;
    public static PhoneCamera shared;

    private readonly Vector3[] rtCorners = new Vector3[4];
    private MoveNet.Result[] results;
    private float threshold = 0.3f;
    private LinePool linePool = new LinePool();
    private Vector2 cameraTextureSize;

    void Awake()
    {
        shared = this;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void Start()
    {
        moveNet = new MoveNet(fileName);

        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            Debug.Log("No camera");
            return;
        }

        foreach (var device in devices)
        {
            if (device.isFrontFacing)
            {
                cameraTexture = new WebCamTexture(device.name, Screen.width, Screen.height);
            }
        }

        Debug.Log("Screen width: " + Screen.width + ", height: " + Screen.height);

        if (cameraTexture == null)
        {
            return;
        }

        cameraTexture.Play();
    }

    // Update is called once per frame
    void Update()
    {
        float ratio = (float)cameraTexture.width / (float)cameraTexture.height;
        ratioFilter.aspectRatio = ratio;

        Debug.Log("Camera width: " + cameraTexture.width + ", height: " + cameraTexture.height);
        if (this.cameraTexture.didUpdateThisFrame)
        {
            RenderTexture texture = Utils.NormalizeWebcam(cameraTexture, cameraTexture.width, cameraTexture.height, true);
            moveNet.Invoke(texture);
            results = moveNet.GetResults();
            cameraPreviewView.texture = texture;
        }

        DrawResult(results);
    }

    private void DrawResult(MoveNet.Result[] results)
    {
        linePool.ResetState();
        if (results == null || results.Length == 0)
        {
            return;
        }

        var connections = MoveNet.Connections;
        int connectionsCount = connections.GetLength(0);

        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        for (int index = 0; index < connectionsCount; index++)
        {
            var a = results[(int)connections[index, 0]];
            var b = results[(int)connections[index, 1]];
            if (a.confidence >= threshold && b.confidence >= threshold)
            {
                Vector3 point1 = PoseEstimatePointToWorldPoint(a);
                Vector3 point2 = PoseEstimatePointToWorldPoint(b);

                LineRenderer line = linePool.GetLine();
                line.SetPosition(0, point1);
                line.SetPosition(1, point2);
            }
        }
    }

    public MoveNet.Result[] GetResult()
    {
        return this.results;
    }

    #region Helper
    public Vector3 PoseEstimatePointToWorldPoint(MoveNet.Result point)
    {
        var rect = cameraPreviewView.GetComponent<RectTransform>();
        rect.GetWorldCorners(rtCorners);
        Vector3 min = rtCorners[0];
        Vector3 max = rtCorners[2];

        Vector3 result = MathTF.Lerp(min, max, new Vector3(point.x, 1f - point.y, 0));

        result.x /= ratioFilter.aspectRatio;
        result.z = 0;
        return result;
    }
    #endregion
}
