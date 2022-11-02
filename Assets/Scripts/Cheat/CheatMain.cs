using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatMain : MonoBehaviour
{
    [SerializeField]
    internal bool topTriggered = false;
    [SerializeField] internal bool bottomTriggered = false;

    public GameObject player;

    private Collision coll;
    private Movement movement;
    // Start is called before the first frame update
    void Start()
    {
        coll = player.GetComponent<Collision>();
        movement = player.GetComponent<Movement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bottomTriggered && !topTriggered && !coll.onGround)
        {
            player.transform.Translate(new Vector2(0.01f * movement.side, 0.005f));
            print("ACK");
        }
    }
}
