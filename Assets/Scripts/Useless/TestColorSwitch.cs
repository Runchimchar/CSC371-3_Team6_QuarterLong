using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestColorSwitch : MonoBehaviour
{
    [SerializeField] private List<Color> _colors;
    [SerializeField] private float _switchDelay = 0.5f;
    private Renderer _renderer;
    private int _index = 0;
    private MaterialPropertyBlock _mpb;
    private static int _colorID = Shader.PropertyToID("_BaseColor");

    public void Start()
    {
        _renderer = GetComponent<Renderer>();
        _mpb = new MaterialPropertyBlock();
        _mpb.SetColor(_colorID, _colors[_index]);
        _renderer.SetPropertyBlock(_mpb);
        StartCoroutine(switchColor(_switchDelay));
    }

    private IEnumerator switchColor(float delay)
    {
        yield return new WaitForSeconds(delay);
        _index = (_index + 1) % _colors.Count;
        _mpb.SetColor(_colorID, _colors[_index]);
        _renderer.SetPropertyBlock(_mpb);
        StartCoroutine(switchColor(_switchDelay));
    }
}
