using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollectibleSystem : MonoBehaviour
{
    [Header("==========Counters==========")]
    public int cassettes;
    public int graffitis;

    [Header("==========Max Collectibles==========")]
    private CassetteCollectible[] cassetteCollectibles;
    public int maxCassettes;

    [Header("==========UI==========")]
    public TextMeshProUGUI cassetteCollectibleCount;
    public float timeOnScreen;

    private void Start()
    {
        cassetteCollectibles = FindObjectsOfType<CassetteCollectible>();
        maxCassettes = cassetteCollectibles.Length;
    }

    public void NewCollectible()
    {
        cassetteCollectibleCount.text = $"Collectibles: {cassettes}/{maxCassettes}";
        StartCoroutine(CollectibleAppear());
    }

    IEnumerator CollectibleAppear()
    {
        cassetteCollectibleCount.gameObject.SetActive(true);
        yield return new WaitForSeconds(timeOnScreen);
        cassetteCollectibleCount.gameObject.SetActive(false);
    }
}
