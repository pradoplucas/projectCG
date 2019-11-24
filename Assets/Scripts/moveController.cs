using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Criação do Componente..
//Para adicionar características de física no 
//personagem sem precisar usar o RigidBody..
[RequireComponent (typeof (CharacterController))]

public class moveController : MonoBehaviour {

    //keyFinal é a variável que verifica se o 
    //player está em posse da chave final..
    public bool keyFinal;

    //porta é uma variável que vai receber a porta
    //que irá abrir, e portaRef é a referência que
    //estou utilizando para verificar a distância
    //do player para abrir a porta..
    private GameObject porta, portaRef;
    public Text textoSair;

    //Para controlar a câmera que vai
    //ficar "seguindo" o player..
    GameObject cameraPlayer;

    //Para controlar a lanterna que o player segura..
    Light flashlight;
    //public Camera camerateste;

    //Um vetor de 3 posições que vai armazenar
    //a direção do movimento do player,
    //inicia com zero pra não andar sozinho..
    Vector3 moveDirection = Vector3.zero;

    //Variável para manipular o characterController..
    CharacterController controller;

    //Para manipular a rotação da câmera..
    float rotX = 0f, rotY = 0f;

    //Altura da câmera se estiver em pé ou abaixado..
    private float min = -0.2f, max = 0.75f;
    Vector3 posCameraStand = new Vector3 (0, 0.75f, 0);
    Vector3 posCameraDown = new Vector3 (0, -0.2f, 0);

    //Velocidade com que o player irá se mover, 
    //sensibilidade do mouse e um fator para
    //o player se abaixar e levantar de maneira
    //mais suave..
    private float speed, sensitivity, factor;

    private int stepCount, auxStepCount;

    private bool isLower;

    void Start () {

        //Armazena na variável camera uma câmera
        //que está como componente filho de player..
        cameraPlayer = GetComponentInChildren (typeof (Camera)).transform.gameObject;

        //Coloca a camera na altura da cabeça de player..
        cameraPlayer.transform.localPosition = posCameraStand;

        //Camera começa com rotação 0, 0, 0..
        cameraPlayer.transform.localRotation = Quaternion.identity;
        //cameraPlayer.transform.localEulerAngles = new Vector3(10, 210, 0);

        //Inicialização das variáveis..
        keyFinal = false;
        speed = 1.5f;
        sensitivity = 1000f;
        factor = 3f;
        textoSair.enabled = false;

        stepCount = 0;
        auxStepCount = 10;

        isLower = false;

        //Encontra os GameObjects com as respectivas tags
        //e atribui nas variáveis..
        porta = GameObject.FindWithTag ("porta");
        portaRef = GameObject.FindWithTag ("portaRef");

        //Para indicar que é um GameObject 
        //do tipo player, facilita quando
        //a aplicação é grande..
        transform.tag = "Player";

        //Controller recebe o componente 
        //CharacterController..
        controller = GetComponent<CharacterController> ();
        flashlight = GetComponentInChildren<Light> ();

        //cameraPlayer.gameObject.SetActive(false);
        //camerateste.gameObject.SetActive(true);

        //Deixa o cusor escondido quando este está
        //posicionado na tela do game..
        Cursor.visible = false;

        //porta.transform.Rotate (0, 90, 0);
    }

    void Update () {
        //Para o player se mover sempre para 
        //onde a câmera está virada quando
        //apertar para frente (W ou SetaUp)..
        Vector3 direcForward = new Vector3 (cameraPlayer.transform.forward.x, 0, cameraPlayer.transform.forward.z);

        //Para o player se mover sempre para 
        //a lateral de onde a câmera está virada
        //quando apertar para o lado (A/D ou <-/->).. 
        Vector3 direcSide = new Vector3 (cameraPlayer.transform.right.x, 0, cameraPlayer.transform.right.z);

        //Normalizar os vetores para que o valor
        //intermediário deles não altere na velocidade..
        direcForward.Normalize ();
        direcSide.Normalize ();

        //As duas variáveis só tem algum valor
        //quando são pressionadas as teclas
        //de movimento, caso contrário, são zero..

        float moveForward = Input.GetAxis ("Vertical");
        float moveSide = Input.GetAxis ("Horizontal");

        direcForward *= moveForward;
        direcSide *= moveSide;

        //A soma dos dois vetores para determinar
        //qual a direção exata que está se movendo..
        Vector3 direcFinal = direcForward + direcSide;

        //Normalizar a direção final..
        //Quando eu tenho algum movimento,
        //pra qualquer lado, direcFinal vai
        //ser ou menor que -1 ou maior que +1,
        //elevando ao quadrado, esse valor é maior
        //que 1, sendo assim, eu normalizo ele em 1,
        //pois eu que isso seja só uma direção, e não
        //um fator de velocidade na hora de multiplicar
        //lá na frente..
        if (direcFinal.sqrMagnitude > 1) {
            direcFinal.Normalize ();
        }

        //"Gravidade"
        Vector3 gravity = transform.position;

        //Sempre que o player se movimentar 0.01
        //unidade em y, ele é reposicionado para baixo..
        if(gravity.y > 0.76f)
            gravity.y = 0.75f - gravity.y;
        else gravity.y = 0;

        //Determina as direções..
        moveDirection = new Vector3 (direcFinal.x, gravity.y, direcFinal.z) * speed * Time.deltaTime;

        //Faz o player se mover..
        controller.Move (moveDirection);

        Debug.Log(moveForward);

        if(((moveForward != 0f) || (moveSide != 0f))){
            stepCount++;
            if(stepCount >= auxStepCount){
                GetComponent<AudioSource>().Play();

                if(!isLower) stepCount = -15;

                else stepCount = -20;
            }
        }
        else {
            stepCount = 0;
        }

        Debug.Log(stepCount);

        //Chama os métodos necessários..
        MoveCamera ();      //Para mover a câmera..
        LightUp ();         //Para ligar/desligar a lanterna
        MoveCtrl ();        //Para se abaixar..
        OpenKeyFinal ();    //Para abrir a porta..
    }

