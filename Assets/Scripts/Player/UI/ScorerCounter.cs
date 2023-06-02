using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScorerCounter : MonoBehaviour
{

    public ScoreManager scoreM;

    public float currentScore;

    public Image c;
    public Image b;
    public Image a;
    public Image s;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentScore = scoreM.combo;

        if (currentScore == 0)
        {
            c.gameObject.SetActive(false);
            b.gameObject.SetActive(false);
            a.gameObject.SetActive(false);
            s.gameObject.SetActive(false);
        }

        if (currentScore >= 1  && currentScore <= 3)
        {
            c.gameObject.SetActive(true);
            b.gameObject.SetActive(false);
            a.gameObject.SetActive(false);
            s.gameObject.SetActive(false);
        }

        if (currentScore >= 3 && currentScore <= 4)
        {
            c.gameObject.SetActive(false);
            b.gameObject.SetActive(true);
            a.gameObject.SetActive(false);
            s.gameObject.SetActive(false);
        }

        if (currentScore >= 5 && currentScore <= 7)
        {
            c.gameObject.SetActive(false);
            b.gameObject.SetActive(false);
            a.gameObject.SetActive(true);
            s.gameObject.SetActive(false);
        }

        if (currentScore >= 8)
        {
            c.gameObject.SetActive(false);
            b.gameObject.SetActive(false);
            a.gameObject.SetActive(false);
            s.gameObject.SetActive(true);
        }

    }
}
