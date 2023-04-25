using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tricking : MonoBehaviour
{
    public SkateController sC;
    ScoreManager scoreM;

    [Header("===============Skate Tricks===============")]
    public SkateTricks[] tricks; //All the tricks that can be perfornmed by the player. In the future it would be cool to make this customizable
    public float comboTimer; // The time that the player has to do another trick to continue the combo
    [HideInInspector] public bool tricking; //Checks if the player is mid trick to know if they have to fall
    [HideInInspector] public bool fall; //If the player touches the ground and is doing a tricks then they fall


    // Start is called before the first frame update
    void Start()
    {
        sC = GetComponent<SkateController>();
        scoreM = GetComponent<ScoreManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (sC.touchingGround)
        {
            sC.momentum = sC.rb.velocity;
            EndTrick();
        }
    }

    public void SkateTricks()
    {

        if (!sC.touchingGround && !tricking)
        {
            if (Input.GetButtonDown("FlipTricks"))
            {
                if (Input.GetAxisRaw("Vertical") < 0)
                {
                    Debug.Log("Heelflip");
                }
                else if (Input.GetAxisRaw("Vertical") > 0)
                {
                    Debug.Log("Pop Shuvit");
                }
                else if (Input.GetAxisRaw("Horizontal") < 0)
                {
                    Debug.Log("Kickflip");
                }
                else if (Input.GetAxisRaw("Horizontal") > 0)
                {
                    Debug.Log("Heelflip");
                }

                StopCoroutine(StartComboCounter());
                scoreM.combo++;
                if (scoreM.combo > 0) { scoreM.score += (tricks[0].scoreAwarded * scoreM.combo); }
                else { scoreM.score += tricks[0].scoreAwarded; }
                sC.anim.SetTrigger("KickFlip");
                tricking = true;
            }

            if (Input.GetButton("GrabTricks"))
            {
                tricking = true;
            }
        }

        if (sC.touchingGround && tricking)
        {
            fall = true;
            sC.anim.SetTrigger("Fall");
        }

        if (fall)
        {
            // maxSpeed = 0;
        }
        else { sC.maxSpeed = sC.ogMaxSpeed; }
    }
    public void EndTrick()
    {
        tricking = false;
    }

    public IEnumerator StartComboCounter()
    {
        yield return new WaitForSeconds(comboTimer);
        scoreM.combo = 0;
    }
}
