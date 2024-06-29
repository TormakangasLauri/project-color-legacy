using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scores : MonoBehaviour
{
    [Range(.1f,5f)]
    public float updateSpeed = 1;

    public Text player1;
    public Text player2;
    public Text player3;
    public Text player4;

	void Start ()
    {
	}

    void ClearScores()
    {
        if (player1 != null) player1.text = "0%";
        if (player2 != null) player2.text = "0%";
        if (player3 != null) player3.text = "0%";
        if (player4 != null) player4.text = "0%";
    }

    private float nextUpdateTime = 0;
	void Update ()
    {
        if (Time.realtimeSinceStartup < nextUpdateTime) return;
        nextUpdateTime = Time.realtimeSinceStartup + updateSpeed;

        PaintTarget.TallyScore();

        float n = PaintTarget.scores.x + PaintTarget.scores.y + PaintTarget.scores.z + PaintTarget.scores.w;
        if (n == 0)
        {
            ClearScores();
            return;
        }

        if (player1 != null) player1.text = Mathf.RoundToInt(((PaintTarget.scores.x / n) * 100.0f)).ToString() + "%";
        if (player2 != null) player2.text = Mathf.RoundToInt(((PaintTarget.scores.y / n) * 100.0f)).ToString() + "%";
        if (player3 != null) player3.text = Mathf.RoundToInt(((PaintTarget.scores.z / n) * 100.0f)).ToString() + "%";
        if (player4 != null) player4.text = Mathf.RoundToInt(((PaintTarget.scores.w / n) * 100.0f)).ToString() + "%";

    }
}
