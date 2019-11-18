using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;

using OpenCvSharp;

namespace SDKTemplate
{
    public class AlgorithmProperty : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Name of the parameter.
        private string parameterName;
        public string ParameterName
        {
            get { return parameterName; }
            set
            {
                parameterName = value;
                NotifyPropertyChanged("ParameterName");
            }
        }

        // Description of the parameter.
        private string description;
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                NotifyPropertyChanged("Description");
            }
        }

        // Value setting panel visibility
        private Visibility sliderVisibility;
        public Visibility SliderVisibility
        {
            get { return sliderVisibility; }
            set
            {
                sliderVisibility = value;
                NotifyPropertyChanged("SliderVisibility");
            }
        }

        // Value setting panel visibility
        private Visibility comboBoxVisibility;
        public Visibility ComboBoxVisibility
        {
            get { return comboBoxVisibility; }
            set
            {
                comboBoxVisibility = value;
                NotifyPropertyChanged("ComboBoxVisibility");
            }
        }

        // Value setting panel visibility
        private Visibility detailsVisibility;
        public Visibility DetailsVisibility
        {
            get { return detailsVisibility; }
            set
            {
                detailsVisibility = value;
                NotifyPropertyChanged("DetailsVisibility");
            }
        }

        // Current value of the parameter
        private double currentValue;
        public object CurrentValue
        {
            get
            {
                if (ParamType == typeof(int))
                {
                    return (int)currentValue;
                }

                if (ParamType == typeof(double))
                {
                    return currentValue;
                }

                if (ParamType == typeof(OpenCvSharp.Size))
                {
                    var res = new OpenCvSharp.Size((int)currentValue, (int)currentValue);
                    return res;
                }

                if (ParamType == typeof(Scalar))
                {
                    return (Scalar)currentValue;
                }

                if (ParamType == typeof(OpenCvSharp.Point))
                {
                    var res = new OpenCvSharp.Point(currentValue, currentValue);
                    return res;
                }

                if (ParamType?.BaseType == typeof(Enum))
                {
                    return ParamList[CurrentIntValue];
                }

                return currentValue;
            }
            set
            {
                currentValue = (double)value;
                CurrentDoubleValue = (double)value;
                CurrentStringValue = CurrentValue.ToString();

                if (ParamType?.BaseType == typeof(Enum))
                {
                    CurrentIntValue = Convert.ToInt32(value);
                }
                else
                {
                    CurrentIntValue = 0;
                }

                NotifyPropertyChanged("CurrentValue");
            }
        }

        private double currentDoubleValue;
        public double CurrentDoubleValue
        {
            get
            {
                return (double)currentDoubleValue;
            }
            set
            {
                currentDoubleValue = value;
                NotifyPropertyChanged("CurrentDoubleValue");
            }
        }

        private string currentStringValue;
        public string CurrentStringValue
        {
            set
            {
                currentStringValue = "Current Value = " + value.ToString();
                NotifyPropertyChanged("CurrentStringValue");
            }
            get
            {
                return currentStringValue;
            }
        }

        private int currentIntValue;
        public int CurrentIntValue
        {
            get
            {
                return currentIntValue;
            }
            set
            {
                currentIntValue = value;
                NotifyPropertyChanged("CurrentIntValue");
            }
        }

        // Maximum value of the parameter
        private double maxValue;
        public double MaxValue
        {
            get { return maxValue; }
            set
            {
                maxValue = value;
                NotifyPropertyChanged("MaxValue");
            }
        }

        // Minimum value of the parameter
        private double minValue;
        public double MinValue
        {
            get { return minValue; }
            set
            {
                minValue = value;
                NotifyPropertyChanged("MinValue");
            }
        }

        private bool isSliderEnable;
        public bool IsSliderEnable
        {
            get { return isSliderEnable; }
            set
            {
                isSliderEnable = value;
                NotifyPropertyChanged("IsSliderEnable");
            }
        }

        private bool isComboBoxEnable;
        public bool IsComboBoxEnable
        {
            get { return isComboBoxEnable; }
            set
            {
                isComboBoxEnable = value;
                NotifyPropertyChanged("IsComboBoxEnable");
            }
        }

        private string tag;
        public string Tag
        {
            get { return tag; }
            set
            {
                tag = value;
                NotifyPropertyChanged("Tag");
            }
        }

        private List<object> comboList;
        public List<object> ComboList
        {
            get { return comboList; }
            set
            {
                comboList = value;
                NotifyPropertyChanged("ComboList");
            }
        }

        private Type paramType;
        public Type ParamType
        {
            get { return paramType; }
            set
            {
                paramType = value;
                NotifyPropertyChanged("ParamType");
            }
        }

        private List<object> paramList;
        public List<object> ParamList
        {
            get { return paramList; }
            set
            {
                paramList = value;
                NotifyPropertyChanged("ParamList");
            }
        }
        // Converter

        // enum val
        public List<string> Selections;
        public int selectIndex;
        public bool isInitialize;

        public AlgorithmProperty(int index, Type type, string name, string description = "The default property description.", double max = 255, double min = 0, double cur = 0)
        {
            ParameterName = name;
            Description = description;
            MaxValue = max;
            MinValue = min;
            CurrentValue = cur > max ? max : cur < min ? min : cur;
            ParamType = type;

            if (type.BaseType != typeof(Enum))
            {
                ParamList = null;
                IsComboBoxEnable = false;
                isSliderEnable = true;
            }
            else
            {
                var _enumval = Enum.GetValues(type).Cast<object>();
                ParamList = _enumval.ToList();
                IsComboBoxEnable = true;
                isSliderEnable = false;
            }

            selectIndex = index;
            SliderVisibility = Visibility.Collapsed;
            ComboBoxVisibility = Visibility.Collapsed;
            DetailsVisibility = Visibility.Collapsed;
            isInitialize = false;
            Tag = name;
        }
        public AlgorithmProperty(string name, List<string> selections, string description = "The default property description.")
        {
            parameterName = name;
            this.description = description;
            Selections = selections;
            selectIndex = 0;
        }

        public void updateSelectIndex(int idx)
        {
            selectIndex = idx;
        }

        public void resetCurrentValue()
        {
            currentValue = (maxValue + minValue) / 2;
        }
    }
}
