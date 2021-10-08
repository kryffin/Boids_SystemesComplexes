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
    private const float ATTRACT_FACTOR = 4f;
    private const float HUNGRY_FACTOR = 6f;

    public Vector2 velocity;
    [SerializeField] private Vector2 desiredVelocity;

    private float turnSpeed = 2f;

    public Rigidbody2D rb;

    private Transform spotLight;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spotLight = transform.Find("Spot Light").transform;

        velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        desiredVelocity = velocity;
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
        //velocity -= (avg / CLOSE_FACTOR);
        desiredVelocity -= (avg / CLOSE_FACTOR);
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
        //velocity += (avg / WITH_FACTOR);
        desiredVelocity += (avg / WITH_FACTOR);
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

        Debug.DrawLine(rb.position, rb.position + distance.normalized, Color.red);

        //velocity += (distance / AWAY_FACTOR);
        desiredVelocity += (distance / AWAY_FACTOR);
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

        Debug.DrawLine(rb.position, rb.position + direction.normalized, Color.red);

        //velocity += (direction / AVOID_FACTOR);
        desiredVelocity += (direction / AVOID_FACTOR);
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
        //velocity -= (avg / ATTRACT_FACTOR);
        desiredVelocity -= (avg / ATTRACT_FACTOR);
    }

    public void Hunger(Vector2 candyPosition)
    {
        Debug.DrawLine(rb.position, candyPosition, new Color(1f, 0.5f, 0.9f, 1f));

        // Sets the velocity towards the candy
        //velocity -= ((rb.position - candyPosition) / HUNGRY_FACTOR);
        desiredVelocity -= ((rb.position - candyPosition) / HUNGRY_FACTOR);
    }

    private void FixedUpdate()
    {

        Debug.DrawRay(rb.position, velocity.normalized * 1.5f, Color.green);

        //float angle = Vector2.Angle(velocity.normalized, desiredVelocity.normalized);
        //Debug.Log(angle);
        //Quaternion rot = Quaternion.Euler(0f, 0f, angle/* * turnSpeed * Time.deltaTime*/);
        //Quaternion rot2 = Quaternion.FromToRotation(velocity.normalized, desiredVelocity.normalized / 2f);
        //velocity = rot2 * velocity;
        velocity = Vector3.RotateTowards(velocity.normalized, desiredVelocity.normalized, turnSpeed * Time.fixedDeltaTime, 0f);

        // Applies the desired velocity to the velocity
        //velocity.x -= (velocity.x - desiredVelocity.x) * Time.deltaTime;
        //velocity.y -= (velocity.y - desiredVelocity.y) * Time.deltaTime;

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

        /*if (Mathf.Abs(desiredVelocity.x) > MAX_VELOCITY || Mathf.Abs(this.desiredVelocity.y) > MAX_VELOCITY)
        {
            float scaleFactor = MAX_VELOCITY / Mathf.Max(Mathf.Abs(desiredVelocity.x), Mathf.Abs(desiredVelocity.y));

            desiredVelocity *= scaleFactor;
        }

        if (Mathf.Abs(desiredVelocity.x) < MIN_VELOCITY && Mathf.Abs(this.desiredVelocity.y) < MIN_VELOCITY)
        {
            float scaleFactor = Mathf.Max(Mathf.Abs(desiredVelocity.x), Mathf.Abs(desiredVelocity.y)) / MIN_VELOCITY;

            desiredVelocity /= scaleFactor;
        };*/

        rb.MovePosition(rb.position + (velocity / 2f) * Time.fixedDeltaTime);

        spotLight.rotation = Quaternion.LookRotation(velocity.normalized);
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
