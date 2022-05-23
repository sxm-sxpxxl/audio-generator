﻿using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Обрабатывает отображение текста ошибки в процессе измерения/калибровки.
/// </summary>
[DisallowMultipleComponent]
public sealed class ErrorLoggerView : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private ImpedanceMeasurer impedanceMeasurer;
    [SerializeField] private ChannelsCalibrator channelsCalibrator;
    
    [Header("Related views")]
    [SerializeField] private Text errorMessageText;

    private void Awake()
    {
        impedanceMeasurer.OnImpedanceMeasuringErrorOccurred += LogErrorWith;
        impedanceMeasurer.OnImpedanceMeasuringFinished += ResetLogError;
        channelsCalibrator.OnCalibrationErrorOccurred += LogErrorWith;
        channelsCalibrator.OnCalibrationFinished += ResetLogError;
        
        LogErrorWith("-");
    }

    private void OnDestroy()
    {
        impedanceMeasurer.OnImpedanceMeasuringErrorOccurred -= LogErrorWith;
        impedanceMeasurer.OnImpedanceMeasuringFinished -= ResetLogError;
        channelsCalibrator.OnCalibrationErrorOccurred -= LogErrorWith;
        channelsCalibrator.OnCalibrationFinished -= ResetLogError;
    }

    private void LogErrorWith(string message)
    {
        errorMessageText.text = message;
    }

    private void ResetLogError(float _) => ResetLogError();
    
    private void ResetLogError()
    {
        errorMessageText.text = "-";
    }
}