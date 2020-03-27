using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINotificationsManager : MonoBehaviour
{
    public RectTransform MenuPanel;
    public RectTransform TimelinePanel;
    public TimelineTicker timelikeTicker;
    public RectTransform DebuggerPanel;
    public RectTransform notificationsPanel; 
    public RectTransform ButtonPanel;
    public RectTransform scoreBoardPanel; 

    private bool isDebuggerPanelEnabled;
    private bool isMenuPanelEnabled;

    public TikTokHeadBopTimeline headBopTimeline; 

    public void Start()
    {
        setUIConfiguration(); 
    }

    public void setUIConfiguration()
    {
        MenuPanel.gameObject.SetActive(true);
        TimelinePanel.gameObject.SetActive(false);
        DebuggerPanel.gameObject.SetActive(false);
        ButtonPanel.gameObject.SetActive(true);
        notificationsPanel.gameObject.SetActive(false);
        timelikeTicker.gameObject.SetActive(false);

        isDebuggerPanelEnabled = false;
        isMenuPanelEnabled = true; 
    }

    #region button methods
    public void PlayTimeline()
    {
        TimelinePanel.gameObject.SetActive(true);
        timelikeTicker.gameObject.SetActive(true);
        headBopTimeline.timelineState = TikTokHeadBopTimeline.TimelineState.Play;


        notificationsPanel.gameObject.SetActive(true);
        StartCoroutine(notificationsPanelTimer());
        MenuPanel.gameObject.SetActive(false);
        scoreBoardPanel.gameObject.SetActive(true);
        isMenuPanelEnabled = false;

    }

    public void PauseTimeline()
    {
        headBopTimeline.timelineState = TikTokHeadBopTimeline.TimelineState.Pause;
    }

    public void StopTimeline()
    {
        headBopTimeline.timelineState = TikTokHeadBopTimeline.TimelineState.Stop;
        TimelinePanel.gameObject.SetActive(false);
        scoreBoardPanel.gameObject.SetActive(false);

    }

    public void onDebuggerPanelButtonPressed()
    {
        if(isDebuggerPanelEnabled)
        {
            DisableDebuggerPanel();
        }
        else
        {
            EnableDebuggerPanel();
        }
    }

    private void EnableDebuggerPanel()
    {
        DebuggerPanel.gameObject.SetActive(true);
        isDebuggerPanelEnabled = true; 
    }

    private void DisableDebuggerPanel()
    {
        DebuggerPanel.gameObject.SetActive(false);
        isDebuggerPanelEnabled = false; 
    }

    public void onMenuPanelButtonPressed()
    {
        if (isMenuPanelEnabled)
        {
            DisableMenuPanel();
        }
        else
        {
            EnableMenuPanel();
        }
    }

    private void EnableMenuPanel()
    {
        MenuPanel.gameObject.SetActive(true);
        isMenuPanelEnabled = true; 
    }

    private void DisableMenuPanel()
    {
        MenuPanel.gameObject.SetActive(false);
        isMenuPanelEnabled = false;

    }

    public IEnumerator notificationsPanelTimer()
    {
        float time = 0f;
        while (time <= 3.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }
        notificationsPanel.gameObject.SetActive(false);
    }
    #endregion
}
