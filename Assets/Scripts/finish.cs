using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class finish : MonoBehaviour {
    public Text textFim, textFim2;

    private GameObject player;

    void Start () {
        textFim.enabled = false;
        textFim2.enabled = false;
        player = GameObject.FindWithTag ("Player");
    }

    void Update () {
        //Debug.Log(Vector3.Distance(transform.position, player.transform.position));
        if (Vector3.Distance (transform.position, player.transform.position) < 3f) {
            textFim.enabled = true;
            textFim2.enabled = true;

            Time.timeScale = 0;

            if (Input.GetKey (KeyCode.Escape)) {
                Time.timeScale = 1;
                SceneManager.LoadScene ("SampleScene");
            }
        } else {
            textFim.enabled = false;
            textFim2.enabled = false;
        }
    }
}