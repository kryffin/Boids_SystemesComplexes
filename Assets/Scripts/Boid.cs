using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    private const float MAX_VELOCITY = 2f;

    private const float CLOSE_FACTOR = 100f;
    private const float WITH_FACTOR = 40f;
    private const float AWAY_FACTOR = 5f;

    public Vector2 dir;
    public float speed;

    public Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.position = new Vector2(Random.Range(-6f, 6f), Random.Range(-3f, 3f));
        this.dir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        this.speed = Random.Range(.1f, .6f);
    }

    public void MoveCloser(List<Boid> boids)
    {
        if (boids.Count < 1) return;

        // Computes the average distance between the other boids
        Vector2 avg = Vector2.zero;
        foreach (Boid b in boids)
        {
            if (b.rb.position == this.rb.position) continue; //myself
            avg += (this.rb.position - b.rb.position);
        }
        avg /= boids.Count;

        // Sets the velocity towards the others
        this.dir -= (avg / CLOSE_FACTOR).normalized;
    }

    public void MoveWith(List<Boid> boids)
    {
        if (boids.Count < 1) return;

        // Computes the average velocity between the other boids
        float avg = 0f;
        foreach (Boid b in boids)
            avg += b.speed;
        avg /= boids.Count;

        // Sets our velocity towards the others
        this.speed += (avg / WITH_FACTOR);
    }

    public void MoveAway(List<Boid> boids, float minDistance)
    {
        if (boids.Count < 1) return;

        Vector2 distance = Vector2.zero;
        int nbClose = 0;

        foreach (Boid b in boids)
        {
            if (Vector2.Distance(this.rb.position, b.rb.position) < minDistance)
            {
                nbClose++;
                Vector2 diff = (this.rb.position - b.rb.position);
                if (diff.x >= 0f) diff.x = Mathf.Sqrt(minDistance) - diff.x;
                else if (diff.x < 0f) diff.x = -Mathf.Sqrt(minDistance) - diff.x;
                if (diff.y >= 0f) diff.y = Mathf.Sqrt(minDistance) - diff.y;
                else if (diff.y < 0f) diff.y = -Mathf.Sqrt(minDistance) - diff.y;
                distance += diff;
            }
        }

        if (nbClose == 0) return;

        this.dir -= (distance / AWAY_FACTOR).normalized;
    }

    private void FixedUpdate()
    {
        if (Mathf.Abs(this.dir.x * speed) > MAX_VELOCITY || Mathf.Abs(this.dir.y * speed) > MAX_VELOCITY)
        {
            float scaleFactor = MAX_VELOCITY / Mathf.Max(Mathf.Abs(this.dir.x * speed), Mathf.Abs(this.dir.y * speed));

            speed *= scaleFactor;
        }
        rb.MovePosition(this.rb.position + (this.dir * speed) * Time.deltaTime);

        transform.rotation = Quaternion.identity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        this.dir = -this.dir;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, .2f);
        Gizmos.DrawWireSphere(rb.position, 2f);

        Gizmos.color = new Color(1f, 0f, 0f, .2f);
        Gizmos.DrawWireSphere(rb.position, 0.5f);

        Gizmos.color = new Color(0f, 0f, 1f, .4f);
        Gizmos.DrawLine(rb.position, rb.position + this.dir.normalized);
    }

}
