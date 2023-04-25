using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tricking : MonoBehaviour
{
    public SkateController sC;
    ScoreManager scoreM;
    private Animator anim;
    private Rigidbody rB;

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
        rB = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (sC.grounded)
     
		{
            sC.momentum = rB.velocity;
            EndTrick();
        }
    }

    public void SkateTricks()
    {

        if (!sC.grounded && !tricking)
        {
            if (Input.GetButtonDown("FlipTricks"))
            {
                Debug.Log("Kickflip");
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
                if (scoreM.combo > 0) { scoreM.curretnScore += (tricks[0].scoreAwarded * scoreM.combo); }
                else { scoreM.curretnScore += tricks[0].scoreAwarded; }
                anim.SetTrigger("KickFlip");
                tricking = true;
            }

            if (Input.GetButton("GrabTricks"))
            {
                tricking = true;
            }
        }

        if (sC.grounded && tricking)
        {
            fall = true;
            anim.SetTrigger("Fall");
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
