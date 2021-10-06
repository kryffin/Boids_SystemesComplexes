using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    public ScoreManager sm;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(worldPosition);
            sm.CreateBall(worldPosition);
        }
    }
}
