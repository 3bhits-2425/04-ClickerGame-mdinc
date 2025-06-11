using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public int score = 0;
    public TextMeshProUGUI scoreText;

    public Button saveButton;
    public Button loadButton;

    public GameObject ballPrefab;

    private List<GameObject> currentBalls = new List<GameObject>();
    private int maxBalls = 1;   // Anzahl der Bälle gleichzeitig
    private int scoreMultiplier = 1;  // Multiplikator für Punkte
    public float baseSwingSpeed = 2f;

    public Vector3 normalBallScale = Vector3.one;
    public Vector3 smallBallScale = Vector3.one * 0.8f;
    public Vector3 bigBallScale = Vector3.one * 1.9f;
    private Vector3 currentBallScale;

    void Start()
    {
        currentBallScale = normalBallScale;

        UpdateScoreUI();

        saveButton.onClick.AddListener(SaveScore);
        loadButton.onClick.AddListener(LoadScore);

        SpawnNewBalls();
    }

    public void AddScore(int amount)
    {
        score += amount * scoreMultiplier;
        UpdateScoreUI();
        CheckForUpgrades();
    }

    void CheckForUpgrades()
    {
        if (score > 0 && score % 5 == 0)
        {
            if (score >= 10)
            {
                currentBallScale = bigBallScale;
                maxBalls = 3;
                scoreMultiplier = 2;
                baseSwingSpeed = 5f;
            }
            else if (score >= 5)
            {
                currentBallScale = smallBallScale;
                maxBalls = 2;
                scoreMultiplier = 1;
                baseSwingSpeed = 4f;
            }
            else
            {
                currentBallScale = normalBallScale;
                maxBalls = 1;
                scoreMultiplier = 1;
                baseSwingSpeed = 2f;
            }
            TrimBallsList();
        }
    }

    void TrimBallsList()
    {
        while (currentBalls.Count > maxBalls)
        {
            GameObject ballToRemove = currentBalls[currentBalls.Count - 1];
            currentBalls.RemoveAt(currentBalls.Count - 1);
            Destroy(ballToRemove);
        }
    }

    public void SaveScore()
    {
        PlayerPrefs.SetInt("score", score);
        PlayerPrefs.Save();
        Debug.Log("Score gespeichert: " + score);
    }

    public void LoadScore()
    {
        score = PlayerPrefs.GetInt("score", 0);
        UpdateScoreUI();
        CheckForUpgrades();
        SpawnNewBalls();
        Debug.Log("Score geladen: " + score);
    }

    void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score;
    }

    public void NotifyBallDestroyed(GameObject ball)
    {
        if (currentBalls.Contains(ball))
        {
            currentBalls.Remove(ball);
        }
        SpawnNewBalls();
    }

    private void SpawnNewBalls()
    {
        while (currentBalls.Count < maxBalls)
        {
            Vector3 newPos = GenerateNonOverlappingPosition();
            GameObject newBall = Instantiate(ballPrefab, newPos, Quaternion.identity);
            Ball ballScript = newBall.GetComponent<Ball>();
            if (ballScript != null)
            {
                ballScript.gameManager = this;
                ballScript.swingSpeed = baseSwingSpeed;
            }
            newBall.transform.localScale = currentBallScale;
            currentBalls.Add(newBall);
        }
    }

    private Vector3 GenerateNonOverlappingPosition()
    {
        int attempts = 0;
        Vector3 pos;

        do
        {
            pos = new Vector3(Random.Range(-6f, 6f), Random.Range(-4f, 4f), 0f);
            attempts++;
            if (attempts > 50) break;

        } while (IsPositionOverlapping(pos));

        return pos;
    }

    private bool IsPositionOverlapping(Vector3 pos)
    {
        float ballRadius = 0.5f * currentBallScale.x; // Ursprungskugel Radius 0.5

        foreach (GameObject ball in currentBalls)
        {
            if (Vector3.Distance(ball.transform.position, pos) < ballRadius * 2f)
                return true;
        }
        return false;
    }
}