    public void SetKeyFinal (bool e) {
        keyFinal = e;
    }

    float ang = 0f;

    void OpenKeyFinal () {

        //Debug.Log(Vector3.Distance (transform.position, portaRef.transform.position));
        if (Vector3.Distance (transform.position, portaRef.transform.position) < 1.6f && keyFinal) {
            textoSair.enabled = true;

            if (Input.GetKey ("e")) {
                ang += Time.deltaTime * 3f;

                if (ang > 3f) {
                    ang = 3f;
                    textoSair.enabled = false;
                    keyFinal = false;
                }

                Debug.Log(ang);

                porta.transform.Rotate (0, ang, 0);
            }
        }
        else{
            textoSair.enabled = false;
        }
    }

    float lastF;
    bool estado = true;
    int aux = 1;

    void LightUp () {
        float actF = Input.GetAxis ("Flashlight");

        if (lastF < actF && aux == 1) {
            aux = 0;
            estado = !estado;
        } else if (lastF > actF) {
            aux = 1;
        }

        flashlight.enabled = estado;
        lastF = actF;
    }

    float lastCtrl = 0f, y = 0f, auxControl = 2;

    //Movimento de abaixar e levantar..
    void MoveCtrl () {
        //Salva o valor atual da tecla ctrl 
        //esquerdo na variável actCtrl, sendo
        //que esta varia de 0 a 1..
        float actCtrl = Input.GetAxis ("Down"), ctrl;

        //Atribiui o valor atual do ctrl esquerdo
        //na variável ctrl, pois, se não entrar
        //no if ou no else if abaixo, significa
        //que actCtrl é igual a 0 ou a 1..
        ctrl = actCtrl;

        //Verifica se a última informação do ctrl
        //é menor do que a atual, se sim, significa
        //que a tecla está sendo apertada,
        //se não, o usuário parou de apertar,
        //e coloca um valor absoluto de 1 ou
        //0 para ctrl;
        if (lastCtrl < actCtrl) {
            ctrl = 1f;
            auxControl = 1;
        } else if (lastCtrl > actCtrl) {
            ctrl = 0f;
            auxControl = 0;
        }

        //Abaixa ou levanta o personagem..
        //As duas primeiras linhas do if e do
        //else são para o player abaixar e levantar
        //suavemente, a velocidade também é diminuida
        //quando se está abaixado..
        if (ctrl == 1f && auxControl == 1) {
            isLower = true;
            y -= Time.deltaTime * factor;
            if (y < min) {
                y = min;
                auxControl = 2;
            }
            cameraPlayer.transform.localPosition = new Vector3 (0, y, 0);
            speed = 1f;
            controller.height = 0.1f; //0.5f
            controller.radius = 0.1f;
            controller.center = new Vector3 (0, -0.560005f, 0); //(0, 0.30f, 0);
        } else if (ctrl == 0f && auxControl == 0){
            isLower = false;
            y += Time.deltaTime * factor;
            if (y > max) {
                y = max;
                auxControl = 2;
            }
            cameraPlayer.transform.localPosition = new Vector3 (0, y, 0);
            speed = 1.5f;
            controller.height = 1.5f;
            controller.radius = 0.3f;
            controller.center = new Vector3 (0, 0.08f, 0);
        }

        lastCtrl = actCtrl;
    }

    //Função para mover a câmera..
    void MoveCamera () {
        //Determina as rotações em x e y..
        rotX += Input.GetAxis ("Mouse X");
        rotY += Input.GetAxis ("Mouse Y");

        rotX = ClampCamera (rotX, -360, 360);
        rotY = ClampCamera (rotY, -80, 80);

        Quaternion xQuat = Quaternion.AngleAxis (rotX, Vector3.up);
        Quaternion yQuat = Quaternion.AngleAxis (rotY, -Vector3.right);

        Quaternion rotFinal = Quaternion.identity * xQuat * yQuat;
        cameraPlayer.transform.localRotation = Quaternion.Lerp (cameraPlayer.transform.localRotation, rotFinal, Time.deltaTime * sensitivity);
    }

    //Função para impedir que o angulo 
    //esteja fora dos limites..
    float ClampCamera (float angle, float min, float max) {
        if (angle < -360) {
            angle = 0;
        }
        if (angle > 360) {
            angle = 0;
        }

        return Mathf.Clamp (angle, min, max);
    }
}