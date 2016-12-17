using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartScript : MonoBehaviour {

    public Canvas menuCanvas;
    public Text scoreText;
    public Text coinText;
    public StackScript stack;

    public AudioSource musicSrc;
    public Sprite musicOn;
    public Sprite musicOff;
    public Image musicImage;

    public AudioSource sfxSrc;
    public Sprite sfxOn;
    public Sprite sfxOff;
    public Image sfxImage;

    public GameObject settingsMenu;

    public GameObject exitMenu;

    public GameObject shopMenu;

    // Use this for initialization
    void Start () {

        exitMenu.SetActive(false);
        shopMenu.SetActive(false);
        menuCanvas.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(false);
        coinText.text = PlayerPrefs.GetInt("coins").ToString();
        settingsMenu.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {

        coinText.text = PlayerPrefs.GetInt("coins").ToString();

        if (Input.GetKeyDown(KeyCode.Escape) && settingsMenu.activeInHierarchy == true && exitMenu.activeInHierarchy == false && shopMenu.activeInHierarchy == false)
        {
            StartCoroutine(SettingsOff());
        }

        if (Input.GetKeyDown(KeyCode.Escape) && settingsMenu.activeInHierarchy == false && exitMenu.activeInHierarchy == false && shopMenu.activeInHierarchy == false)
        {
            stack.gamePaused = true;
            StartCoroutine(ExitOn());
        }

        if (Input.GetKeyDown(KeyCode.Escape) && settingsMenu.activeInHierarchy == false && exitMenu.activeInHierarchy == true && shopMenu.activeInHierarchy == false)
        {
            stack.EndGame();
            Application.Quit();
        }

        if(Input.GetKeyDown(KeyCode.Escape) && shopMenu.activeInHierarchy == true && exitMenu.activeInHierarchy == false && settingsMenu.activeInHierarchy == false)
        {
            shopMenu.SetActive(false);
        }
    }

    public void OnPlayClick()
    {
        menuCanvas.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(true);
        stack.gameStarted = true;
    }

    public void OnSettingsClick()
    {
        if(settingsMenu.activeInHierarchy == false)
            StartCoroutine(SettingsOn());
        else
            StartCoroutine(SettingsOff());
    }

    public void OnExitYes()
    {
        stack.EndGame();
        Application.Quit();
    }

    public void OnExitNo()
    {
        StartCoroutine(ExitOff());
        stack.gamePaused = false;
    }

    IEnumerator ExitOn()
    {
        yield return new WaitForSeconds(0.01f);
        exitMenu.SetActive(true);
    }

    IEnumerator ExitOff()
    {
        yield return new WaitForSeconds(0.01f);
        exitMenu.SetActive(false);
    }

    IEnumerator SettingsOn()
    {
        yield return new WaitForSeconds(0.01f);
        settingsMenu.SetActive(true);
    }

    IEnumerator SettingsOff()
    {
        yield return new WaitForSeconds(0.01f);
        settingsMenu.SetActive(false);
    }

    public void MusicOnOff()
    {
        if (musicSrc.mute == false)
            StartCoroutine(MuteMusic());
        else
            StartCoroutine(UnmuteMusic());
    }

    IEnumerator MuteMusic()
    {
        yield return new WaitForSeconds(0.01f);
        musicImage.sprite = musicOff;
        musicSrc.mute = true;
    }

    IEnumerator UnmuteMusic()
    {
        yield return new WaitForSeconds(0.01f);
        musicImage.sprite = musicOn;
        musicSrc.mute = false;
    }


    public void sfxOnOff()
    {
        if (sfxSrc.mute == false)
            StartCoroutine(MuteSfx());
        else
            StartCoroutine(UnmuteSfx());
    }

    IEnumerator MuteSfx()
    {
        yield return new WaitForSeconds(0.01f);
        sfxImage.sprite = sfxOff;
        sfxSrc.mute = true;
    }

    IEnumerator UnmuteSfx()
    {
        yield return new WaitForSeconds(0.01f);
        sfxImage.sprite = sfxOn;
        sfxSrc.mute = false;
    }

    public void OnShopClick()
    {
        if (settingsMenu.activeInHierarchy == true)
            settingsMenu.SetActive(false);
        shopMenu.SetActive(true);
    }
}
