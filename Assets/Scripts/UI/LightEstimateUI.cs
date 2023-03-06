using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace DemoAR
{
    public class LightEstimateUI : MonoBehaviour
    {
        [SerializeField] LightEstimation _lightEstimation;

        [Tooltip("The UI Text element used to display the estimated ambient intensity in the physical environment.")]
        [SerializeField]
        TMP_Text m_AmbientIntensityText;

        /// <summary>
        /// The UI Text element used to display the estimated ambient intensity value.
        /// </summary>
        public TMP_Text ambientIntensityText
        {
            get { return m_AmbientIntensityText; }
            set { m_AmbientIntensityText = ambientIntensityText; }
        }

        [Tooltip("The UI Text element used to display the estimated ambient color in the physical environment.")]
        [SerializeField]
        TMP_Text m_AmbientColorText;

        /// <summary>
        /// The UI Text element used to display the estimated ambient color in the scene.
        /// </summary>
        public TMP_Text ambientColorText
        {
            get { return m_AmbientColorText; }
            set { m_AmbientColorText = value; }
        }

        [Tooltip("The UI Text element used to display the estimated direction of the main light for the physical environment.")]
        [SerializeField]
        TMP_Text m_MainLightDirectionText;
        public TMP_Text mainLightDirectionText
        {
            get => m_MainLightDirectionText;
            set => m_MainLightDirectionText = value;
        }

        [Tooltip("The UI Text element used to display the estimated intensity in lumens of the main light for the physical environment.")]
        [SerializeField]
        TMP_Text m_MainLightIntensityLumens;
        public TMP_Text mainLightIntensityLumens
        {
            get => m_MainLightIntensityLumens;
            set => m_MainLightIntensityLumens = value;
        }

        [Tooltip("The UI Text element used to display the estimated color of the main light for the physical environment.")]
        [SerializeField]
        TMP_Text m_MainLightColor;
        public TMP_Text mainLightColorText
        {
            get => m_MainLightColor;
            set => m_MainLightColor = value;
        }

        [Tooltip("The UI Text element used to display the estimated spherical harmonics coefficients for the physical environment.")]
        [SerializeField]
        TMP_Text m_SphericalHarmonicsText;
        public TMP_Text ambientSphericalHarmonicsText
        {
            get => m_SphericalHarmonicsText;
            set => m_SphericalHarmonicsText = value;
        }
        StringBuilder m_SphericalHarmonicsStringBuilder = new StringBuilder("");

        const string k_UnavailableText = "Unavailable";


        void Update()
        {
            //Display basic light estimation info
            SetUIValue(_lightEstimation.brightness, ambientIntensityText, "Brightness");
            //Display color temperature or color correction if supported
            if (_lightEstimation.colorTemperature != null)
                SetUIValue(_lightEstimation.colorTemperature, ambientColorText, "Color temperature");
            else if (_lightEstimation.colorCorrection != null)
                SetUIValue(_lightEstimation.colorCorrection, ambientColorText, "Color correction");
            else
                SetUIValue<float>(null, ambientColorText, "Ambient color");

            //Display HDR only light estimation info
            SetUIValue(_lightEstimation.mainLightDirection, mainLightDirectionText, "Main light direction");
            SetUIValue(_lightEstimation.mainLightColor, mainLightColorText, "Main light color");
            SetUIValue(_lightEstimation.mainLightIntensityLumens, mainLightIntensityLumens, "Main light intensity lumen");
            SetSphericalHarmonicsUIValue(_lightEstimation.sphericalHarmonics, ambientSphericalHarmonicsText);
        }


        void SetSphericalHarmonicsUIValue(SphericalHarmonicsL2? maybeAmbientSphericalHarmonics, TMP_Text text)
        {
            if (text != null)
            {
                if (maybeAmbientSphericalHarmonics.HasValue)
                {
                    m_SphericalHarmonicsStringBuilder.Clear();
                    for (int i = 0; i < 3; ++i)
                    {
                        if (i == 0)
                            m_SphericalHarmonicsStringBuilder.Append("R:[");
                        else if (i == 1)
                            m_SphericalHarmonicsStringBuilder.Append("G:[");
                        else
                            m_SphericalHarmonicsStringBuilder.Append("B:[");

                        for (int j = 0; j < 9; ++j)
                        {
                            m_SphericalHarmonicsStringBuilder.Append(j != 8 ? $"{maybeAmbientSphericalHarmonics.Value[i, j]}, " : $"{maybeAmbientSphericalHarmonics.Value[i, j]}]\n");
                        }
                    }
                    text.text = m_SphericalHarmonicsStringBuilder.ToString();
                }
                else
                {
                    text.text = k_UnavailableText;
                }
            }
        }


        void SetUIValue<T>(T? displayValue, TMP_Text text, string valueName) where T : struct
        {
            if (text != null)
                text.text = valueName + ": " + (displayValue.HasValue ? displayValue.Value.ToString() : k_UnavailableText);
        }
    }
}
