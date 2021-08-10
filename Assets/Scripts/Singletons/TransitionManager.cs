using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;
using UnityEngine.UI;
public class TransitionManager : MonoBehaviour
{
    //for making transitions, Make a trigger with code name "Switch"
    public TransitionScene[] transitions;

    static GameObject instance;
    private int currentTransitionID;
    private bool btnClick;

    void Awake()
    {
        if (instance == null)
        {
            instance = gameObject;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        foreach(TransitionScene obj in transitions)
        {
            obj.animator.gameObject.SetActive(false);
        }
        currentTransitionID = -1;
    }

    public void OpenScene(int sceneID, int transitionID) 
    {
        if(currentTransitionID == -1)
        {
            if(transitionID <= transitions.Length)
            {
                currentTransitionID = transitionID;
                StartCoroutine(Transition(sceneID, transitionID));
            }
        }
    }

    IEnumerator Transition(int sceneID, int transitionID)
    {
        bool hasLoading = transitions[transitionID].hasLoadingBar;
        bool hasClicky = transitions[transitionID].hasClickToContinue;
        transitions[transitionID].animator.gameObject.SetActive(false);

        if (hasLoading)
        {
            transitions[transitionID].loadSlider.gameObject.SetActive(true);
            transitions[transitionID].loadText.gameObject.SetActive(true);
            transitions[transitionID].loadText.text = "0%";
            transitions[transitionID].loadSlider.value = 0;
        }
        if (hasClicky)
        {
            btnClick = false;
            transitions[transitionID].clicker.SetActive(false);
        }
        transitions[transitionID].animator.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(2);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneID);
        operation.allowSceneActivation = false;
        float progress = 0;
        while (progress < 0.9)
        {
            progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (hasLoading)
            {
                transitions[transitionID].loadText.text = Mathf.RoundToInt(progress * 100).ToString() + "%";
                transitions[transitionID].loadSlider.value = progress;
            }
            yield return null;
        }
        if (hasClicky)
        {
            transitions[transitionID].clicker.SetActive(true);
            transitions[transitionID].loadText.text = "100%";
            transitions[transitionID].loadSlider.value = 1;
            while (!btnClick)
            {
                yield return null;
            }
            transitions[transitionID].clicker.SetActive(false);
        }
        if (hasLoading)
        {
            transitions[transitionID].loadSlider.gameObject.SetActive(false);
            transitions[transitionID].loadText.gameObject.SetActive(false);
        }
        if (hasClicky)
        {
            transitions[transitionID].clicker.SetActive(false);
        }
        yield return new WaitForEndOfFrame();
        operation.allowSceneActivation = true;
        transitions[transitionID].animator.SetTrigger("Switch");
        currentTransitionID = -1;
    }

    [Serializable]
    public class TransitionScene
    {
        public Animator animator;
        public bool hasLoadingBar;
        public bool hasClickToContinue;
        [Tooltip("loading make sure loading is true")]
        public Slider loadSlider;
        public Text loadText;
        public GameObject clicker;
    }
}
