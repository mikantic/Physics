using System;
using Core.Numerics;
using Physics.Numerics;
using UnityEngine;

namespace Physics
{
    [Serializable]
    public struct Force
    {
        [SerializeField] private Vector3 value;
        public readonly Vector3 Value => value;

        [SerializeField] private float magnitude;
        public readonly float Magnitude => magnitude;
    }
}