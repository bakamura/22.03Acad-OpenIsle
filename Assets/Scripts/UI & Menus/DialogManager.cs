using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogManager : MonoBehaviour {
    //http://digitalnativestudios.com/textmeshpro/docs/rich-text/
    public static DialogManager Instance { get; private set; }

    [Header("Components")]
    [SerializeField] private TMP_Text _dialogeText;
    [SerializeField] private TMP_Text _characterNameText;
    [SerializeField] private Image _nextDialogeIcon;

    [Header("Info")]
    [SerializeField, Tooltip("If there is no Font defined by the current dialoge, this will be the font")] private TMP_FontAsset _standardFont;
    [SerializeField, Tooltip("If there is no WriteInterval defined by the current dialoge, this will be the WriteInterval")] private float _standardWriteSpeed;

    private Coroutine _currentWrittingCoroutine = null;
    private int _currentDialogeBox;
    private DialogeFormatationInfo[] _currentDialoge;
    private float _charInterval;
    private char[] _currentCharArray;
    private short _edittingState; // 0 = not editting, 1 = start editting, 2 = is editting, 3 = end editting
    private int _currentEffectIndex = -1;
    private readonly Dictionary<int, DialogeAnimationInfo> _vectorsToAnim = new Dictionary<int, DialogeAnimationInfo>();
    private readonly ChangedFormatation _currentFormatationChanges = new ChangedFormatation();
    private byte _edittingCharCount;//counts how many edit chars had in the current dialoge to animations properly work

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            _charInterval = _standardWriteSpeed;
        }
        else if (Instance != this) Destroy(this);
    }

    private void Update() {
        SkipText();
        TextAnimation();
    }
    //the player inputs to continue dialoge
    private void SkipText() {
        if (PlayerInputs.interactKeyPressed > 0 && _currentDialoge != null) {
            PlayerInputs.interactKeyPressed = 0;
            if (_currentWrittingCoroutine != null) {//stop dialoge anim
                StopCoroutine(_currentWrittingCoroutine);
                _currentWrittingCoroutine = null;
                _edittingCharCount = 0;
                _dialogeText.text = "";
                _vectorsToAnim.Clear();
                _currentFormatationChanges.ResetValues();
                for (int i = 0; i < _currentCharArray.Length; i++) SetTextConfigs(_currentCharArray[i], i);
                _nextDialogeIcon.enabled = true;
            }
            else {
                _currentDialogeBox++;
                _currentEffectIndex = -1;
                if (_currentDialogeBox >= _currentDialoge.Length) {//end of dialoge                    
                    _characterNameText.text = "";
                    _dialogeText.text = "";
                    _nextDialogeIcon.enabled = false;
                    _edittingCharCount = 0;
                    _currentDialoge = null;
                    _vectorsToAnim.Clear();
                    _currentFormatationChanges.ResetValues();
                }
                else {//next dialoge box
                    _nextDialogeIcon.enabled = false;
                    _characterNameText.text = _currentDialoge[_currentDialogeBox].characterName;
                    _dialogeText.text = "";
                    _vectorsToAnim.Clear();
                    _currentFormatationChanges.ResetValues();
                    _currentWrittingCoroutine = StartCoroutine(WriteDialoge());
                }
            }
        }
    }

    public bool StartDialoge(DialogeFormatationInfo[] dialogData) {
        if (_currentWrittingCoroutine == null) {
            _currentDialogeBox = 0;
            _currentDialoge = dialogData;
            _currentWrittingCoroutine = StartCoroutine(WriteDialoge());
            return true;
        }
        else {
            Debug.LogWarning("Dialoge Manager Already with text");
            return false;
        }
    }

    //writes char per char in the dialoge box
    private IEnumerator WriteDialoge() {
        _currentCharArray = _currentDialoge[_currentDialogeBox].dialoge.ToCharArray();
        for (int i = 0; i < _currentCharArray.Length; i++) {
            if (_currentCharArray[i] == DialogeContent.startAndEndEditChar) _edittingCharCount++;
            SetTextConfigs(_currentCharArray[i], i - (_edittingCharCount));
            yield return new WaitForSeconds(_charInterval);
        }
        _nextDialogeIcon.enabled = true;
        _currentWrittingCoroutine = null;
    }

    private void SetTextConfigs(char c, int charIndex) {
        if (c == DialogeContent.startAndEndEditChar) {
            _edittingState++;
            StandardFormatation();
        }
        else {
            _dialogeText.text += c;
            //adds this char verticies to current animation list if is set to animate
            if (_edittingState > 0) AnimationFormatation(charIndex);
        }
    }

    //will do the basic formatation of the text Ex: size, fonStyle, Color, etc
    private void StandardFormatation() {
        if (_edittingState == 1) {//enables basic editting on section
            _currentEffectIndex++;
            if (_currentDialoge[_currentDialogeBox].font.Length > _currentEffectIndex) {
                if (_currentDialoge[_currentDialogeBox].font[_currentEffectIndex] != null) {
                    _dialogeText.text += $"<font={_currentDialoge[_currentDialogeBox].font[_currentEffectIndex]}>";
                    _currentFormatationChanges.fontChanged = true;
                }
            }
            if (_currentDialoge[_currentDialogeBox].fontSize.Length > _currentEffectIndex) {
                if (_currentDialoge[_currentDialogeBox].fontSize[_currentEffectIndex] > 0) {
                    _dialogeText.text += $"<size={_currentDialoge[_currentDialogeBox].fontSize[_currentEffectIndex]}>";
                    _currentFormatationChanges.fontSizeChanged = true;
                }
            }
            if (_currentDialoge[_currentDialogeBox].writeInterval.Length > _currentEffectIndex) if (_currentDialoge[_currentDialogeBox].writeInterval[_currentEffectIndex] != 0) _charInterval = _currentDialoge[_currentDialogeBox].writeInterval[_currentEffectIndex];

            if (_currentDialoge[_currentDialogeBox].color.Length > _currentEffectIndex) {
                if (_currentDialoge[_currentDialogeBox].color[_currentEffectIndex].a > 0) {
                    _dialogeText.text += $"<color=#{ColorUtility.ToHtmlStringRGBA(_currentDialoge[_currentDialogeBox].color[_currentEffectIndex])}>";
                    _currentFormatationChanges.colorChanged = true;
                }
            }
            if (_currentDialoge[_currentDialogeBox].bold.Length > _currentEffectIndex) {
                if (_currentDialoge[_currentDialogeBox].bold[_currentEffectIndex]) {
                    _dialogeText.text += "<b>";
                    _currentFormatationChanges.boldChanged = true;
                }
            }
            if (_currentDialoge[_currentDialogeBox].italic.Length > _currentEffectIndex) {
                if (_currentDialoge[_currentDialogeBox].italic[_currentEffectIndex]) {
                    _dialogeText.text += "<i>";
                    _currentFormatationChanges.italicChanged = true;
                }
            }
            _edittingState++;
        }
        else if (_edittingState == 3) {//disables basic editting on section
            if (_currentFormatationChanges.fontChanged) _dialogeText.text += "</font>";
            if (_currentFormatationChanges.fontSizeChanged) _dialogeText.text += "</size>";
            _charInterval = _standardWriteSpeed;
            if (_currentFormatationChanges.colorChanged) _dialogeText.text += "</color>";
            if (_currentFormatationChanges.boldChanged) _dialogeText.text += "</b>";
            if (_currentFormatationChanges.italicChanged) _dialogeText.text += "</i>";
            _edittingState = 0;
            _currentFormatationChanges.ResetValues();
        }
    }

    //saves each char that needs to be animated in a dictionary to be redrawn in order to perform the animation
    private void AnimationFormatation(int charIndex) {
        if (_currentDialoge[_currentDialogeBox].animationDetails.Length > _currentEffectIndex) {
            if (_currentDialoge[_currentDialogeBox].animationDetails[_currentEffectIndex].animationIntensity > 0) {
                _vectorsToAnim.Add(charIndex, _currentDialoge[_currentDialogeBox].animationDetails[_currentEffectIndex]);//saving the chars of the mesh that will be animated    
            }
        }
    }

    //makes the characters animate
    private void TextAnimation() {
        if (_vectorsToAnim.Count > 0) {
            _dialogeText.ForceMeshUpdate();
            Mesh mesh = _dialogeText.mesh;
            Vector3[] vertices = mesh.vertices;
            for (int a = 0; a < _dialogeText.textInfo.characterCount; a++) {
                if (_vectorsToAnim.ContainsKey(a)) {
                    int index = _dialogeText.textInfo.characterInfo[a].vertexIndex;
                    switch (_vectorsToAnim[a].animationType) {
                        case DialogeAnimationInfo.AnimationTypes.Wobble:
                            vertices[index] += WobbleCalc(1 + a, _vectorsToAnim[a].animationIntensity);
                            vertices[index + 1] += WobbleCalc(Time.time + a, _vectorsToAnim[a].animationIntensity);
                            vertices[index + 2] += WobbleCalc(Time.time + a, _vectorsToAnim[a].animationIntensity);
                            vertices[index + 3] += WobbleCalc(Time.time + a, _vectorsToAnim[a].animationIntensity);
                            break;
                        case DialogeAnimationInfo.AnimationTypes.Shake:
                            Vector3 currentShakeVal = ShakeCalc(_vectorsToAnim[a].animationIntensity);
                            vertices[index] += currentShakeVal;
                            vertices[index + 1] += currentShakeVal;
                            vertices[index + 2] += currentShakeVal;
                            vertices[index + 3] += currentShakeVal;
                            break;
                        case DialogeAnimationInfo.AnimationTypes.Wave:
                            vertices[index] += WaveCalc(vertices[index].x, _vectorsToAnim[a].animationIntensity);
                            vertices[index + 1] += WaveCalc(vertices[index + 1].x, _vectorsToAnim[a].animationIntensity);
                            vertices[index + 2] += WaveCalc(vertices[index + 2].x, _vectorsToAnim[a].animationIntensity);
                            vertices[index + 3] += WaveCalc(vertices[index + 3].x, _vectorsToAnim[a].animationIntensity);
                            break;
                    }
                }
            }
            _dialogeText.mesh.vertices = vertices;
            _dialogeText.canvasRenderer.SetMesh(mesh);
        }
    }

    private Vector3 WobbleCalc(float time, float intensity) {
        return new Vector3(Mathf.Sin(time * (1.1f + intensity)), Mathf.Sin(time * (.8f + intensity)), 0);
    }
    private Vector3 ShakeCalc(float intensity) {
        return new Vector3(Random.Range(-intensity, intensity), Random.Range(-intensity, intensity), 0);
    }
    private Vector3 WaveCalc(float currentVertexXValue, float intensity) {
        return new Vector3(0, Mathf.Sin(Time.time * 2 + currentVertexXValue * .01f) * intensity, 0);
    }
}
