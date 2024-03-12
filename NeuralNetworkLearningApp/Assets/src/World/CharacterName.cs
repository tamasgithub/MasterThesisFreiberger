using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterName : MonoBehaviour
{
    private Transform player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(2 * transform.position - player.position);
    }
}
