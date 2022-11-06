using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public GameObject player;
    float lastFrame = 0;
    float thisFrame;
    private bool started = false;
    public float parallaxSpeed = 10f;   //making this variable public breaks the parallax for some reason ???
    float halfScreenLength = 25;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        thisFrame = player.transform.position.x;
        if (!started) {
            lastFrame = thisFrame;
            started = true;
        }
        transform.Translate(new Vector3(-(thisFrame-lastFrame) * Time.deltaTime * parallaxSpeed, 0f, 0f), Space.World);
        lastFrame = player.transform.position.x;
        //Debug.Log(started);

        //Debug.Log(transform.position.x.ToString()+" < "+ (player.transform.position.x-halfScreenLength).ToString());
        Debug.Log(transform.position.x.ToString()+" vs player "+player.transform.position.x.ToString());
        if (transform.position.x > player.transform.position.x+halfScreenLength) {
            transform.Translate(new Vector3(-2*halfScreenLength, 0, 0));
            Debug.Log("planet teleported left");
        }
        else if (transform.position.x < player.transform.position.x-halfScreenLength) {
            transform.Translate(new Vector3(2*halfScreenLength, 0, 0));
            Debug.Log("planet teleported right");
        }

    }
}
