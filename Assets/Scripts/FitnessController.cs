using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TensorFlowLite;
using UnityEngine.UI;

public class FitnessController : MonoBehaviour
{
    public GameObject square;
    public Text pointLabel;
    private float velocity = 0.5f;
    private float delayCheckBodyTime = 0;
    private int currentPoint = 0;

    void Start()
    {
        ResetJumpGuard();
        ResetPoint();
    }

    private void ResetJumpGuard()
    {
        Vector2 worldSize = Utils.WorldSize();
        Debug.Log("World size " + worldSize);
        square.transform.position = new Vector2(0, -3f * worldSize.y / 4f);
        square.transform.localScale = new Vector3(worldSize.x, worldSize.y, 1);
    }

    private void UpdateJumpGuard()
    {
        square.transform.position = new Vector2(0, velocity * Time.deltaTime + square.transform.position.y);
    }

    void Update()
    {
        UpdateJumpGuard();
        if (delayCheckBodyTime > 0)
        {
            delayCheckBodyTime -= Time.deltaTime;
        }
        else
        {
            MoveNet.Result[] results = PhoneCamera.shared.GetResult();
            if (results == null)
            {
                return;
            }

            MoveNet.Result leftAnkle = results[(int)MoveNet.Part.LEFT_ANKLE];
            MoveNet.Result rightAnkle = results[(int)MoveNet.Part.RIGHT_ANKLE];

            if (leftAnkle.confidence > 0.3 && rightAnkle.confidence > 0.3)
            {
                Vector3 leftAnkleWorldPoint = PhoneCamera.shared.PoseEstimatePointToWorldPoint(leftAnkle);
                Vector3 rightAnkleWorldPoint = PhoneCamera.shared.PoseEstimatePointToWorldPoint(rightAnkle);

                Debug.Log("leftAnkleWorldPoint: " + leftAnkleWorldPoint);
                Debug.Log("square.transform.position.y + (square.transform.localScale.y / 2): " + square.transform.position.y + (square.transform.localScale.y / 2));

                if (leftAnkleWorldPoint.y > square.transform.position.y + (square.transform.localScale.y / 2))
                {
                    Debug.Log("Point");
                    ResetJumpGuard();
                    delayCheckBodyTime = 0.5f;
                    IncreasePoint();
                }
            }
        }

        if (square.transform.position.y >= 0)
        {
            Debug.Log("Game over");
            ResetPoint();
            ResetJumpGuard();
        }

        // square.transform.position = new Vector2(0, 0);
        // square.transform.localScale = new Vector3(worldSize.x, worldSize.y, 1);

        // Jump


        // Lie
        // square.transform.position = new Vector2(0, worldSize.y / 8);
        // square.transform.localScale = new Vector3(worldSize.x, 3 * worldSize.y / 4, 1);
    }

    private void IncreasePoint()
    {
        currentPoint++;
        pointLabel.text = currentPoint.ToString();
    }

    private void ResetPoint()
    {
        currentPoint = 0;
        pointLabel.text = currentPoint.ToString();
    }
}
