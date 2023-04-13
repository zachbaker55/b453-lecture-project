using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BillionGame {
    [System.Serializable]
    public class RadialBar {

        //Using shader from https://github.com/Nrjwolf/unity-shader-sprite-radial-fill
        [SerializeField] private Shader _shader;
        public Shader Shader {
            get { return _shader; }
        }
        [SerializeField] private Color _xpColor;
        public Color XPColor {
            get { return _xpColor; }
        }
    }
}
