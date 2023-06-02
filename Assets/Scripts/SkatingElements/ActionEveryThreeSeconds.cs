using UnityEngine;
using System.Collections;

public class ActionEveryThreeSeconds : MonoBehaviour
{
    private IEnumerator Start()
    {
        while (true)
        {
            // Perform your action here
            Debug.Log("Performing action!");

            yield return new WaitForSeconds(1f);
        }
    }
}