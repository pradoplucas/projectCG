using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class textCatch : MonoBehaviour {
    public Text texto;
    private GameObject player;

    void Start () {
        texto.enabled = false;
        player = GameObject.FindWithTag ("Player");
    }

    // Update is called once per frame
    void Update () {
        if (Vector3.Distance (transform.position, player.transform.position) < 1.5f &&
            Input.GetAxis ("Down") > 0) {
            texto.enabled = true;
        } else {
            texto.enabled = false;
        }
    }
}