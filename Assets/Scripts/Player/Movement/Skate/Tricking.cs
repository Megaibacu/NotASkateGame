using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tricking : SkateController
{
    ScoreManager scoreM;
    // Start is called before the first frame update
    void Start()
    {
        scoreM = GetComponent<ScoreManager>();
    }

    // Update is called once per frame
    public override void Update()
    {
        
    }

    public void SkateTricks()
    {

        if (!touchingGround && !tricking)
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
                anim.SetTrigger("KickFlip");
                tricking = true;
            }

            if (Input.GetButton("GrabTricks"))
            {
                tricking = true;
            }
        }

        if (touchingGround && tricking)
        {
            fall = true;
            anim.SetTrigger("Fall");
        }

        if (fall)
        {
            // maxSpeed = 0;
        }
        else { maxSpeed = ogMaxSpeed; }
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
