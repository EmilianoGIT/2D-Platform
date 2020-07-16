using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class LevelExit : MonoBehaviour
{
    [SerializeField] float timeNeededToConfirmExiting = 1f;
    [SerializeField] float deelayToLoadNextLevel = 2f;

    GameObject player;


    bool playerIsInFrontOfExit = false;
    bool stillCheckingIfWantToExit = true;
    bool levelIsBeingLoaded = false;

    float timeElapsed;

    private void Start()
    {
        timeElapsed = timeNeededToConfirmExiting;
    }
    private void OnTriggerStay2D(Collider2D collider)
    {
        if(collider.gameObject.layer == 10)
        {
            player = collider.gameObject;
            playerIsInFrontOfExit = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        timeElapsed = timeNeededToConfirmExiting;
        stillCheckingIfWantToExit = true;
        playerIsInFrontOfExit = false;
    }

    void Update()
    {

        if (playerIsInFrontOfExit)
        {
            bool isHoldingUp = CrossPlatformInputManager.GetAxis("Vertical") > 0;
            if (isHoldingUp && timeElapsed > 0)
            {
                timeElapsed = timeElapsed - ( 1 *Time.deltaTime);

                player.GetComponent<Player>().Exit();
                player.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
                player.transform.position = gameObject.transform.position;
                player.GetComponent<Animator>().SetBool("Exiting", true);
            }
            else if (!isHoldingUp && stillCheckingIfWantToExit)
            {
                player.GetComponent<Player>().StopExiting();
                player.GetComponent<Animator>().SetBool("Exiting", false);
                timeElapsed = timeNeededToConfirmExiting;
            }
            else
            {
                playerIsInFrontOfExit = false;
                stillCheckingIfWantToExit = false;
            }

        }
         if(!playerIsInFrontOfExit && !stillCheckingIfWantToExit && !levelIsBeingLoaded)
        {
            Debug.Log("heyy");
            StartCoroutine(LoadNextLevel());
            levelIsBeingLoaded = true;
        }
        
    }

    IEnumerator LoadNextLevel()
    {
        FindObjectOfType<GameSession>().GetComponent<Animator>().SetTrigger("FadeIn");
        yield return new WaitForSeconds(deelayToLoadNextLevel);
        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        Debug.Log(SceneManager.sceneCountInBuildSettings);
        if (currentSceneIndex + 1 >= SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(0);
        else
        SceneManager.LoadScene(currentSceneIndex + 1);
        FindObjectOfType<GameSession>().GetComponent<Animator>().SetTrigger("FadeOut");
        FindObjectOfType<GameSession>().GetComponentInChildren<Animator>().SetTrigger("Null");
    }

}
