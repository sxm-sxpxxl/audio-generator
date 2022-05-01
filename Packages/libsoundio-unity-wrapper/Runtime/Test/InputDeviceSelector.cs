using SoundIO.SimpleDriver;
using System;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using DeviceType = SoundIO.SimpleDriver.DeviceType;

// Input device selector class
// - Controlls the input device selection UI.
// - Manages SoundIO objects.
// - Provides audio data for the other scripts.

public sealed class InputDeviceSelector : MonoBehaviour
{
    #region Editable attributes

    [SerializeField] Dropdown _deviceList = null;
    [SerializeField] Dropdown _channelList = null;
    [SerializeField] Text _statusText = null;

    #endregion

    #region Public properties

    public int Channel => _channelList.value;
    public int ChannelCount => _stream?.ChannelCount ?? 0;
    public int SampleRate => _stream?.SampleRate ?? 0;

    public float Volume { get; set; } = 1;

    public ReadOnlySpan<float> AudioDataSpan =>
        _audioData.GetSubArray(0, _audioDataFilled).GetReadOnlySpan();

    public NativeSlice<float> AudioDataSlice =>
        new NativeSlice<float>(_audioData, 0, _audioDataFilled);

    #endregion

    #region Internal objects

    InputStream _stream;
    NativeArray<float> _audioData;
    int _audioDataFilled;

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        // Buffer allocation
        _audioData = new NativeArray<float>(4096, Allocator.Persistent);

        // Clear the UI contents.
        _deviceList.ClearOptions();
        _channelList.ClearOptions();
        _statusText.text = "";

        // Null device option
        _deviceList.options.Add(new Dropdown.OptionData() { text = "--" });

        // Device list initialization
        _deviceList.options.AddRange(
            Enumerable.Range(0, DeviceDriver.InputDeviceCount).
                Select(i => DeviceDriver.GetDeviceName(i, DeviceType.Input)).
                Select(name => new Dropdown.OptionData() { text = name }));

        _deviceList.RefreshShownValue();
    }

    void OnDestroy()
    {
        _stream?.Dispose();
        _audioData.Dispose();
    }

    void Update()
    {
        if (_stream == null)
        {
            _audioDataFilled = 0;
            return;
        }

        // Strided copy
        var input = _stream.LastFrameWindow;
        var stride = _stream.ChannelCount;
        var offset = Channel;

        _audioDataFilled = Mathf.Min(input.Length, input.Length / stride);

        for (var i = 0; i < _audioDataFilled; i++)
            _audioData[i] = input[i * stride + offset] * Volume;

        // Status line
        _statusText.text =
            $"Sampling rate: {_stream.SampleRate:n0}Hz / " +
            $"Software Latency: {_stream.Latency * 1000:n2}ms / " +
            $"Amplifier: {Volume:P0}";
    }

    #endregion

    #region UI callback

    public void OnDeviceSelected(int index)
    {
        // Stop and destroy the current stream.
        if (_stream != null)
        {
            _stream.Dispose();
            _stream = null;
        }

        // Reset the UI elements.
        _channelList.ClearOptions();
        _channelList.RefreshShownValue();
        _statusText.text = "";

        // Break if the null device option was selected.
        if (_deviceList.value == 0) return;

        // Open a new stream.
        try
        {
            _stream = DeviceDriver.OpenInputStream(_deviceList.value - 1);
        }
        catch (System.InvalidOperationException e)
        {
            _statusText.text = $"Error: {e.Message}";
            return;
        }

        // Construct the channel list.
        _channelList.options =
            Enumerable.Range(0, _stream.ChannelCount).
            Select(i => $"Channel {i + 1}").
            Select(text => new Dropdown.OptionData() { text = text }).
            ToList();

        _channelList.RefreshShownValue();
    }

    #endregion
}
