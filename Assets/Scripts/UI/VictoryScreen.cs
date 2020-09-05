using System.Collections;
using System.Collections.Generic;
using Mechanics.Collectibles;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CollectiblesManager collectiblesManager = GameObject.Find("CollectiblesManager").GetComponent<CollectiblesManager>();
        transform.Find("Gems").GetComponent<Text>().text = collectiblesManager.Gems.ToString();
        transform.Find("Letters").GetComponent<Text>().text = collectiblesManager.TeammateLetters.ToString();
        transform.Find("EldaanLetters").GetComponent<Text>().text = collectiblesManager.EldaanLetters.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
