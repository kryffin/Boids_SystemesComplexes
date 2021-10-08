using UnityEngine;

public class CandyManager : MonoBehaviour
{

    public bool isCandyDown = false;

    public GameObject candyPrefab;
    private GameObject candy;
    private Rigidbody2D candyRb;
    private SpriteRenderer candySr;

    private void Start()
    {
        candy = Instantiate(candyPrefab, this.transform);
        candyRb = candy.GetComponent<Rigidbody2D>();
        candySr = candy.GetComponent<SpriteRenderer>();
    }

    public void Move(Vector2 pos)
    {
        candyRb.position = pos;
    }

    public Vector2 GetCandyPosition()
    {
        return candyRb.position;
    }

    public void CandyDown()
    {
        candySr.enabled = true;
        isCandyDown = true;
    }
    public void CandyUp()
    {
        candySr.enabled = false;
        isCandyDown = false;
    }


}
