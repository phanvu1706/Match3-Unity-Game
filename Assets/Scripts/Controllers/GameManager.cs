using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action<eStateGame> StateChangedAction = delegate { };

    public enum eLevelMode
    {
        TIMER,
        MOVES
    }

    public enum eStateGame
    {
        SETUP,
        MAIN_MENU,
        GAME_STARTED,
        PAUSE,
        GAME_OVER,
    }

    private eStateGame m_state;
    public eStateGame State
    {
        get { return m_state; }
        private set
        {
            m_state = value;

            StateChangedAction(m_state);
        }
    }


    private GameSettings m_gameSettings;


    private BoardController m_boardController;

    private UIMainManager m_uiMenu;

    private LevelCondition m_levelCondition;

    private eLevelMode m_lastLevelMode;

    private NormalItemSkin m_normalItemSkin;

    private void Awake()
    {
        State = eStateGame.SETUP;

        m_gameSettings = Resources.Load<GameSettings>(Constants.GAME_SETTINGS_PATH);

        m_uiMenu = FindObjectOfType<UIMainManager>();
        m_uiMenu.Setup(this);

        m_normalItemSkin = Resources.Load<NormalItemSkin>(Constants.NORMAL_ITEM_SKIN_PATH);
        m_normalItemSkin.SetSkin(m_gameSettings.itemSkinName);

        // Add ObjectPooling
        int preloadNormalItemCount = m_gameSettings.BoardSizeX * m_gameSettings.BoardSizeY / 7 + 3;

        ObjectPooling op = Instantiate(Resources.Load<ObjectPooling>(Constants.PREFAB_OBJECT_POOLING));
        op.PreloadObject(Constants.PREFAB_NORMAL_TYPE_ONE, preloadNormalItemCount);
        op.PreloadObject(Constants.PREFAB_NORMAL_TYPE_TWO, preloadNormalItemCount);
        op.PreloadObject(Constants.PREFAB_NORMAL_TYPE_THREE, preloadNormalItemCount);
        op.PreloadObject(Constants.PREFAB_NORMAL_TYPE_FOUR, preloadNormalItemCount);
        op.PreloadObject(Constants.PREFAB_NORMAL_TYPE_FIVE, preloadNormalItemCount);
        op.PreloadObject(Constants.PREFAB_NORMAL_TYPE_SIX, preloadNormalItemCount);
        op.PreloadObject(Constants.PREFAB_NORMAL_TYPE_SEVEN, preloadNormalItemCount);

        op.PreloadObject(Constants.PREFAB_BONUS_BOMB, 3);
        op.PreloadObject(Constants.PREFAB_BONUS_HORIZONTAL, 3);
        op.PreloadObject(Constants.PREFAB_BONUS_VERTICAL, 3);
    }

    void Start()
    {
        State = eStateGame.MAIN_MENU;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_boardController != null) m_boardController.Update();
    }


    internal void SetState(eStateGame state)
    {
        State = state;

        if (State == eStateGame.PAUSE)
        {
            DOTween.PauseAll();
        }
        else
        {
            DOTween.PlayAll();
        }
    }

    public void LoadLevel(eLevelMode mode)
    {
        m_boardController = Instantiate(Resources.Load<BoardController>(Constants.PREFAB_BOARD_CONTROLLER));// new GameObject("BoardController").AddComponent<BoardController>();
        m_boardController.StartGame(this, m_gameSettings);

        if (mode == eLevelMode.MOVES)
        {
            m_levelCondition = this.gameObject.AddComponent<LevelMoves>();
            m_levelCondition.Setup(m_gameSettings.LevelMoves, m_uiMenu.GetLevelConditionView(), m_boardController);
        }
        else if (mode == eLevelMode.TIMER)
        {
            m_levelCondition = this.gameObject.AddComponent<LevelTime>();
            m_levelCondition.Setup(m_gameSettings.LevelTime, m_uiMenu.GetLevelConditionView(), this);
        }
        m_levelCondition.ConditionCompleteEvent += GameOver;

        m_lastLevelMode = mode;
        State = eStateGame.GAME_STARTED;
    }

    internal void RestartLevel()
    {
        m_boardController.ClearAllCells();
        m_boardController.RestartGame();

        if (m_lastLevelMode == eLevelMode.MOVES)
        {
            m_levelCondition = this.gameObject.AddComponent<LevelMoves>();
            m_levelCondition.Setup(m_gameSettings.LevelMoves, m_uiMenu.GetLevelConditionView(), m_boardController);
        }
        else if (m_lastLevelMode == eLevelMode.TIMER)
        {
            m_levelCondition = this.gameObject.AddComponent<LevelTime>();
            m_levelCondition.Setup(m_gameSettings.LevelTime, m_uiMenu.GetLevelConditionView(), this);
        }
        m_levelCondition.ConditionCompleteEvent += GameOver;

        State = eStateGame.GAME_STARTED;
    }

    public void GameOver()
    {
        StartCoroutine(WaitBoardController());
    }

    internal void ClearLevel()
    {
        if (m_boardController)
        {
            m_boardController.Clear();
            Destroy(m_boardController.gameObject);
            m_boardController = null;
        }
    }

    private IEnumerator WaitBoardController()
    {
        while (m_boardController.IsBusy)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1f);

        State = eStateGame.GAME_OVER;

        if (m_levelCondition != null)
        {
            m_levelCondition.ConditionCompleteEvent -= GameOver;

            Destroy(m_levelCondition);
            m_levelCondition = null;
        }
    }
}
