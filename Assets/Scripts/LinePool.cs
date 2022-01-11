using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TensorFlowLite;

public class LinePool
{
    private List<GameObject> backed = new List<GameObject>();
    private int usedCount = 0;

    public void ResetState()
    {
        foreach (var line in backed)
        {
            line.SetActive(false);
        }

        usedCount = 0;
    }

    public LineRenderer GetLine()
    {
        GameObject lineContainer = null;
        if (usedCount >= backed.Count)
        {
            lineContainer = new GameObject();
            LineRenderer lineRenderer = lineContainer.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.startColor = Color.green;
            lineRenderer.endColor = Color.green;
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;

            backed.Add(lineContainer);
        }
        else
        {
            lineContainer = backed[usedCount];
        }

        lineContainer.SetActive(true);
        usedCount++;
        return lineContainer.GetComponent<LineRenderer>();
    }
}