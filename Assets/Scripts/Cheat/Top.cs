using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Top : MonoBehaviour
{
    public GameObject cheaterMain;
    private CheatMain cheater;
    void Start()
    {
        cheater = cheaterMain.GetComponent<CheatMain>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        cheater.topTriggered = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        cheater.topTriggered = false;
    }

}
