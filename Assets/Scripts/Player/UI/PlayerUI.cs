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
    public Image speedometer;
    GameObject ui;
    Tricking trk;
    ScoreManager scoreM;
    SkateController sC;

    private void Start()
    {
        //TRUCOS DESACTIVADOS DEMOMENTO
        trk = FindObjectOfType<Tricking>();
        sC = FindObjectOfType<SkateController>();
        scoreM = FindObjectOfType<ScoreManager>();
        ui = GameObject.Find("UI");
        trickIndicator = ui.transform.GetChild(1).gameObject;
    }


    private void Update()
    {
        PointsUI();
        SpeedometerUpdate();
    }

    public void PointsUI()
    {
        if (trk.flipTricking || trk.grabTricking)
        {
            trickIndicator.SetActive(true);
        }
        else
        {
            trickIndicator.SetActive(false);
        }

        playerPoints.text = $"Score: {scoreM.curretnScore}";
        playerCombo.text = $"Combo: {scoreM.combo}";
    }

    public void SpeedometerUpdate()
    {
        speedometer.fillAmount = sC.currentSpeed / sC.maxSpeed;
    }
}
