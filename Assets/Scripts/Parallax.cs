using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public GameObject binding;
    float lastFrame = 0;
    float thisFrame;
    private bool started = false;
    float parallaxSpeed;   //making this variable public breaks the parallax for some reason ???
    float parallaxSpeedBase = 35f;
    float halfScreenLength = 21;
    


    void Start()
    {
        //Debug.Log(transform.localScale.x);
        parallaxSpeed = parallaxSpeedBase*(transform.localScale.x)/8f;
        //parallaxSpeed = parallaxSpeedBase;
        //Debug.Log(parallaxSpeed);

    }


    void Update()
    {
        thisFrame = binding.transform.position.x;
        if (!started) {
            lastFrame = thisFrame;
            started = true;
        }
        transform.Translate(new Vector3(-(thisFrame-lastFrame) * Time.deltaTime * parallaxSpeed, 0f, 0f), Space.World);
        lastFrame = binding.transform.position.x;


        if (transform.position.x > binding.transform.position.x+halfScreenLength) {
            transform.Translate(new Vector3(-2*halfScreenLength, 0, 0));
            transform.position = new Vector3(transform.position.x, Random.Range(-5f, 5f), 0);
            float gotRange = Random.Range(0.5f, 13f);
            transform.localScale = new Vector3(gotRange,gotRange,gotRange);
            parallaxSpeed = parallaxSpeedBase*(transform.localScale.x)/8f;
            //Debug.Log(parallaxSpeed);
            //Debug.Log("planet teleported left");
        }
        else if (transform.position.x < binding.transform.position.x-halfScreenLength) {
            transform.Translate(new Vector3(2*halfScreenLength, 0, 0));
            transform.position = new Vector3(transform.position.x, Random.Range(-5f, 5f), 0);
            float gotRange = Random.Range(0.5f, 13f);
            transform.localScale = new Vector3(gotRange,gotRange,gotRange);
            parallaxSpeed = parallaxSpeedBase*(transform.localScale.x)/8f;
            //Debug.Log(parallaxSpeed);
            //Debug.Log("planet teleported right");
        }

    }
}
