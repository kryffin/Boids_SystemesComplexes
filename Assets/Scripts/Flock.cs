using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public const int NB_BOIDS = 500;

    public float MAX_DIST; //is a boid too far from another ?
    public float MIN_DIST; //is a boid too close to another ?

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

    void Update()
    {
        foreach (Boid b in boids)
        {
            List<Boid> closeBoids = new List<Boid>();
            foreach (Boid other in boids)
            {
                if (other == b) continue;
                float distance = Vector2.Distance(b.transform.position, other.transform.position);
                if (distance < MAX_DIST)
                    closeBoids.Add(other);
            }
            b.MoveCloser(closeBoids);
            b.MoveWith(closeBoids);
            b.MoveAway(closeBoids, 20f);

            float color = (float)closeBoids.Count / (float)boids.Count;
            b.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, color);
        }
    }
}
