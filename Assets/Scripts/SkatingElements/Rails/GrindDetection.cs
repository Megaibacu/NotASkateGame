using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrindDetection : MonoBehaviour
{
    public float finalForceMultiplier;
    bool grinding;
    //SplineWalker player;
    //BezierSpline spline;
    float distanceOfGrind;

    private void Start()
    {
        //spline = GetComponent<BezierSpline>();
    }

    private void Update()
    {
        if (grinding)
        {
            /*if(player.progress >= 1 || Input.GetKey(KeyCode.Space))
            {
                player.enabled = false;
                player.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * finalForceMultiplier, ForceMode.Impulse);
                player.gameObject.GetComponent<Rigidbody>().AddForce(transform.right * finalForceMultiplier, ForceMode.Impulse);
                player.gameObject.GetComponent<Rigidbody>().AddForce(transform.up * finalForceMultiplier, ForceMode.Impulse);
                grinding = false;
                player.progress = 0;
            }*/

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            /*if (other.GetComponent<StateChange>().state == States.skating)
            {
                grinding = true;
                player = other.gameObject.transform.GetComponent<SplineWalker>();
                other.GetComponent<SplineWalker>().spline = this.gameObject.GetComponent<BezierSpline>();
                other.GetComponent<SplineWalker>().progress = this.gameObject.GetComponent<BezierSpline>().GetPoint(other.transform.position);
                other.GetComponent<SplineWalker>().enabled = true;
            }*/
            
        }
    }
}
