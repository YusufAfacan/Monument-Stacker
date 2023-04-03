using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private Transform player;
    private Vector3 offset;

    private void Awake()
    {
        player = FindAnyObjectByType<Player>().transform;
    }
    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - player.position;
    }

    private void LateUpdate()
    {
        transform.position = player.position + offset;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
