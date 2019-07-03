using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowDebugInfo : MonoBehaviour
{
    public Transform CameraTransform;
    private TMP_Text text;

    private void Start() {
        text = GetComponent<TMP_Text>();
    }

    private void Update() {
        text.SetText($"Pos: {Mathf.Round(CameraTransform.position.x)}, {Mathf.Round(CameraTransform.position.y)}, {Mathf.Round(CameraTransform.position.z)}\nChunk: {Mathf.Floor(CameraTransform.position.x/64)},{Mathf.Floor(CameraTransform.position.z/64)}\nFPS: {Mathf.Round(1/Time.deltaTime)}");
    }
}
