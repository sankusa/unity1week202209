using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

namespace Sankusa.unity1week202209.View {
    public class InputFieldValidator : MonoBehaviour
    {
        [SerializeField] private InputField inputField;
        private string pattern = "^[0-9a-zA-Zぁ-んァ-ヶｱ-ﾝﾞﾟ一-龠ー]*$";

        void Start() {
            inputField.onValidateInput += ValidateInput;
        }

        public char ValidateInput(string text, int charIndex, char addChar) {
            if(Regex.IsMatch(addChar.ToString(), pattern)) {
                return addChar;
            } else {
                return '\0';
            }
        }
    }
}