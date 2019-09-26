using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class moveObject : MonoBehaviour {
    //Parâmetro da distância que o objeto ficará do usuário
    //quando for "pego", a distância máxima que o objeto
    //pode estar para ser pego, e a força com a qual ele
    //pode ser arremessado..
    float distance, scale;
    [Space (10)]
    
    //As sprites que mostram na tela a imagem da mão aberta
    //ou da mão fechada..
    public Image maoFechada, maoAberta;
    public Text textCatch;

    //Verifica se o objeto pode se mover (de acordo com a
    //distância e obstáculos na frente), e se ele está se
    //movendo (quando apertar o botão esquerdo do mouse);
    bool move, movendo;
    
    //===================================================================================
    float rotX, rotY, sensitivity, distanceScreen;
    //===================================================================================

    Vector3 finalPonto, direcao, tempSpeed;

    GameObject goRaycast;
    RaycastHit raycast;
    Camera mainCamera;
    Rigidbody rbd;

    //Awake para iniciar antes do start de outros scripts,
    //para previnir erros..
    void Awake () {
        //Inicializações necessárias..
        scale = 0.7f;
        distance = 2;
        sensitivity = 1000f;
        mainCamera = Camera.main;

        //Como o script está na câmera, o root pega o 
        //Player, que é o mais alto na hierarquia..
        GameObject setLayer = transform.root.gameObject;
        //Determina a layer 2 para todos os filhos de 
        //Player, para eles ignorarem o raycast de colisão..
        setLayer.layer = 2;
        //Foreach melhor coisa s2..
        foreach (Transform components in setLayer.GetComponentsInChildren<Transform> (true)) {
            components.gameObject.layer = 2;
        }

        maoFechada.enabled = false;
        maoAberta.enabled = false;

    }

    void Update () {
        textCatch.enabled = false;

        finalPonto = transform.position + transform.forward * distance;
        if (Physics.Raycast (transform.position, transform.forward, out raycast, 1f)) {
            //Verifica se é possível pegar o cubo..
            if (Vector3.Distance (transform.position, raycast.point) <= 1 && raycast.transform.CompareTag ("moveObject")) {
                move = true;
            } else {
                move = false;
            }

            //Pega o Cubo..
            if (Input.GetKeyDown (KeyCode.Mouse0) && move) {
                if (raycast.rigidbody) {
                    raycast.rigidbody.useGravity = true;
                    distance = Vector3.Distance (transform.position, raycast.point);
                    goRaycast = raycast.transform.gameObject;
                    movendo = true;
                }
            }
        } else {
            move = false;
        }

        //Aproximação e distanciamento..
        distance += Input.GetAxis ("Mouse ScrollWheel") * 5.0f;
        distance = Mathf.Clamp (distance, 1.5f, 3f);

        //Cria um RB pro cubo..
        if (goRaycast) {
            rbd = goRaycast.GetComponent<Rigidbody> ();
        }

        //Solta o cubo..
        if (Input.GetKeyUp (KeyCode.Mouse0) && goRaycast) {
            rbd.useGravity = true;
            goRaycast = null;
            rbd = null;
            movendo = false;
        }
        //Arremessa o cubo..
        if (Input.GetKeyDown (KeyCode.Mouse1) && goRaycast) {
            direcao = finalPonto - transform.position;
            direcao.Normalize ();
            rbd.useGravity = true;
            rbd.AddForce (direcao * 400);
            goRaycast = null;
            rbd = null;
            movendo = false;
        }
        //Distância muito longa para pegar..
        if (goRaycast) {
            if (Vector3.Distance (transform.position, goRaycast.transform.position) > 6) {
                rbd.useGravity = true;
                goRaycast = null;
                rbd = null;
                movendo = false;
            }
        }

        //Movimento de rotação..
        if (goRaycast && mainCamera) {
            if (Input.GetKey (KeyCode.R)) {
                rotX = Input.GetAxis ("Mouse X") * Time.deltaTime * sensitivity;
                rotY = Input.GetAxis ("Mouse Y") * Time.deltaTime * sensitivity;
                goRaycast.transform.Rotate (mainCamera.transform.up, -rotX, Space.World);
                goRaycast.transform.Rotate (mainCamera.transform.right, rotY, Space.World);
            }
        }

        //Mudança de escala..
        if (goRaycast && mainCamera) {
            if (Input.GetKeyDown (KeyCode.C)) {
                scale += 0.1f;
                if(scale > 1.6f) scale = 0.7f;
                Vector3 newScale = new Vector3(scale, scale, scale);
                goRaycast.transform.localScale = newScale;
            }
        }

        //Mão na tela
        if (move && !movendo && maoAberta) {
            maoAberta.enabled = true;
            maoFechada.enabled = false;
            textCatch.enabled = true;
        } else if (movendo && maoFechada) {
            maoAberta.enabled = false;
            maoFechada.enabled = true;
            textCatch.enabled = false;
        } else {
            maoAberta.enabled = false;
            maoFechada.enabled = false;
        }
    }

    //O correlacionamento do Rigidbody com a
    //aplicação foi colocado dentro do FixedUpdate,
    //pois assim se tem um controle melhor e 
    //objetivo do distanciamento e aproximação do cubo..
    void FixedUpdate () {
        if (goRaycast) {
            rbd = goRaycast.GetComponent<Rigidbody> ();
            rbd.angularVelocity = new Vector3 (0, 0, 0);
            tempSpeed = (finalPonto - rbd.transform.position);
            tempSpeed.Normalize ();
            distanceScreen = Vector3.Distance (finalPonto, rbd.transform.position);
            distanceScreen = Mathf.Clamp (distanceScreen, 0, 1);
            rbd.velocity = Vector3.Lerp (rbd.velocity, tempSpeed * 15f * distanceScreen, Time.deltaTime * 12);
        }
    }
}