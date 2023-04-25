using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("==========Trick UI==========")]
    public GameObject trickIndicator;
    public TextMeshProUGUI playerPoints;
    public TextMeshProUGUI playerCombo;
    GameObject ui;
    Tricking trk;
    ScoreManager scoreM;

    private void Start()
    {
        //TRUCOS DESACTIVADOS DEMOMENTO
        //trk = FindObjectOfType<Tricking>();
        scoreM = FindObjectOfType<ScoreManager>();
        ui = GameObject.Find("UI");
        trickIndicator = ui.transform.GetChild(1).gameObject;
    }


    private void Update()
    {
        //PointsUI();
    }

    public void PointsUI()
    {
        if (trk.tricking)
        {
            trickIndicator.SetActive(true);
        }
        else
        {
            trickIndicator.SetActive(false);
        }

        playerPoints.text = $"Score: {scoreM.score}";
        playerCombo.text = $"Combo: {scoreM.combo}";
    }
}
