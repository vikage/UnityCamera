using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TensorFlowLite;
using Cysharp.Threading.Tasks;

public class Utils
{
    static TextureResizer resizer = new TextureResizer();

    private static bool IsPortrait(WebCamTexture texture)
    {
        return texture.videoRotationAngle == 90 || texture.videoRotationAngle == 270;
    }

    public static RenderTexture NormalizeWebcam(WebCamTexture texture, int width, int height, bool isFrontFacing)
    {
        int cameraWidth = texture.width;
        int cameraHeight = texture.height;
        bool isPortrait = IsPortrait(texture);
        if (isPortrait)
        {
            (cameraWidth, cameraHeight) = (cameraHeight, cameraWidth); // swap
        }

        float cameraAspect = (float)cameraWidth / (float)cameraHeight;
        float targetAspect = (float)width / (float)height;

        int w, h;
        if (cameraAspect > targetAspect)
        {
            w = Mathf.FloorToInt(cameraHeight * targetAspect);
            h = cameraHeight;
        }
        else
        {
            w = cameraWidth;
            h = Mathf.FloorToInt(cameraWidth / targetAspect);
        }

        Matrix4x4 mtx;
        Vector4 uvRect;
        int rotation = texture.videoRotationAngle;

        // Seems to be bug in the android. might be fixed in the future.
        if (Application.platform == RuntimePlatform.Android)
        {
            rotation = -rotation;
        }

        if (isPortrait)
        {
            mtx = TextureResizer.GetVertTransform(rotation, texture.videoVerticallyMirrored, isFrontFacing);
            uvRect = TextureResizer.GetTextureST(targetAspect, cameraAspect, AspectMode.Fit);
        }
        else
        {
            mtx = TextureResizer.GetVertTransform(rotation, isFrontFacing, texture.videoVerticallyMirrored);
            uvRect = TextureResizer.GetTextureST(cameraAspect, targetAspect, AspectMode.Fit);
        }

        // Debug.Log($"camera: rotation:{texture.videoRotationAngle} flip:{texture.videoVerticallyMirrored}");
        return resizer.Resize(texture, w, h, false, mtx, uvRect);
    }
}