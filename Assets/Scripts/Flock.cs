using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public int NB_BOIDS = 25;

    public float VIEW_DIST; //is a boid too far from another ?
    public float CLOSE_DIST; //is a boid too close to another ?
    public float OBSTACLE_DIST; //is a boid too close to an obstacle ?

    private List<Boid> boids;

    public GameObject boidPrefab;
    
    void Start()
    {
        boids = new List<Boid>();

        for (int i = 0; i < NB_BOIDS; i++)
        {
            GameObject g = Instantiate(boidPrefab, this.transform);
            boids.Add(g.GetComponent<Boid>());
        }
    }

    private Vector2 Rotate(Vector2 v, float a)
    {
        float sin = Mathf.Sin(a * Mathf.Deg2Rad);
        float cos = Mathf.Cos(a * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
    }

    void Update()
    {
        foreach (Boid b in boids)
        {
            List<Boid> closeBoids = new List<Boid>();
            foreach (Boid other in boids)
            {
                if (other == b) continue; //myself

                float distance = Vector2.Distance(b.rb.position, other.rb.position);
                if (distance < VIEW_DIST)
                    closeBoids.Add(other);
            }

            b.MoveCloser(closeBoids);
            b.MoveWith(closeBoids);
            b.MoveAway(closeBoids, CLOSE_DIST);

            // Boid's alpha channel based on number of close neighbors
            float color = (float)closeBoids.Count / (float)boids.Count;
            b.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, color);

            // Casting a ray in front of the boid to check for incoming obstacles
            List<Vector2> avoid = new List<Vector2>();

            foreach (RaycastHit2D h in
                Physics2D.RaycastAll(b.rb.position, b.velocity.normalized, OBSTACLE_DIST, LayerMask.GetMask("Walls", "Obstacles")))
                avoid.Add(h.point);

            foreach (RaycastHit2D h in
                Physics2D.RaycastAll(b.rb.position, Rotate(b.velocity, 25f).normalized, OBSTACLE_DIST, LayerMask.GetMask("Walls", "Obstacles")))
                avoid.Add(h.point);

            foreach (RaycastHit2D h in
                Physics2D.RaycastAll(b.rb.position, Rotate(b.velocity, -25f).normalized, OBSTACLE_DIST, LayerMask.GetMask("Walls", "Obstacles")))
                avoid.Add(h.point);

            foreach (RaycastHit2D h in
                Physics2D.RaycastAll(b.rb.position, Rotate(b.velocity, 50f).normalized, OBSTACLE_DIST, LayerMask.GetMask("Walls", "Obstacles")))
                avoid.Add(h.point);

            foreach (RaycastHit2D h in
                Physics2D.RaycastAll(b.rb.position, Rotate(b.velocity, -50f).normalized, OBSTACLE_DIST, LayerMask.GetMask("Walls", "Obstacles")))
                avoid.Add(h.point);


            // Draws the rays
            Debug.DrawRay(b.rb.position, b.velocity.normalized, new Color(0f, 1f, 1f, 0.8f));
            Debug.DrawRay(b.rb.position, Rotate(b.velocity, 25f).normalized, new Color(0f, 1f, 1f, 0.6f));
            Debug.DrawRay(b.rb.position, Rotate(b.velocity, -25f).normalized, new Color(0f, 1f, 1f, 0.6f));
            Debug.DrawRay(b.rb.position, Rotate(b.velocity, 50f).normalized, new Color(0f, 1f, 1f, 0.4f));
            Debug.DrawRay(b.rb.position, Rotate(b.velocity, -50f).normalized, new Color(0f, 1f, 1f, 0.4f));

            b.Avoid(avoid, OBSTACLE_DIST);    
        }
    }
}
