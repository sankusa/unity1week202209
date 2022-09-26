using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Sankusa.unity1week202209.Domain {
    [Serializable]
    public class SquareStructureStorage
    {
        [SerializeField] private List<SquareStructure> squareStructures = new List<SquareStructure>();
        public List<SquareStructure> SquareStructures => squareStructures;
    }
}