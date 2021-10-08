using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{

    public enum STATE
    {
        SEEK, FEAR
    };
    public STATE state = STATE.SEEK;

    public enum TEAM
    {
        RED, BLUE
    };
    public TEAM team = TEAM.RED;

    public int NB_BOIDS = 25;

    public float VIEW_DIST; //is a boid too far from another ?
    public float CLOSE_DIST; //is a boid too close to another ?
    public float OBSTACLE_DIST; //is a boid too close to an obstacle ?
    public float ATTRACT_DIST; //is a boid too close to a ball ?

    private List<Boid> boids;

    public GameObject boidPrefab;

    public Color teamColor;

    public CandyManager cm;

    void Start()
    {
        boids = new List<Boid>();
        Vector2 spawnSize = GetComponent<BoxCollider2D>().size;
        Vector2 spawnPos = transform.position;
        spawnSize /= 2f;

        for (int i = 0; i < NB_BOIDS; i++)
        {
            Vector2 spawn = new Vector2(Random.Range(-spawnSize.x, spawnSize.x) + spawnPos.x, Random.Range(-spawnSize.y, spawnSize.y) + spawnPos.y);
            GameObject g = Instantiate(boidPrefab, this.transform);
            if (team == TEAM.RED)
                g.layer = LayerMask.NameToLayer("Red Team");
            else
                g.layer = LayerMask.NameToLayer("Blue Team");
            g.GetComponent<Rigidbody2D>().position = spawn;
            g.GetComponent<SpriteRenderer>().color = teamColor;
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
            //float color = (float)closeBoids.Count / (float)boids.Count;
            //b.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, color);


            // Casting rays in front of the boid to check for incoming obstacles
            List<Vector2> avoid = new List<Vector2>();

            LayerMask lm;
            if (team == TEAM.RED)
                lm = LayerMask.GetMask("Constraints", "Blue Team");
            else
                lm = LayerMask.GetMask("Constraints", "Red Team");

            foreach (RaycastHit2D h in
                Physics2D.RaycastAll(b.rb.position, b.velocity.normalized, OBSTACLE_DIST, lm))
                avoid.Add(h.point);

            foreach (RaycastHit2D h in
                Physics2D.RaycastAll(b.rb.position, Rotate(b.velocity, 25f).normalized, OBSTACLE_DIST, lm))
                avoid.Add(h.point);

            foreach (RaycastHit2D h in
                Physics2D.RaycastAll(b.rb.position, Rotate(b.velocity, -25f).normalized, OBSTACLE_DIST, lm))
                avoid.Add(h.point);

            foreach (RaycastHit2D h in
                Physics2D.RaycastAll(b.rb.position, Rotate(b.velocity, 50f).normalized, OBSTACLE_DIST, lm))
                avoid.Add(h.point);

            foreach (RaycastHit2D h in
                Physics2D.RaycastAll(b.rb.position, Rotate(b.velocity, -50f).normalized, OBSTACLE_DIST, lm))
                avoid.Add(h.point);

            b.Avoid(avoid, OBSTACLE_DIST);

            // Casting rays in front of the boid to check for incoming balls
            List<Vector2> attract = new List<Vector2>();

            lm = LayerMask.GetMask("Balls");

            foreach (RaycastHit2D h in
                Physics2D.RaycastAll(b.rb.position, b.velocity.normalized, ATTRACT_DIST, lm))
                attract.Add(h.point);

            foreach (RaycastHit2D h in
                Physics2D.RaycastAll(b.rb.position, Rotate(b.velocity, 25f).normalized, ATTRACT_DIST, lm))
                attract.Add(h.point);

            foreach (RaycastHit2D h in
                Physics2D.RaycastAll(b.rb.position, Rotate(b.velocity, -25f).normalized, ATTRACT_DIST, lm))
                attract.Add(h.point);

            foreach (RaycastHit2D h in
                Physics2D.RaycastAll(b.rb.position, Rotate(b.velocity, 50f).normalized, ATTRACT_DIST, lm))
                attract.Add(h.point);

            foreach (RaycastHit2D h in
                Physics2D.RaycastAll(b.rb.position, Rotate(b.velocity, -50f).normalized, ATTRACT_DIST, lm))
                attract.Add(h.point);

            if (state == STATE.SEEK) b.Attract(attract);
            if (state == STATE.FEAR) b.Fear(attract);

            //Attract towards candy if present
            if (cm.isCandyDown) b.Hunger(cm.GetCandyPosition());


            // Draws the rays
            Debug.DrawRay(b.rb.position, b.velocity.normalized * ATTRACT_DIST, new Color(0f, 1f, 1f, 0.8f));
            Debug.DrawRay(b.rb.position, Rotate(b.velocity, 25f).normalized * ATTRACT_DIST, new Color(0f, 1f, 1f, 0.6f));
            Debug.DrawRay(b.rb.position, Rotate(b.velocity, -25f).normalized * ATTRACT_DIST, new Color(0f, 1f, 1f, 0.6f));
            Debug.DrawRay(b.rb.position, Rotate(b.velocity, 50f).normalized * ATTRACT_DIST, new Color(0f, 1f, 1f, 0.4f));
            Debug.DrawRay(b.rb.position, Rotate(b.velocity, -50f).normalized * ATTRACT_DIST, new Color(0f, 1f, 1f, 0.4f));

        }
    }
}
