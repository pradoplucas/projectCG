using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class textKey : MonoBehaviour {
    public Text textoE, textoKey;
    public Image imagem;

    private GameObject player;
    private float distance, block;
    private bool catche;

    void Start () {
        distance = 0.5f;
        block = 0;
        textoE.enabled = false;
        textoKey.enabled = false;
        imagem.enabled = false;
        catche = false;
        player = GameObject.FindWithTag ("Player");
    }

    void Update () {
        if (Vector3.Distance (transform.position, player.transform.position) < distance &&
            Input.GetAxis ("Down") > 0 && block == 0) {
            catche = true;
            textoE.enabled = catche;
        } else {
            catche = false;
            textoE.enabled = catche;
        }

        catchKeyDown ();

        catchKeyUp ();
    }

    void catchKeyDown () {
        if (catche && Input.GetAxis ("Action") > 0) {
            textoE.enabled = false;
            textoKey.enabled = true;
            imagem.enabled = true;
            block = 1;
            Time.timeScale = 0;
        }
    }

    void catchKeyUp () {
        if (Input.GetKey ("space") && block == 1) {
            Time.timeScale = 1;
            imagem.enabled = false;
            textoKey.enabled = false;
            catche = false;
            block = -1;

            SpriteRenderer sprite = GetComponent<SpriteRenderer> ();
            sprite.enabled = false;
            moveController chaves = player.GetComponent<moveController> ();
            chaves.setChaveFinal (true);
        }

    }
}