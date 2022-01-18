using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TensorFlowLite;
using UnityEngine.UI;

enum FitnessGameState
{
    IDLE = 0,
    STARTED = 1,
    GAME_OVER = 2
}

public class FitnessController : MonoBehaviour
{
    public GameObject square;
    public Text pointLabel;
    public Button startButton;
    public Text gameOverText;

    private float velocity = 0.5f;
    private float delayCheckBodyTime = 0;
    private int currentPoint = 0;
    private FitnessGameState state = FitnessGameState.IDLE;


    void Start()
    {
        ResetJumpGuard();
        ResetPoint();
        square.SetActive(false);
        startButton.onClick.AddListener(StartGame);
        gameOverText.gameObject.SetActive(false);
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
        if (this.state != FitnessGameState.STARTED)
        {
            return;
        }

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

                if (leftAnkleWorldPoint.y > square.transform.position.y + (square.transform.localScale.y / 2) &&
                    rightAnkleWorldPoint.y > square.transform.position.y + (square.transform.localScale.y / 2))
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
            this.state = FitnessGameState.GAME_OVER;
            Debug.Log("Game over");
            ResetPoint();
            ResetJumpGuard();
            square.SetActive(false);
            gameOverText.gameObject.SetActive(true);
            startButton.gameObject.SetActive(true);
        }

        // square.transform.position = new Vector2(0, 0);
        // square.transform.localScale = new Vector3(worldSize.x, worldSize.y, 1);

        // Jump


        // Lie
        // square.transform.position = new Vector2(0, worldSize.y / 8);
        // square.transform.localScale = new Vector3(worldSize.x, 3 * worldSize.y / 4, 1);
    }

    public void StartGame()
    {
        this.state = FitnessGameState.STARTED;
        this.gameOverText.gameObject.SetActive(false);
        square.SetActive(true);
        startButton.gameObject.SetActive(false);
        ResetJumpGuard();
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
