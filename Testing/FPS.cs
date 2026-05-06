using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class FPS : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    private float _lastupdateTime;

    private List<float> _deltaTimes { get; } = new();

    void Update()
    {
        if ((Time.time - _lastupdateTime) > 1)
        {
            _text.text = $"FPS: {(int)(_deltaTimes.Average(delta => 1f / delta))}";
            _deltaTimes.Clear();
            _lastupdateTime = Time.time;
        }

        _deltaTimes.Add(Time.deltaTime);
    }
}
