using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastAlignerGood : MonoBehaviour
{
	public Transform objectToPlace;

	public LayerMask raycastNotHit;

	private bool isGrounded;

	public SkateController sC;

	private void Start()
	{
		StartCoroutine(RayHasBeenCast());
		//isGrounded = true;
		sC = GetComponent<SkateController>();
	}

	void Update()
	{
		Debug.Log(isGrounded);

		if (sC.grounded)
        {
			isGrounded = true;

		}

        else
        {
			isGrounded = false;
		}
	}

	//private void OnTriggerEnter(Collider other)
	//{
	//	Debug.Log("wsg");
	//	isGrounded = true;
	//}

	//private void OnTriggerExit(Collider other)
	//{
	//	isGrounded = false;
	//}

	private IEnumerator RayHasBeenCast()
	{

		while (true)
        {
			Ray ray = new Ray(transform.position, -transform.up);
			RaycastHit hitInfo;

			if (isGrounded == true && Physics.Raycast(ray, out hitInfo))
			{
				objectToPlace.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
				yield return new WaitForSeconds(0.1f);
			}

			else
			{
				yield return new WaitForSeconds(0.1f);
			}
		}
		

	}
}
