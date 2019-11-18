using System.Collections.Generic;
using Windows.UI.Xaml;

namespace SDKTemplate
{
    public class Algorithm
    {
        public List<AlgorithmProperty> AlgorithmProperties { get; set; }
        public string AlgorithmName { get; set; }

        public Algorithm()
        {
        }

        public void AddProperty(AlgorithmProperty additionParams)
        {
            AlgorithmProperties.Add(additionParams);
        }

        public void UpdateProperty(string pName, AlgorithmPropertyType mName, double newValue)
        {
            foreach (var p in AlgorithmProperties)
            {
                if (p.ParameterName == pName)
                {
                    switch (mName)
                    {
                        case AlgorithmPropertyType.currentValue:
                            p.CurrentValue = newValue;
                            break;
                        case AlgorithmPropertyType.maxValue:
                            p.MaxValue = newValue;
                            break;
                        case AlgorithmPropertyType.minValue:
                            p.MinValue = newValue;
                            break;
                        default:
                            p.CurrentValue = newValue;
                            break;
                    }
                }
            }
        }

        public void UpdateCurrentValue(AlgorithmProperty newParam)
        {
            foreach (var p in AlgorithmProperties)
            {
                if (p.ParameterName == newParam.ParameterName)
                {
                    p.CurrentValue = newParam.CurrentDoubleValue;
                }
            }
        }

        public void UpdateCurrentValue(string pName, double newValue)
        {
            foreach (var p in AlgorithmProperties)
            {
                if (p.ParameterName == pName)
                {
                    p.CurrentValue = newValue;
                }
            }
        }

        public void RevertEnable(string paramName)
        {
            foreach (var p in AlgorithmProperties)
            {
                if (p.ParameterName == paramName)
                {
                    if (p.IsComboBoxEnable)
                    {
                        if (p.ComboBoxVisibility == Visibility.Visible)
                        {
                            p.ComboBoxVisibility = Visibility.Collapsed;
                        }
                        else
                        {
                            p.ComboBoxVisibility = Visibility.Visible;
                        }
                    }
                    if (p.IsSliderEnable)
                    {
                        if (p.SliderVisibility == Visibility.Visible)
                        {
                            p.SliderVisibility = Visibility.Collapsed;
                        }
                        else
                        {
                            p.SliderVisibility = Visibility.Visible;
                        }
                    }

                    if (p.DetailsVisibility == Visibility.Visible)
                    {
                        p.DetailsVisibility = Visibility.Collapsed;
                    }
                    else
                    {
                        p.DetailsVisibility = Visibility.Visible;
                    }
                }
            }
        }

        public void ResetEnable()
        {
            foreach (var ap in AlgorithmProperties)
            {
                ap.DetailsVisibility = Visibility.Collapsed;
                ap.SliderVisibility = Visibility.Collapsed;
                ap.ComboBoxVisibility = Visibility.Collapsed;
                ap.isInitialize = false;
            }
        }

        public object FindParamByName(string paramName)
        {
            foreach (var ap in AlgorithmProperties)
            {
                if (ap.ParameterName == paramName)
                {
                    return ap.CurrentValue;
                }
            }
            return null;
        }

        public static List<AlgorithmProperty> GetObjects(Algorithm algorithm)
        {
            var algorithmProperties = new List<AlgorithmProperty>();
            foreach (var algorithmProperty in algorithm.AlgorithmProperties)
            {
                algorithmProperties.Add(algorithmProperty);
            }
            return algorithmProperties;
        }

        public void SetObjects(Algorithm algorithm)
        {
            AlgorithmProperties.Clear();
            foreach (var algorithmProperty in algorithm.AlgorithmProperties)
            {
                AlgorithmProperties.Add(algorithmProperty);
            }
        }
    }
}
