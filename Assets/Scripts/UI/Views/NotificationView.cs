﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Обрабатывает отображение текста ошибки в процессе измерения/калибровки.
/// </summary>
[DisallowMultipleComponent]
public sealed class NotificationView : MonoBehaviour
{
    private const string MeasurementErrorTitle = "Measurement failed";
    private const string CalibrationErrorTitle = "Calibration failed";
    
    [Header("Dependencies")]
    [SerializeField] private ImpedanceMeasurer impedanceMeasurer;
    [SerializeField] private ChannelsCalibrator channelsCalibrator;

    [Header("Content")]
    [SerializeField] private GameObject notificationContainer;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text contentText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        HideNotification();
        
        impedanceMeasurer.OnImpedanceMeasuringErrorOccurred += OnImpedanceMeasuringErrorOccurred;
        channelsCalibrator.OnCalibrationErrorOccurred += OnCalibrationErrorOccurred;
        closeButton.onClick.AddListener(HideNotification);
    }

    private void OnDestroy()
    {
        impedanceMeasurer.OnImpedanceMeasuringErrorOccurred -= OnImpedanceMeasuringErrorOccurred;
        channelsCalibrator.OnCalibrationErrorOccurred -= OnCalibrationErrorOccurred;
        closeButton.onClick.RemoveListener(HideNotification);
    }

    private void OnImpedanceMeasuringErrorOccurred(string message)
    {
        ShowNotification(MeasurementErrorTitle, message);
    }

    private void OnCalibrationErrorOccurred(string message)
    {
        ShowNotification(CalibrationErrorTitle, message);
    }

    private void ShowNotification(string title, string message)
    {
        titleText.text = title;
        contentText.text = message;
        notificationContainer.SetActive(true);
    }

    private void HideNotification()
    {
        notificationContainer.SetActive(false);
    }
}
