using UnityEngine;

public class InputManager : MonoBehaviour
{

    public ScoreManager sm;
    public CandyManager cm;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            sm.CreateBall(worldPosition);
        }
        else if (Input.GetMouseButton(1))
        {
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cm.Move(worldPosition);
            cm.isCandyDown = true;
        }
        else
        {
            cm.isCandyDown = false;
        }
    }
}
