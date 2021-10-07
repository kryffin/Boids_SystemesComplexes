using UnityEngine;

public class CandyManager : MonoBehaviour
{

    public bool isCandyDown = false;

    public GameObject candyPrefab;
    private GameObject candy;

    private Rigidbody2D candyRb;

    private void Start()
    {
        candy = Instantiate(candyPrefab, this.transform);
        candyRb = candy.GetComponent<Rigidbody2D>();
    }

    public void Move(Vector2 pos)
    {
        candyRb.position = pos;
    }

}
