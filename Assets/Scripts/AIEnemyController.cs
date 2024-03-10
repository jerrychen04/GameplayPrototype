using System.Collections;
using UnityEngine;

public class AIEnemyController : MonoBehaviour
{
    public Transform player; // Assign the player's transform in the inspector
    public float speed = 5.0f;
    public float accelerationForce = 10.0f;
    public float dashAvailabilityInterval = 2.0f; // Dash becomes available every 2 seconds
    public float flashDuration = 0.5f; // Duration in seconds to flash the "Dash Ready!"
    public float momentumStopTime = 2.0f; // Time in seconds after which momentum stops

    private Rigidbody2D rb;
    private Vector2 direction;
    private bool isDashAvailable = false; // Initially, dash is not available

    // Optional: AudioSource for dash sound, similar to the player controller
    public AudioSource enemyAudioSource;
    public AudioClip dashSound;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(DashAvailabilityRoutine());
    }

    void Update()
    {
        if (player != null)
        {
            CalculateDirectionTowardsPlayer();
            MoveTowardsPlayer();
        }

        if (isDashAvailable)
        {
            DashTowardsPlayer();
            isDashAvailable = false; // Reset dash availability after dashing
        }
    }

    void CalculateDirectionTowardsPlayer()
    {
        Vector2 playerPosition = player.position;
        Vector2 currentPosition = transform.position;
        direction = (playerPosition - currentPosition).normalized;
    }

    void MoveTowardsPlayer()
    {
        rb.velocity = direction * speed;
    }

    void DashTowardsPlayer()
    {
        rb.AddForce(direction * accelerationForce, ForceMode2D.Impulse);
        StartCoroutine(StopMomentumAfterDelay());
    }

    IEnumerator StopMomentumAfterDelay()
    {
        yield return new WaitForSeconds(momentumStopTime);
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    IEnumerator DashAvailabilityRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(dashAvailabilityInterval - flashDuration);
            isDashAvailable = true;
            yield return new WaitForSeconds(flashDuration); // Simulate the flash duration
        }
    }
}
