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

    [Tooltip("Dropdown to select video clips")]
    [SerializeField]
    private TMP_Dropdown dropdown;

    [Tooltip("Video player component")]
    [SerializeField]
    private VideoPlayer videoPlayer;

    [Tooltip("Play/Pause button GameObject")]
    [SerializeField]
    private GameObject buttonPlayOrPause;

    [Tooltip("Play/Pause Button TMP Text")]
    [SerializeField]
    private TMP_Text textPlayOrPause;

    [Tooltip("Slider that controls the video")]
    [SerializeField]
    private Slider slider;

    [Tooltip("Slider text")]
    [SerializeField]
    private TMP_Text sliderText;

    [Tooltip("If checked, the slider will fade off after a few seconds. If unchecked, the slider will remain on.")]
    [SerializeField]
    private bool hideSliderAfterFewSeconds;

    [Header("Settings")]
    
    [Tooltip("List of video clips for the dropdown")]
    [SerializeField]
    private List<VideoClip> dropdownVideoClips;

    [Header("Events")]

    [Tooltip("Event triggered when the dropdown value changes")]
    [SerializeField]
    private UnityEvent onDropdownChanged;

    [Tooltip("Event triggered when the video plays")]
    [SerializeField]
    private UnityEvent onVideoPlay;

    [Tooltip("Event triggered when the video stops")]
    [SerializeField]
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

        // Sets up player to play correctly
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

        // Check whether the video is playing normally
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

    private void UpdateVideoClip()
    {
        // Do nothing if user selects the same video
        if (currentDropdownIndex == dropdown.value)
            return;

        currentDropdownIndex = dropdown.value;

        // Error handling
        if (dropdownVideoClips.Count - 1 < currentDropdownIndex)
        {
            Debug.LogWarning("VideoController: Not enough video clips to select this index");
            return;
        }

        // Set the video to the selected clip
        videoPlayer.clip = dropdownVideoClips[currentDropdownIndex];

        onDropdownChanged?.Invoke(); // Activate the function if it exists
    }

    /// <summary>
    ///     To be used as part of an interaction event.
    ///     Enables the 'jumping' state of the slider, allowing the user to scrub the timeline.
    /// </summary>
    public void OnPointerDown()
    {
        videoJumpPending = true;
        VideoStop();
        VideoJump();
    }

    /// <summary>
    ///     To be used as part of an interaction event, in conjunction with the OnPointerDown() function.
    ///     Disables the 'jumping' state of the slider.
    /// </summary>
    public void OnRelease()
    {
        isDragging = false;
        VideoPlay();
        VideoJump();
    }

    private IEnumerator HideSliderAfterSeconds(float duration = 1f)
    {
        yield return new WaitForSeconds(duration);
        slider.gameObject.SetActive(false);
    }

    /// <summary>
    ///     To be used as part of an interaction event.
    /// </summary>
    public void OnDrag()
    {
        isDragging = true;
        videoJumpPending = true;
    }

    private void VideoJump()
    {
        videoJumpPending = true;
        var frame = videoPlayer.clip.length * slider.value;
        lastFrameBeforeScrub = (long)videoPlayer.time;
        videoPlayer.time = (long)frame;
    }

    /// <summary>
    ///     To be used as part of an interaction event.
    ///     Toggles the playback state of the video player.
    /// </summary>
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

    /// <summary>
    ///     To be used as part of an interaction event.
    /// </summary>
    public void VideoStop()
    {
        videoIsPlaying = false;
        videoPlayer.Pause();
        textPlayOrPause.text = "Play";
        onVideoStop?.Invoke();
    }

    /// <summary>
    ///     To be used as part of an interaction event.
    /// </summary>
    public void VideoPlay()
    {
        videoIsPlaying = true;
        videoPlayer.Play();
        textPlayOrPause.text = "Pause";
        onVideoPlay?.Invoke();
    }

    private void UpdateSliderText()
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
