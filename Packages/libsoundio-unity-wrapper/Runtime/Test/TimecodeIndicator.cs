using UnityEngine;
using UnityEngine.UI;

// LTC (linear timecode) indicator

public sealed class TimecodeIndicator : MonoBehaviour
{
    [SerializeField] InputDeviceSelector _selector = null;
    [SerializeField] Text _label = null;

    Ltc.TimecodeDecoder _decoder = new Ltc.TimecodeDecoder();

    void Update()
    {
        _decoder.ParseAudioData(_selector.AudioDataSpan);
        var tc = _decoder.LastTimecode;
        var drop = tc.dropFrame ? " (drop frame)" : " (non-drop frame)";
        _label.text = tc.ToString() + drop;
    }
}
