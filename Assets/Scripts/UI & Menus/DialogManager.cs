using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogManager : MonoBehaviour {
    //http://digitalnativestudios.com/textmeshpro/docs/rich-text/
    public static DialogManager Instance { get; private set; }

    [Header("Components")]
    [SerializeField] private TMP_Text _dialogText;
    [SerializeField] private TMP_Text _characterNameText;
    [SerializeField] private Image _nextDialogeIcon;

    [Header("Info")]
    [SerializeField, Tooltip("If there is no Font defined by the current dialoge, this will be the font")] private TMP_FontAsset _standardFont;
    [SerializeField, Tooltip("If there is no WriteInterval defined by the current dialoge, this will be the WriteInterval")] private float _standardWriteSpeed;
    [SerializeField] private float testing;

    private Coroutine _currentWrittingCoroutine = null;
    private int _currentDialogeBox;
    private DialogeCustomInformation[] _currentDialoge;
    private float _charInterval;
    private char[] _currentCharArray;
    private short _edittingState; // 0 = not editting, 1 = start editting, 2 = is editting, 3 = end editting
    private int _currentEffectIndex;
    private Dictionary<Vector3, Vector3> _vectorsToAnim = new Dictionary<Vector3, Vector3>();
    private ChangedFormatation _currentFormatationChanges = new ChangedFormatation();

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

    private void SkipText() {
        if (PlayerInputs.interactKeyPressed > 0 && _currentDialoge != null) {
            PlayerInputs.interactKeyPressed = 0;
            if (_currentWrittingCoroutine != null) {//stop dialoge anim
                StopCoroutine(_currentWrittingCoroutine);
                _currentWrittingCoroutine = null;
                _dialogText.text = "";
                _vectorsToAnim.Clear();
                _currentFormatationChanges.ResetValues();
                for (int i = 0; i < _currentCharArray.Length; i++) SetTextConfigs(_currentCharArray[i], i);
                _nextDialogeIcon.enabled = true;
            }
            else {
                _currentDialogeBox++;
                _currentEffectIndex = 0;
                if (_currentDialogeBox >= _currentDialoge.Length) {//end of dialoge
                    _characterNameText.text = "";
                    _dialogText.text = "";
                    _nextDialogeIcon.enabled = false;
                    _currentDialoge = null;
                    _vectorsToAnim.Clear();
                    _currentFormatationChanges.ResetValues();
                }
                else _currentWrittingCoroutine = StartCoroutine(WriteDialoge());//next dialoge box                
            }
        }
    }

    public bool StartDialoge(DialogeCustomInformation[] dialogData) {
        if (_currentWrittingCoroutine == null) {
            _currentDialogeBox = 0;
            _currentDialoge = dialogData;
            _currentWrittingCoroutine = StartCoroutine(WriteDialoge());
            return true;
        }
        else {
            Debug.LogError("Already with text");
            return false;
        }
    }

    private IEnumerator WriteDialoge() {
        _nextDialogeIcon.enabled = false;
        _characterNameText.text = _currentDialoge[_currentDialogeBox].characterName;
        _dialogText.text = "";
        _currentCharArray = _currentDialoge[_currentDialogeBox].dialoge.ToCharArray();
        for (int i = 0; i < _currentCharArray.Length; i++) {
            SetTextConfigs(_currentCharArray[i], i);
            yield return new WaitForSeconds(_charInterval);
        }
        _nextDialogeIcon.enabled = true;
        _currentWrittingCoroutine = null;
    }

    private void SetTextConfigs(char c, int charIndex) {
        if (c == DialogeContent.startAndEndEditChar) _edittingState++;
        else _dialogText.text += c;

        if (_edittingState == 1) {//enables basic editting on section
            if (_currentDialoge[_currentDialogeBox].font.Length > _currentEffectIndex) {
                if (_currentDialoge[_currentDialogeBox].font[_currentEffectIndex] != null) {
                    _dialogText.text += $"<font={_currentDialoge[_currentDialogeBox].font[_currentEffectIndex]}>";
                    _currentFormatationChanges.fontChanged = true;
                }
            }
            if (_currentDialoge[_currentDialogeBox].fontSize.Length > _currentEffectIndex) {
                if (_currentDialoge[_currentDialogeBox].fontSize[_currentEffectIndex] != 0) {
                    _dialogText.text += $"<size={_currentDialoge[_currentDialogeBox].fontSize[_currentEffectIndex]}>";
                    _currentFormatationChanges.fontSizeChanged = true;
                }
            }
            if (_currentDialoge[_currentDialogeBox].writeInterval.Length > _currentEffectIndex) if (_currentDialoge[_currentDialogeBox].writeInterval[_currentEffectIndex] != 0) _charInterval = _currentDialoge[_currentDialogeBox].writeInterval[_currentEffectIndex];

            if (_currentDialoge[_currentDialogeBox].color.Length > _currentEffectIndex) {
                if (_currentDialoge[_currentDialogeBox].color[_currentEffectIndex].a != 0) {
                    _dialogText.text += $"<color=#{ColorUtility.ToHtmlStringRGBA(_currentDialoge[_currentDialogeBox].color[_currentEffectIndex])}>";
                    _currentFormatationChanges.colorChanged = true;
                }
            }
            if (_currentDialoge[_currentDialogeBox].bold.Length > _currentEffectIndex) {
                if (_currentDialoge[_currentDialogeBox].bold[_currentEffectIndex]) {
                    _dialogText.text += "<b>";
                    _currentFormatationChanges.boldChanged = true;
                }
            }
            if (_currentDialoge[_currentDialogeBox].italic.Length > _currentEffectIndex) {
                if (_currentDialoge[_currentDialogeBox].italic[_currentEffectIndex]) {
                    _dialogText.text += "<i>";
                    _currentFormatationChanges.italicChanged = true;
                }
            }            
            _edittingState++;
        }
        else if (_edittingState == 3) {//disables basic editting on section
            if (_currentFormatationChanges.fontChanged) _dialogText.text += "</font>";
            if (_currentFormatationChanges.fontSizeChanged) _dialogText.text += "</size>";
            _charInterval = _standardWriteSpeed;
            if (_currentFormatationChanges.colorChanged) _dialogText.text += "</color>";
            if (_currentFormatationChanges.boldChanged) _dialogText.text += "</b>";
            if (_currentFormatationChanges.italicChanged) _dialogText.text += "</i>";
            _currentEffectIndex++;
            _edittingState = 0;
            _currentFormatationChanges.ResetValues();
        }
        //adds this char to current animation list if is set to animate
        //if (_edittingState > 0 && _edittingState < 3) if (_currentDialoge[_currentDialogeBox].animationType.Length > 0) for (int i = 0; i < 4; i++) _vectorsToAnim.Add(_dialogText.textInfo.meshInfo[charIndex].vertices[i], _dialogText.textInfo.meshInfo[charIndex].vertices[i]);//_charsToAnimate.Add(_dialogText.textInfo.meshInfo[charIndex * 2]);
    }

    private void TextAnimation() {
        if (_vectorsToAnim.Count > 0) {
            switch (_currentDialoge[_currentDialogeBox].animationType[_currentEffectIndex]) {
                case DialogeCustomInformation.AnimationTypes.Wobble:
                    _dialogText.ForceMeshUpdate();
                    Mesh meshWobble = _dialogText.mesh;
                    Vector3[] verticesWobble = meshWobble.vertices;
                    for (int a = 0; a < verticesWobble.Length; a++) if (_vectorsToAnim.ContainsKey(verticesWobble[a])) verticesWobble[a] += WobbleCalc(Time.time + a, _currentDialoge[_currentDialogeBox].animationIntensity[_currentEffectIndex]);
                    _dialogText.mesh.vertices = verticesWobble;
                    _dialogText.canvasRenderer.SetMesh(meshWobble);
                    break;
                case DialogeCustomInformation.AnimationTypes.Shake:
                    _dialogText.ForceMeshUpdate();
                    Mesh meshShake = _dialogText.mesh;
                    Vector3[] verticesShake = meshShake.vertices;
                    Vector3 currentShakeVal = ShakeCalc(0, _currentDialoge[_currentDialogeBox].animationIntensity[_currentEffectIndex]);
                    for (int a = 0; a < verticesShake.Length; a++) {
                        if (_vectorsToAnim.ContainsKey(verticesShake[a])) {
                            if (a % 4 == 0) currentShakeVal = ShakeCalc(Time.time + a, _currentDialoge[_currentDialogeBox].animationIntensity[_currentEffectIndex]);
                            verticesShake[a] += currentShakeVal;
                        }
                    }
                    _dialogText.mesh.vertices = verticesShake;
                    _dialogText.canvasRenderer.SetMesh(meshShake);
                    break;
                case DialogeCustomInformation.AnimationTypes.Wave:
                    //for (int i = 0; i < animChar.mesh.vertices.Length; i++) {
                    //                    animChar.mesh.vertices[i] += new Vector3(0, Mathf.Sin(Time.time * 2 + animChar.mesh.vertices[i].x * .01f) * 10, 0);
                    //                    _dialogText.UpdateGeometry(animChar.mesh, GetMeshIndex(animChar.mesh));
                    //                }
                    //                Vector3[] verts = _dialogText.textInfo.meshInfo[animChar.materialReferenceIndex].vertices;
                    //                for (int i = 0; i < 4; i++) {
                    //                    Vector3 origin = verts[animChar.vertexIndex + i];
                    //                    verts[animChar.vertexIndex + i] = origin + new Vector3(0, Mathf.Sin(Time.time * 2 + origin.x * .01f) * 10, 0);
                    //                }
                    //                for (int j = 0; j < _dialogText.textInfo.meshInfo.Length; j++) {
                    //                    _dialogText.textInfo.meshInfo[j].mesh.vertices = _dialogText.textInfo.meshInfo[j].vertices;
                    //                    _dialogText.UpdateGeometry(_dialogText.textInfo.meshInfo[j].mesh, j);
                    //                }
                    break;
            }
        }

        //        _dialogText.ForceMeshUpdate();
        //Mesh m = _dialogText.mesh;
        //Vector3[] vertices = m.vertices;
        //Vector3 currentShakeVal = ShakeCalc(0, testing);
        //for (int a = 0; a < vertices.Length; a++) {            
        //    if(a % 4 == 0) currentShakeVal = ShakeCalc(Time.time + a, testing);
        //    vertices[a] += currentShakeVal;
        //    //vertices[a] += WobbleCalc(Time.time + a, testing);
        //}

        //_dialogText.mesh.vertices = vertices;
        //_dialogText.canvasRenderer.SetMesh(m);
    }

    private Vector3 WobbleCalc(float time, float intensity) {
        return new Vector3(Mathf.Sin(time * (1.1f + intensity)), Mathf.Sin(time * (.8f + intensity)), 0);
    }
    private Vector3 ShakeCalc(float time, float intensity) {
        return new Vector3(Random.Range(-intensity, intensity), Random.Range(-intensity, intensity), 0);
    }
    private Vector3 WaveCalc(float currentVertexXValue, float intensity) {
        return new Vector3(0, Mathf.Sin(Time.time * 2 + currentVertexXValue * .01f) * intensity, 0);
    }
}
