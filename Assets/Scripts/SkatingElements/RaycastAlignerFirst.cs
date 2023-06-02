using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastAlignerFirst : MonoBehaviour
{
	public Transform objectToPlace;

	public LayerMask raycastNotHit;

	private bool isGrounded;

    private void Start()
    {
		StartCoroutine(RayHasBeenCast());
		isGrounded = true;
	}

    void Update()
	{
		
	}

    private void OnTriggerEnter(Collider other)
    {
		isGrounded = true;
    }

 //   private void OnTriggerExit(Collider other)
 //   {
	//	isGrounded = false;
	//}

    IEnumerator RayHasBeenCast()
	{
		//if (isGrounded == true)
		{
			//Ray ray = new Ray(transform.position, -transform.up);
			//RaycastHit hitInfo;

			//if (Physics.Raycast(ray, out hitInfo))
			//{
			//	objectToPlace.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
			//}

			Debug.Log("Passing");
			yield return new WaitForSeconds(0.1f);
		}
		
	}
}
