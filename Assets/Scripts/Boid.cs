using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    private const float MAX_VELOCITY = 2f;

    private const float CLOSE_FACTOR = 100f;
    private const float WITH_FACTOR = 40f;
    private const float AWAY_FACTOR = 50f;

    public Vector2 velocity;

    public Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.position = new Vector2(Random.Range(-6f, 6f), Random.Range(-3f, 3f));
        velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
    }

    public void MoveCloser(List<Boid> boids)
    {
        if (boids.Count < 1) return;

        // Computes the average distance between the other boids
        Vector2 avg = Vector2.zero;

        foreach (Boid b in boids)
        {
            if (rb.position == b.rb.position) continue; //myself

            avg += (rb.position - b.rb.position);
        }

        avg /= boids.Count;

        Debug.DrawLine(rb.position, rb.position - avg); //direction towards nearest flock

        // Sets the velocity towards the others
        velocity -= (avg / CLOSE_FACTOR);
    }

    public void MoveWith(List<Boid> boids)
    {
        if (boids.Count < 1) return;

        // Computes the average velocity between the other boids
        Vector2 avg = Vector2.zero;

        foreach (Boid b in boids)
            avg += b.velocity;

        avg /= boids.Count;

        // Sets our velocity towards the others
        velocity += (avg / WITH_FACTOR);
    }

    public void MoveAway(List<Boid> boids, float minDistance)
    {
        if (boids.Count < 1) return;

        Vector2 distance = Vector2.zero;
        int nbClose = 0;

        foreach (Boid b in boids)
        {
            if (Vector2.Distance(rb.position, b.rb.position) < minDistance)
            {
                nbClose++;
                Vector2 diff = (rb.position - b.rb.position);
                if (diff.x >= 0f) diff.x = Mathf.Sqrt(minDistance) - diff.x;
                else if (diff.x < 0f) diff.x = -Mathf.Sqrt(minDistance) - diff.x;
                if (diff.y >= 0f) diff.y = Mathf.Sqrt(minDistance) - diff.y;
                else if (diff.y < 0f) diff.y = -Mathf.Sqrt(minDistance) - diff.y;
                distance += diff;
            }
        }

        if (nbClose == 0) return;

        velocity -= (distance / AWAY_FACTOR);
    }

    private void FixedUpdate()
    {
        if (Mathf.Abs(velocity.x) > MAX_VELOCITY || Mathf.Abs(this.velocity.y) > MAX_VELOCITY)
        {
            float scaleFactor = MAX_VELOCITY / Mathf.Max(Mathf.Abs(this.velocity.x), Mathf.Abs(this.velocity.y));

            velocity *= scaleFactor;
        }

        rb.MovePosition(rb.position + (velocity / 10f) * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        velocity = -velocity;
    }

    private void OnDrawGizmos()
    {
        // MoveWith & MoveCloser
        Gizmos.color = new Color(0f, 1f, 0f, .2f);
        Gizmos.DrawWireSphere(rb.position, 3f);

        // MoveAway
        Gizmos.color = new Color(1f, 0f, 0f, .2f);
        Gizmos.DrawWireSphere(rb.position, 1f);

        Gizmos.color = new Color(0f, 0f, 1f, .4f);
        Gizmos.DrawLine(rb.position, rb.position + velocity.normalized);
    }

}
