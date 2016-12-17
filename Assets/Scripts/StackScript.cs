using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StackScript : MonoBehaviour
{

    public Text scoreText;
    public Text highscoreText;
    public GameObject endPanel;
    public Color32[] gameColors = new Color32[4];
    public Material stackMat;

    public AudioClip failSound;
    public AudioClip perfectSound;
    public AudioSource audioSrc;

    public bool gameStarted;
    public bool gamePaused;

    private const float BOUNDS_SIZE = 3.5f;
    private const float STACK_MOVING_SPEED = 0.1f;
    private const float ERROR_MARGIN = 0.15f;
    private const float STACK_BOUNDS_GAIN = 0.25f;

    private const int COMBO_GAIN = 4;

    private GameObject[] stack;

    private int scoreCount = 0;
    private int stackIndex;
    private int combo = 0;
    private int coins;
    private int coinCounter = 0;

    private float tileTransition = 0.0f;
    private float tileSpeed = 2.5f;
    private float secondaryPos;

    private bool isMovingOnX = true;
    private bool gameOver = false;

    private Vector3 desiredPos;
    private Vector3 lastTilePos;

    private Vector2 stackBounds = new Vector2(BOUNDS_SIZE, BOUNDS_SIZE);

    void Start ()
    {
        coins = PlayerPrefs.GetInt("coins");
        gameStarted = false;
        endPanel.SetActive(false);
        stack = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            stack[i] = transform.GetChild(i).gameObject;
            ColorMesh(stack[i].GetComponent<MeshFilter>().mesh);
        }

        stackIndex = transform.childCount - 1;
	}
	
	void Update ()
    {
        if (gameOver)
            return;

        //if(Input.GetKeyDown(KeyCode.F))
        //{
        //    for (int i = 0; i < transform.childCount; i++)
        //    {
        //        Renderer rend = transform.GetChild(i).GetComponent<Renderer>();
        //        //rend.material.shader = Shader.Find("MK_Mobile-Diffuse");
        //        rend.material.SetColor("_MKGlowColor", Color.red);
        //        rend.material.SetColor("_MKGlowTexColor", Color.red);

        //    }
        //}

        if (!gamePaused)
        {
            if (gameStarted)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (PlaceTile())
                    {
                        SpawnTile();
                        scoreCount++;
                        coinCounter++;
                        if (coinCounter >= 10)
                        {
                            coins++;
                            PlayerPrefs.SetInt("coins", coins);
                            coinCounter = 0;
                        }
                        scoreText.text = scoreCount.ToString();
                    }
                    else
                        EndGame();
                }

                MoveTile();

                transform.position = Vector3.Lerp(transform.position, desiredPos, STACK_MOVING_SPEED);
            } 
        }
	}

    private void SpawnTile()
    {
        lastTilePos = stack[stackIndex].transform.localPosition;
        stackIndex--;

        if (stackIndex < 0)
            stackIndex = transform.childCount - 1;

        desiredPos = (Vector3.down) * scoreCount;
        stack[stackIndex].transform.localPosition = new Vector3(0, scoreCount, 0);
        stack[stackIndex].transform.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

        ColorMesh(stack[stackIndex].GetComponent<MeshFilter>().mesh);
    }

    private bool PlaceTile()
    {
        Transform t = stack[stackIndex].transform;

        if(isMovingOnX)
        {
            float deltaX = lastTilePos.x - t.position.x;
            if(Mathf.Abs(deltaX) > ERROR_MARGIN)
            {
                audioSrc.pitch = 1f;
                combo = 0;
                stackBounds.x -= Mathf.Abs(deltaX);
                if (stackBounds.x <= 0)
                    return false;

                float middle = lastTilePos.x + t.localPosition.x / 2;
                t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                CreateRubble(       
                                new Vector3((t.position.x > 0)
                                                ? t.position.x + (t.localScale.x / 2)
                                                : t.position.x - (t.localScale.x / 2),
                                                t.position.y, t.position.z),
                                new Vector3(Mathf.Abs(deltaX), 1, t.localScale.z)
                            );
                t.localPosition = new Vector3(middle - (lastTilePos.x / 2), scoreCount, lastTilePos.z);
                audioSrc.PlayOneShot(failSound);
            }
            else
            {
                if (combo > COMBO_GAIN)
                {
                    stackBounds.x += STACK_BOUNDS_GAIN;
                    if (stackBounds.x > BOUNDS_SIZE)
                        stackBounds.x = BOUNDS_SIZE;
                    stackBounds.y += STACK_BOUNDS_GAIN;
                    if (stackBounds.y > BOUNDS_SIZE)
                        stackBounds.y = BOUNDS_SIZE;
                    float middle = lastTilePos.x + t.localPosition.x / 2;
                    t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

                    t.localPosition = new Vector3(middle - (lastTilePos.x / 2), scoreCount, lastTilePos.z);
                }
                combo++;
                t.localPosition = new Vector3(lastTilePos.x, scoreCount, lastTilePos.z);   
                audioSrc.PlayOneShot(perfectSound);
                if (combo <= COMBO_GAIN + 1)
                    audioSrc.pitch += 0.1f;
            }
        }
        else
        {
            float deltaZ = lastTilePos.z - t.position.z;
            if (Mathf.Abs(deltaZ) > ERROR_MARGIN)
            {
                audioSrc.pitch = 1f;
                combo = 0;
                stackBounds.y -= Mathf.Abs(deltaZ);
                if (stackBounds.y <= 0)
                    return false;

                float middle = lastTilePos.z + t.localPosition.z / 2;
                t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                CreateRubble(
                         new Vector3(t.position.x,
                                     t.position.y,
                                     (t.position.z > 0)
                                        ? t.position.z + (t.localScale.z / 2)
                                        : t.position.z - (t.localScale.z / 2)),
                         new Vector3(t.localScale.x, 1, Mathf.Abs(deltaZ))
        );
                t.localPosition = new Vector3(lastTilePos.x, scoreCount, middle - (lastTilePos.z / 2));
                audioSrc.PlayOneShot(failSound);
            }
            else
            {
                if (combo > COMBO_GAIN)
                {
                    stackBounds.x += STACK_BOUNDS_GAIN;
                    if (stackBounds.x > BOUNDS_SIZE)
                        stackBounds.x = BOUNDS_SIZE;
                    stackBounds.y += STACK_BOUNDS_GAIN;
                    if (stackBounds.y > BOUNDS_SIZE)
                        stackBounds.y = BOUNDS_SIZE;
                    float middle = lastTilePos.z + t.localPosition.z / 2;
                    t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                    t.localPosition = new Vector3(lastTilePos.x, scoreCount, middle - (lastTilePos.z / 2));
                }
                combo++;
                t.localPosition = new Vector3(lastTilePos.x, scoreCount, lastTilePos.z);
                audioSrc.PlayOneShot(perfectSound);
                if (combo <= COMBO_GAIN + 1)
                    audioSrc.pitch += 0.1f;
            }
        }

        secondaryPos = (isMovingOnX) ? t.localPosition.x : t.localPosition.z;
        isMovingOnX = !isMovingOnX;
        
        return true;
    }

    private void MoveTile()
    {
        if (gameOver)
            return;

        tileTransition += Time.deltaTime * tileSpeed;
        if (isMovingOnX)
            stack[stackIndex].transform.localPosition = new Vector3(Mathf.Sin(tileTransition) * (BOUNDS_SIZE + 0.5f), scoreCount, secondaryPos);
        else
            stack[stackIndex].transform.localPosition = new Vector3(secondaryPos, scoreCount, Mathf.Sin(tileTransition) * (BOUNDS_SIZE + 0.5f));
    }

    private void CreateRubble(Vector3 pos, Vector3 scale)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.AddComponent<Rigidbody>();

        go.GetComponent<MeshRenderer>().material = stackMat;
        ColorMesh(go.GetComponent<MeshFilter>().mesh);
    }

    private void ColorMesh(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        Color32[] colors = new Color32[vertices.Length];
        float f = Mathf.Sin(scoreCount * 0.15f);
        for (int i = 0; i < vertices.Length; i++)
        {
            colors[i] = LerpColor(gameColors[0], gameColors[1], gameColors[2], gameColors[3], f);
        }

        mesh.colors32 = colors;
    }

    private Color32 LerpColor(Color32 a, Color32 b, Color32 c, Color32 d, float t)
    {
        if (t < 0.33f)
            return Color.Lerp(a, b, t / 0.33f);
        else if (t < 0.66f)
            return Color.Lerp(b, c, (t - 0.33f) / 0.33f);
        else
            return Color.Lerp(c, d, (t - 0.66f) / 0.66f);

    }

    public void EndGame()
    {
        if (PlayerPrefs.GetInt("score") < scoreCount)
            PlayerPrefs.SetInt("score", scoreCount);

        PlayerPrefs.SetInt("coins", coins);
        gameOver = true;
        highscoreText.text = PlayerPrefs.GetInt("score").ToString();
        endPanel.SetActive(true);
        if (stack[stackIndex] != null)
        {
            stack[stackIndex].AddComponent<Rigidbody>();
        }
    }

    public void OnButtonClick(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
