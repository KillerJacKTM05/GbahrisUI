using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SafeZone
{
    public class InterfaceManager : MonoBehaviour
    {
        private Coroutine switchRoutine;
        private Coroutine wait;
        private Coroutine attentionR;
        private bool routineActive = false;
        public static InterfaceManager Instance { get; private set; }
        public void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }

        }
        [Header("Main Canvas Groups")]
        public CanvasGroup m_gameGroup;
        public CanvasGroup m_menuGroup;
        public CanvasGroup m_preload;
        public CanvasGroup m_mainMenu;

        [Header("Canvas Panels")]
        public CanvasGroup m_interactionGroup;
        public CanvasGroup m_inGameGroup;
        public CanvasGroup m_menuCanvasGroup;
        public CanvasGroup m_levelSelectionGroup;
        public CanvasGroup m_scenarioEndGroup;
        public CanvasGroup m_settingsGroup;
        public CanvasGroup m_mapGroup;
        public Transform m_objective;

        [Header("Script Refs")]
        public InGameUI InGameUI;
        public ObjectivePanel objectivePanelUI;
        public Map mapScript;

        private bool objectiveState = true;
        private bool arrowState = true;
        private bool mapState = false;
        private TMP_InputField playerTextField;

        private float mapLength = 58f;
        private float mapHeight = 30f;
        private Vector2 crossStartPos = new Vector2(0, 0);
        public void Start()
        {
            playerTextField = m_mainMenu.GetComponentInChildren<TMP_InputField>();
            playerTextField.onEndEdit.AddListener(delegate { PlayerKeyInput(); });
            //improvement: we can create a better solution for button toggling especially for child finding process. It's a bit messy version.
            ToggleButton(m_mainMenu.transform.Find("Panel"), "PlayButton", false);

            crossStartPos = new Vector2(-714, -380);
        }
        public void PlayerKeyInput()
        {
            if(playerTextField.text.Length > 0 && playerTextField.text.Length != 5)
            {
                attentionR = StartCoroutine(PopUp(GameManager.Instance.GetGameSettings().attentionCloud, m_mainMenu.transform.Find("Panel"), new Vector3(-410, -10, 0), 0 , 2f));
            }
            else if (playerTextField.text.Length > 0 && playerTextField.text.Length == 5)
            {
                Debug.Log("player key: " + playerTextField.text);
                DataManager.Instance.SetPlayer(playerTextField.text);
                ToggleButton(m_mainMenu.transform.Find("Panel"), "PlayButton", true);
            }
            else
            {
            }
        }
        public void ToggleButton(Transform parent, string buttonName, bool state)
        {
            parent.Find(buttonName).GetComponent<Button>().interactable = state;
        }
        public void SwitchCanvasGroups(CanvasGroup first, CanvasGroup second, int state, float timer = 1f)
        {
            switchRoutine = StartCoroutine(SwitchRoutine(first,second,state,timer));
        }
        public void ProceedAndStartGame()
        {
            SwitchCanvasGroups(m_preload, m_inGameGroup, 1);
            /*m_preload.alpha = 0;
            m_preload.blocksRaycasts = false;
            m_preload.interactable = false;*/
            GameManager.Instance.SetGameStarted(true);
        }

        public void ScenarioEnd()
        {
            Time.timeScale = 0;
        }

        public void SetRoutineBlocker(bool state)
        {
            routineActive = state;
        }
        public void MainMenu()
        {
            SwitchCanvasGroups(m_mainMenu, m_levelSelectionGroup, 1, 0);
        }
        public void StartGame()
        {
            SwitchCanvasGroups(m_levelSelectionGroup, m_preload, 1);
        }
        public void ReturnToMenu()
        {
            SwitchCanvasGroups(m_levelSelectionGroup, m_mainMenu, 1, 0);
        }
        [Tooltip("If state = 1, it's scenario end. Else, it's for pause screen.")]
        public void StopGame(int state)
        {
            if(state == 1)
            {
                if (!routineActive)
                {
                    wait = StartCoroutine(PauseRoutine(0));
                    SwitchCanvasGroups(m_inGameGroup, m_scenarioEndGroup, 1);
                }
            }
            else
            {
                if (!routineActive)
                {
                    wait = StartCoroutine(PauseRoutine(0));
                    SwitchCanvasGroups(m_inGameGroup, m_menuCanvasGroup, 1);
                }
            }
        }
        [Tooltip("If state = 1, it's scenario end. Else, it's for pause screen.")]
        public void ReturnToGame(int state)
        {
            if(state == 1)
            {
                if (!routineActive)
                {
                    RobotTask tempTask = new RobotTask();
                    tempTask.FreeRoam();
                    GameManager.Instance.AddFreeRoam(tempTask);
                    wait = StartCoroutine(PauseRoutine(1));
                    SwitchCanvasGroups(m_scenarioEndGroup, m_inGameGroup, 1);
                }
            }
            else
            {
                if (!routineActive)
                {
                    wait = StartCoroutine(PauseRoutine(1));
                    SwitchCanvasGroups(m_menuCanvasGroup, m_inGameGroup, 1);
                }
            }
        }
        [Tooltip("if state = 0, just quit. Else, save and quit.")]
        public void SaveAndQuit(int state)
        {
            if(state == 1)
            {
                DataManager.Instance.WriteToFile();
            }
            StartCoroutine(WaitAndQuit());
        }
        public bool GetObjective()
        {
            return objectiveState;
        }
        public void SetObjective(bool state)
        {
            objectiveState = state;
        }
        public bool GetArrowState()
        {
            return arrowState;
        }
        public void SetArrowState(bool state)
        {
            arrowState = state;
        }
        public bool GetMapState()
        {
            return mapState;
        }
        public void SetMapState(bool state)
        {
            mapState = state;
        }
        public void OpenCloseTab(Transform group, bool state)
        {
            if (state)
            {
                group.gameObject.SetActive(true);
            }
            else
            {
                group.gameObject.SetActive(false);
            }
        }      
        public void UpdateCrossair()
        {
            mapScript.GetCrossAirImage().rectTransform.anchoredPosition = CharacterPositionFinder();
        }
        //it represents the approximate map position of the character with the help of map border rates.
        private Vector2 CharacterPositionFinder()
        {
            float[] rates = new float[2]; 
            rates[0] = ((mapLength / 2f) + GameManager.Instance.GetPlayer().transform.position.x) / mapLength;
            rates[1] = ((mapHeight / 2f) +GameManager.Instance.GetPlayer().transform.position.z) / mapHeight;
            float positionX = mapScript.GetMapImage().rectTransform.rect.width * rates[0];
            float positionY = mapScript.GetMapImage().rectTransform.rect.height * rates[1];
            //Debug.Log("posx:" + positionX + " posy:" + positionY);
            Vector2 newPos = new Vector2(crossStartPos.x + positionX, crossStartPos.y + positionY);
            return newPos;
        }
        public IEnumerator PopUp(Transform target, Transform parent, Vector3 relativePosition, int childIndex = 0, float duration = 1f)
        {
            Transform attentionUI = Instantiate(target);
            attentionUI.SetParent(parent);
            attentionUI.SetSiblingIndex(childIndex);
            attentionUI.localPosition = relativePosition;
            yield return new WaitForSecondsRealtime(duration);
            Destroy(attentionUI.gameObject);
        }
        private IEnumerator SwitchRoutine(CanvasGroup f, CanvasGroup s,int state, float timer)
        {
            if(timer != 0)
            {
                float fl = 0;
                float totalTime = 1 / 60f * timer;
                while (fl <= 1f)
                {
                    //Debug.Log(fl);
                    if (state == 1)
                    {
                        f.alpha = Mathf.Lerp(1, 0, fl);
                        s.alpha = Mathf.Lerp(0, 1, fl);
                    }
                    else
                    {
                        f.alpha = Mathf.Lerp(0, 1, fl);
                        s.alpha = Mathf.Lerp(1, 0, fl);
                    }
                    fl += totalTime;
                    yield return new WaitForSecondsRealtime(totalTime);
                }
            }
            else
            {
                if (state == 1)
                {
                    f.alpha = 0;
                    s.alpha = 1;
                }
                else
                {
                    f.alpha = 1;
                    s.alpha = 0;
                }
            }
            if(state == 1)
            {
                f.blocksRaycasts = false;
                f.interactable = false;
                s.blocksRaycasts = true;
                s.interactable = true;
            }
            else
            {
                f.blocksRaycasts = true;
                f.interactable = true;
                s.blocksRaycasts = false;
                s.interactable = false;
            }
            routineActive = false;
        }
        private IEnumerator PauseRoutine(int time)
        {
            float timer = GameManager.Instance.GetGameSettings().realWorldTimeForEachGameHour / 60f;
            while (routineActive)
            {
                yield return new WaitForSecondsRealtime(timer);
            }
            yield return new WaitForSecondsRealtime(0.2f);
            Time.timeScale = time;
        }
        private IEnumerator WaitAndQuit()
        {
            yield return new WaitForSeconds(2f);
            Time.timeScale = 1;
            yield return new WaitForSeconds(0.2f);
            Application.Quit();
        }
    }
}

