using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public float MAX_VELOCITY = 4f;
    public float MIN_VELOCITY = 1f;

    private const float CLOSE_FACTOR = 100f;
    private const float WITH_FACTOR = 40f;
    private const float AWAY_FACTOR = 5f;
    private const float AVOID_FACTOR = 5f;
    private const float ATTRACT_FACTOR = 1f;

    public Vector2 velocity;

    public Rigidbody2D rb;

    private Transform spotLight;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spotLight = transform.Find("Spot Light").transform;

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

        Debug.DrawLine(rb.position, rb.position - avg, Color.white);

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

        Debug.DrawLine(rb.position, rb.position + (velocity - distance).normalized, Color.red);

        velocity += (distance / AWAY_FACTOR);
    }

    public void Avoid (List<Vector2> hitPositions, float minDistance)
    {
        if (hitPositions.Count < 1) return;

        Vector2 direction = Vector2.zero;
        foreach (Vector2 v in hitPositions)
        {
            Vector2 diff = (rb.position - v);
            if (diff.x >= 0f) diff.x = Mathf.Sqrt(minDistance) - diff.x;
            else if (diff.x < 0f) diff.x = -Mathf.Sqrt(minDistance) - diff.x;
            if (diff.y >= 0f) diff.y = Mathf.Sqrt(minDistance) - diff.y;
            else if (diff.y < 0f) diff.y = -Mathf.Sqrt(minDistance) - diff.y;
            direction += diff;
        }

        velocity += (direction / AVOID_FACTOR);
    }

    public void Attract(List<Vector2> hitPositions)
    {
        if (hitPositions.Count < 1) return;

        // Computes the average distance between the other balls
        Vector2 avg = Vector2.zero;

        foreach (Vector2 v in hitPositions)
            avg += (rb.position - v);

        avg /= hitPositions.Count;

        Debug.DrawLine(rb.position, rb.position - avg, new Color(1f, 0.5f, 0f, 1f));

        // Sets the velocity towards the others
        velocity -= (avg / ATTRACT_FACTOR);
    }

    private void FixedUpdate()
    {
        if (Mathf.Abs(velocity.x) > MAX_VELOCITY || Mathf.Abs(this.velocity.y) > MAX_VELOCITY)
        {
            float scaleFactor = MAX_VELOCITY / Mathf.Max(Mathf.Abs(velocity.x), Mathf.Abs(velocity.y));

            velocity *= scaleFactor;
        }

        if (Mathf.Abs(velocity.x) < MIN_VELOCITY && Mathf.Abs(this.velocity.y) < MIN_VELOCITY)
        {
            float scaleFactor = Mathf.Max(Mathf.Abs(velocity.x), Mathf.Abs(velocity.y)) / MIN_VELOCITY;

            velocity /= scaleFactor;
        };


        rb.MovePosition(rb.position + (velocity / 2f) * Time.deltaTime);

        spotLight.rotation = Quaternion.LookRotation(velocity.normalized);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        velocity = -velocity;
    }

    private void OnDrawGizmos()
    {
        //Far green circle
        Gizmos.color = new Color(0f, 1f, 0f, .2f);
        Gizmos.DrawWireSphere(rb.position, 1.5f);

        //Near red circle
        Gizmos.color = new Color(1f, 0f, 0f, .2f);
        Gizmos.DrawWireSphere(rb.position, .5f);

        //Boid direction line
        Gizmos.color = new Color(0f, 0f, 1f, .6f);
        Gizmos.DrawLine(rb.position, rb.position + velocity.normalized); //boid direction
    }

}
