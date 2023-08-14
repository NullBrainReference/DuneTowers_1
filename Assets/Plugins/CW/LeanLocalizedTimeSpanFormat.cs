using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lean.Localization
{
    public class LeanLocalizedTimeSpanFormat : LeanLocalizedText
    {
        [SerializeField]
        public TimeSpan value;
        
        

        public override void UpdateTranslation(LeanTranslation translation)
        {
            // Get the Text component attached to this GameObject
            var text = GetComponent<Text>();

            // Use translation?
            if (translation != null && translation.Data is string)
            {
                text.text = string.Format(LeanTranslation.FormatText((string)translation.Data, text.text, this), value);
            }
            // Use fallback?
            else
            {
                text.text = string.Format(LeanTranslation.FormatText(FallbackText, text.text, this), value);
            }
        }
    }
}