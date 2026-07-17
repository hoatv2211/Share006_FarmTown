using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour {

    public AudioSource musicBG, musicrandom, soundFx;
    public Sprite onMusic, offMusic,onSound,offSound;
    public Image btnMusic, btnSound;
    [SerializeField] AudioClip[] soundrandom, sound;
    public static AudioManager Instance { get; private set; }
    bool check;
    [SerializeField] GameObject dialogExitGamegame;
    void Start()
    {
        Instance = this;
        if (PlayerPrefs.GetInt("music") == 1)
        {

        }
        else
        {
            btnMusic.GetComponent<Image>().sprite = offMusic;
            musicBG.mute = !musicBG.mute;
            musicrandom.mute = !musicrandom.mute;          
        }
        if (PlayerPrefs.GetInt("sound") == 1)
        {

        }
        else
        {
            btnSound.GetComponent<Image>().sprite = offSound;
            soundFx.mute = !soundFx.mute;     
        }
        StartCoroutine(PlayRandomAmbientSound());
    }
    public void OpenSettings()
    {
        if (!PlayerPrefs.HasKey("huongdan"))
        {
            if (check)
            {
                check = false;
                gameObject.GetComponent<Animator>().enabled = false;
                transform.GetChild(0).localScale = Vector3.zero;
                transform.GetChild(1).localScale = Vector3.zero;
                transform.GetChild(2).localScale = Vector3.zero;
            }
            else
            {
                gameObject.GetComponent<Animator>().enabled = true;
                gameObject.GetComponent<Animator>().Play("btnSetting", -1, 0);
                check = true;
            }
            click();
        }
    }
    IEnumerator PlayRandomAmbientSound()
    {
        yield return new WaitForSeconds(Random.Range(7, 14));
        musicrandom.PlayOneShot(soundrandom[Random.Range(0, soundrandom.Length)], 1f);
        StartCoroutine(PlayRandomAmbientSound());
    }
    public void ToggleBackgroundMusic()
    {
        
        if (PlayerPrefs.GetInt("music") == 1)
        {
            PlayerPrefs.SetInt("music", 0);
            btnMusic.GetComponent<Image>().sprite = offMusic;
            musicBG.mute = !musicBG.mute;
            musicrandom.mute = !musicrandom.mute;
           
        }
        else
        {
            PlayerPrefs.SetInt("music", 1);
            btnMusic.GetComponent<Image>().sprite = onMusic;
            musicBG.mute = !musicBG.mute;
            musicrandom.mute = !musicrandom.mute;
          
        }
    }
    public void ToggleSound()
    {
        if (PlayerPrefs.GetInt("sound") == 1)
        {
            PlayerPrefs.SetInt("sound", 0);
            btnSound.GetComponent<Image>().sprite = offSound;
            soundFx.mute = !soundFx.mute;
            
        }
        else
        {
            PlayerPrefs.SetInt("sound", 1);
            btnSound.GetComponent<Image>().sprite = onSound;
            soundFx.mute = !soundFx.mute;
        }
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            MainCamera.Instance.lockCam();
            dialogExitGamegame.SetActive(true);
        }
    }
    public void OpenExitDialog()
    {
        MainCamera.Instance.lockCam();
        dialogExitGamegame.SetActive(true);
    }
    public void ExitGame()
    {
        dialogExitGamegame.transform.GetChild(2).gameObject.SetActive(true);
        MobileFullVideo.instance.ShowFullNormal();
        StartCoroutine(ExitGamegame());
    }
    public void CloseExitDialog()
    {
        MainCamera.Instance.unLockCam();
        dialogExitGamegame.SetActive(false);
    }
    IEnumerator ExitGamegame()
    {
        yield return new WaitForSeconds(1.5f);
        Application.Quit();
    }
    public void click()
    {
        soundFx.PlayOneShot(sound[0], 1f);
    }
    public void PlaySawSound()
    {
        soundFx.PlayOneShot(sound[1], 1f);
    }
    public void PlayChopSound()
    {
        soundFx.PlayOneShot(sound[2], 1f);
    }
    public void PlayExplosionSound()
    {
        soundFx.PlayOneShot(sound[3], 1f);
    }
    public void ga()
    {
        soundFx.PlayOneShot(sound[4], 1f);
    }
    public void bo()
    {
        soundFx.PlayOneShot(sound[5], 1f);
    }
    public void lon()
    {
        soundFx.PlayOneShot(sound[6], 1f);
    }
    public void cuu()
    {
        soundFx.PlayOneShot(sound[7], 1f);
    }
    public void PlayTreeUpgradeSound()
    {
        soundFx.PlayOneShot(sound[8], 1f);
    }
    public void Harvest()
    {
        soundFx.PlayOneShot(sound[9], .5f);
    }
    public void lenlevel()
    {
        soundFx.PlayOneShot(sound[10], 1f);
    }
    public void bakery()
    {
        soundFx.PlayOneShot(sound[11], 1f);
    }
    public void popcorn()
    {
        soundFx.PlayOneShot(sound[12], 1f);
    }
    public void milk()
    {
        soundFx.PlayOneShot(sound[13], 1f);
    }
    public void oven()
    {
        soundFx.PlayOneShot(sound[14], 1f);
    }
    public void soup()
    {
        soundFx.PlayOneShot(sound[15], 1f);
    }
    public void CarStart()
    {
        soundFx.PlayOneShot(sound[16], 1f);
    }
    public void CarEnd()
    {
        soundFx.PlayOneShot(sound[17], 1f);
    }
    public void Duck()
    {
        soundFx.PlayOneShot(sound[18], 1f);
    }
    public void bufalo()
    {
        soundFx.PlayOneShot(sound[19], 1f);
    }
    public void goat()
    {
        soundFx.PlayOneShot(sound[20], 1f);
    }
}
