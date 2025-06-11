using UnityEngine;

public class Ball : MonoBehaviour
{
    public GameManager gameManager;
    public float swingSpeed = 2f;
    public float swingAmount = 1.5f;

    private Vector3 startPos;
    private float timeOffset;
    private bool isClicked = false;

    void Start()
    {
        startPos = transform.position;
        timeOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        float x = Mathf.Sin(Time.time * swingSpeed + timeOffset) * swingAmount;
        transform.position = new Vector3(startPos.x + x, startPos.y, startPos.z);
    }

    private void OnMouseDown()
    {
        if (isClicked) return;
        isClicked = true;

        gameManager.AddScore(1);
        gameManager.NotifyBallDestroyed(gameObject);
        Destroy(gameObject);
    }
}
