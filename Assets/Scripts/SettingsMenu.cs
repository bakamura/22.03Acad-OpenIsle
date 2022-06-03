using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour {

    [Header("Audio")]
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private SliderData _masterVol;
    [SerializeField] private SliderData _musicVol;
    [SerializeField] private SliderData _sfxVol;
    private float _currentSliderValue;

    public void SetVolume(int volToChange) {
        SliderData slider = GetSliderVol(volToChange);
        _mixer.SetFloat(slider.name, Mathf.Log10(_currentSliderValue) * 20); // Slider lowest must be 0.001 !!!
        GetManagerVol(volToChange) = Mathf.Round(slider.slider.value * 1000f) / 1000f;
        slider.valueText.text = (slider.slider.value * 100f).ToString("F0");
    }

    private ref float GetManagerVol(int volVarID) {
        switch (volVarID) {
            case 0: return ref GameManager.MasterVol;
            case 1: return ref GameManager.MusicVol;
            case 2: return ref GameManager.SfxVol;
            default:
                Debug.Log("Error Fetching Volume Var From Manager, used MasterVol Instead");
                return ref GameManager.MasterVol;
        }
    }

    private SliderData GetSliderVol(int volVarID) {
        switch (volVarID) {
            case 0: return _masterVol;
            case 1: return _musicVol;
            case 2: return _sfxVol;
            default:
                Debug.Log("Error Fetching Volume Slider Value, used MasterVolSlider Instead");
                return _masterVol;
        }
    }
}
