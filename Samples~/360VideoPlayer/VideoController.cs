using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    [Header("References")]

    [SerializeField]
    [Tooltip("Dropdown to select video clips")]
    private TMP_Dropdown dropdown;

    [SerializeField]
    [Tooltip("Video player component")]
    private VideoPlayer videoPlayer;

    [SerializeField]
    [Tooltip("Play/Pause button GameObject")]
    private GameObject buttonPlayOrPause;

    [SerializeField]
    [Tooltip("Play/Pause Button TMP Text")]
    private TMP_Text textPlayOrPause;

    [SerializeField]
    [Tooltip("Slider that controls the video")]
    private Slider slider;

    [SerializeField]
    [Tooltip("Slider text")]
    private TMP_Text sliderText;

    [SerializeField]
    [Tooltip("If checked, the slider will fade off after a few seconds. If unchecked, the slider will remain on.")]
    private bool hideSliderAfterFewSeconds;

    [Header("Settings")]

    [SerializeField]
    [Tooltip("List of video clips for the dropdown")]
    private List<VideoClip> dropdownVideoClips;

    [Header("Events")]

    [SerializeField]
    [Tooltip("Event triggered when the dropdown value changes")]
    private UnityEvent onDropdownChanged;

    [SerializeField]
    [Tooltip("Event triggered when the video plays")]
    private UnityEvent onVideoPlay;

    [SerializeField]
    [Tooltip("Event triggered when the video stops")]
    private UnityEvent onVideoStop;

    private int currentDropdownIndex = -1; // Init on an impossible index
    private bool isDragging;
    private bool videoIsPlaying;
    private bool videoJumpPending;
    private long lastFrameBeforeScrub;

    private void Start()
    {
        // Error checking
        if (dropdown == null) Debug.LogWarning(name + " dropdown == null");
        if (videoPlayer == null) Debug.LogWarning(name + " videoPlayer == null");
        if (dropdownVideoClips == null) Debug.LogWarning(name + " dropdownVideoClips == null");
        if (onDropdownChanged == null) Debug.LogWarning(name + " onDropdownChanged == null");

        if (!videoPlayer.playOnAwake)
        {
            videoPlayer.playOnAwake = true; // Set play on awake for next enable.
            videoPlayer.Play(); // Play video to load first frame.
            VideoStop(); // Stop the video to set correct state and pause frame.
        }
        else
        {
            VideoPlay(); // Play to ensure correct state.
        }
    }

    private void OnEnable()
    {
        if (videoPlayer != null)
        {
            videoPlayer.time = 0;
        }

        slider.value = 0.0f;
        slider.gameObject.SetActive(true);
        if (hideSliderAfterFewSeconds)
            StartCoroutine(HideSliderAfterSeconds());

        UpdateSliderText();
    }

    private void Update()
    {
        UpdateVideoClip();

        if (videoJumpPending)
        {
            // We're trying to jump to a new position, but we're checking to make sure the video player is updated to our new jump frame.
            if (lastFrameBeforeScrub == (long)videoPlayer.time)
                return;

            // If the video player has been updated with desired jump frame, reset these values.
            lastFrameBeforeScrub = long.MinValue;
            videoJumpPending = false;
        }

        if (!isDragging && !videoJumpPending)
        {
            if (videoPlayer.clip.length > 0)
            {
                var progress = (float)videoPlayer.time / (float)videoPlayer.clip.length;
                slider.value = progress;

                UpdateSliderText();
            }
        }
    }

    void UpdateVideoClip()
    {
        if (currentDropdownIndex == dropdown.value)
            return;

        currentDropdownIndex = dropdown.value;

        if (dropdownVideoClips.Count - 1 < currentDropdownIndex)
        {
            Debug.LogWarning("VideoController: Not enough video clips to select this index");
            return;
       }

        videoPlayer.clip = dropdownVideoClips[currentDropdownIndex];

        onDropdownChanged?.Invoke();
    }

    public void OnPointerDown()
    {
        videoJumpPending = true;
        VideoStop();
        VideoJump();
    }

    public void OnRelease()
    {
        isDragging = false;
        VideoPlay();
        VideoJump();
    }

    IEnumerator HideSliderAfterSeconds(float duration = 1f)
    {
        yield return new WaitForSeconds(duration);
        slider.gameObject.SetActive(false);
    }

    public void OnDrag()
    {
        isDragging = true;
        videoJumpPending = true;
    }

    void VideoJump()
    {
        videoJumpPending = true;
        var frame = videoPlayer.clip.length * slider.value;
        lastFrameBeforeScrub = (long)videoPlayer.time;
        videoPlayer.time = (long)frame;
    }

    public void PlayOrPauseVideo()
    {
        if (videoIsPlaying)
        {
            VideoStop();
        }
        else
        {
            VideoPlay();
        }
    }

    public void VideoStop()
    {
        videoIsPlaying = false;
        videoPlayer.Pause();
        textPlayOrPause.text = "Play";
        onVideoStop?.Invoke();
    }

    public void VideoPlay()
    {
        videoIsPlaying = true;
        videoPlayer.Play();
        textPlayOrPause.text = "Pause";
        onVideoPlay?.Invoke();
    }

    void UpdateSliderText()
    {
        // Convert the current time and clip length to TimeSpan objects.
        TimeSpan currentTimeSpan = TimeSpan.FromSeconds(videoPlayer.time);
        TimeSpan clipLengthTimeSpan = TimeSpan.FromSeconds(videoPlayer.clip.length);

        // Format the time values as "m:ss".
        string currentTimeFormatted = string.Format("{0}:{1:00}", currentTimeSpan.Minutes, currentTimeSpan.Seconds);
        string clipLengthFormatted = string.Format("{0}:{1:00}", clipLengthTimeSpan.Minutes, clipLengthTimeSpan.Seconds);

        // Update the slider text with the formatted time values.
        sliderText.text = currentTimeFormatted + " / " + clipLengthFormatted;
    }
}
