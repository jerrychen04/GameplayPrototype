using System.Collections;
using UnityEngine;
using TMPro; // Make sure to use TextMeshPro for the UI

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public float accelerationForce = 10.0f;
    public Transform indicator; // The cube indicator as a child of the circle GameObject
    public float radius = 1.0f; // The radius of the circle to place the indicator on the edge
    public TextMeshProUGUI cooldownText; // Using TextMeshProUGUI for the cooldown display
    public BoxCollider2D playerCollider; // The player's BoxCollider2D component

    private Rigidbody2D rb;
    private Vector2 direction;
    private bool isDashAvailable = false; // Initially, dash is not available
    public float dashAvailabilityInterval = 2.0f; // Dash becomes available every 2 seconds
    public float flashDuration = 0.5f; // Duration in seconds to flash the "Dash Ready!" text
    public float momentumStopTime = 2.0f; // Time in seconds after which momentum stops

    public AudioSource PlayerAudioSource; // Assign the AudioSource component in the inspector
    public AudioClip dashSound; // Assign the dash sound in the inspector


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!playerCollider) playerCollider = GetComponent<BoxCollider2D>(); // Ensure the collider is set
        rb.drag = 0.5f;
        StartCoroutine(DashAvailabilityRoutine());
    }

    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(moveHorizontal, moveVertical).normalized;

        CalculateMouseDirection();
        PositionIndicator();

        if (movement.magnitude > 0)
        {
            rb.AddForce(movement * speed, ForceMode2D.Force);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isDashAvailable)
        {
            Debug.Log($"Applying impulse in direction: {direction}, Force: {accelerationForce}");
            playerCollider.isTrigger = true; 
            rb.AddForce(direction * accelerationForce, ForceMode2D.Impulse);
            StartCoroutine(StopMomentumAfterDelay());
            isDashAvailable = false;
            cooldownText.text = ""; // Optionally clear the text immediately after dashing
        }

        // press R to restart level
        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Root");
        }
    }

    void CalculateMouseDirection()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition.z = 0;

        direction = (mouseWorldPosition - transform.position).normalized;
    }

    void PositionIndicator()
    {
        indicator.position = transform.position + (Vector3)(direction * radius);
    }

    IEnumerator StopMomentumAfterDelay()
    {
        yield return new WaitForSeconds(momentumStopTime);
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        playerCollider.isTrigger = false; 
    }

    IEnumerator DashAvailabilityRoutine()
    {
        while (true)
        {
            for (float i = dashAvailabilityInterval; i > 0; i -= Time.deltaTime)
            {
                cooldownText.text = $"{i.ToString("F1")}";
                yield return null;
            }

            isDashAvailable = true;
            cooldownText.text = "Dash!";
            yield return new WaitForSeconds(flashDuration); // Flash duration for "Dash Ready!"
            PlayerAudioSource.PlayOneShot(dashSound); // Play the dash sound
            cooldownText.text = ""; // Hide the text after the flash duration
            yield return new WaitForSeconds(flashDuration); // Wait for the remainder of the interval minus flash duration
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy")) // Check if the collided object has the tag "Enemy"
        {
            other.gameObject.SetActive(false);
            Destroy(other.gameObject); // Destroy the enemy game object
        }
    }
}
