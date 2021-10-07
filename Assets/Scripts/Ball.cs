using UnityEngine;

public class Ball : MonoBehaviour
{

    public ScoreManager sm;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Red Team"))
        {
            sm.RedScore();
        }
        else
        {
            sm.BlueScore();
        }

        //spawn new ball ?
        this.transform.position = new Vector3(Random.Range(-6f, 6f), Random.Range(-3f, 3f), -1f);
    }

}
