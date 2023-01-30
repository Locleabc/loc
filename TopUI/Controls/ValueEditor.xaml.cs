using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TopCom.Models;

namespace TopUI.Controls
{
    /// <summary>
    /// Interaction logic for ValueEditor.xaml
    /// </summary>
    public partial class ValueEditor : Window, INotifyPropertyChanged
    {
        #region Properties
        private PositionData data;
        public PositionData Data
        {
            get { return data; }
            set
            {
                data = value;
                OnPropertyChanged("TiltleString");
            }
        }

        private string inputString = "";
        public string InputString
        {
            get { return inputString; }
            set
            {
                inputString = value;
                OnPropertyChanged("InputString");
            }
        }

        public string TiltleString
        {
            get { return Data.PositionName; }
        }

        private double newValue;
        public double NewValue
        {
            get { return newValue; }
            set
            {
                newValue = double.Parse(string.Format("{0:0.000}", value));
                if (Data.MinValue != null)
                {
                    if (newValue < Data.MinValue) newValue = (double)Data.MinValue;
                }
                if (Data.MaxValue != null)
                {
                    if (newValue > Data.MaxValue) newValue = (double)Data.MaxValue;
                }
                OnPropertyChanged("NewValue");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public double DataToChange
        {
            get
            {
                return Data.Value;
            }
            set
            {
                Data.Value = value;
            }
        }
        #endregion

        #region Caculation delegate
        private double number1;
        public double Number1
        {
            get { return number1; }
            set
            {
                number1 = value;
                if (Calculator != null)
                {
                    NewValue = Calculator(Number1, Number2);
                }
                OnPropertyChanged("Number1");
                OnPropertyChanged("NewValue");
            }
        }

        private double number2;
        public double Number2
        {
            get { return number2; }
            set
            {
                number2 = value;
                if (Calculator != null)
                {
                    NewValue = Calculator(Number1, Number2);
                }
                OnPropertyChanged("Number2");
                OnPropertyChanged("NewValue");
            }
        }

        public delegate double Calculation(double num1, double num2);

        private Calculation calculator;
        public Calculation Calculator
        {
            get { return calculator; }
            set
            {
                calculator = value;
                if (Calculator != null)
                {
                    NewValue = Calculator(Number1, Number2);
                }
                OnPropertyChanged("Caculator");
                OnPropertyChanged("NewValue");
            }
        }

        public double Add(double num1, double num2)
        {
            return num1 + num2;
        }

        public double Sub(double num1, double num2)
        {
            return num1 - num2;
        }

        public double Muliple(double num1, double num2)
        {
            return num1 * num2;
        }

        public double Division(double num1, double num2)
        {
            return num1 / num2;
        }
        #endregion

        public ValueEditor(PositionData positionData)
        {
            InitializeComponent();

            this.DataContext = this;
            Data = positionData;

            Number1 = DataToChange;
            Number2 = 0;

            Calculator = new Calculation(Add);
        }

        private void BigButton_Click(object sender, RoutedEventArgs e)
        {
            string tag = (sender as Button).Tag.ToString();

            switch (tag)
            {
                case "ApplyButton":
                    DataToChange = NewValue;
                    DialogResult = true;
                    this.Close();
                    break;
                case "CloseButton":
                    DialogResult = false;
                    this.Close();
                    break;
                case "ClearButton":
                    InputString = "";

                    Number1 = DataToChange;
                    Number2 = 0;

                    Calculator = new Calculation(Add);
                    break;
                case "BackButton":
                    if (InputString.Length > 1)
                    {
                        InputString = InputString.Remove(InputString.Length - 1, 1);

                        GetNumbersFromInputString();
                    }
                    else if (InputString.Length == 1)
                    {
                        InputString = InputString.Remove(InputString.Length - 1, 1);

                        Number1 = 0;
                        Number2 = 0;
                    }
                    else
                    {
                        Number1 = 0;
                        Number2 = 0;
                    }
                    break;
            }
        }

        private void HeaderLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void OperandButton_Click(object sender, RoutedEventArgs e)
        {
            string operand = (sender as Button).Content.ToString();

            if (string.IsNullOrEmpty(InputString))
            {
                InputString += Number1.ToString();
            }
            else if (InputString.Contains("+") || InputString.Contains("–") || InputString.Contains("*") || InputString.Contains("/"))
            {
                if (InputString.EndsWith("+") || InputString.EndsWith("–") || InputString.EndsWith("*") || InputString.EndsWith("/"))
                {
                    InputString = InputString.Remove(InputString.Length - 1, 1);
                }
                else
                {
                    InputString = NewValue.ToString();
                    Number1 = NewValue;
                    Number2 = 0;
                }
            }

            InputString += operand;

            switch (operand)
            {
                case "+":
                    Calculator = new Calculation(Add);
                    break;

                case "–":
                    Calculator = new Calculation(Sub);
                    break;

                case "*":
                    Calculator = new Calculation(Muliple);
                    break;

                case "/":
                    Calculator = new Calculation(Division);
                    break;

                default:
                    break;
            }
        }

        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            GetNumbersFromInputString((sender as Button).Content.ToString());
        }

        private void SignButton_Click(object sender, RoutedEventArgs e)
        {
            if (Calculator == Muliple || Calculator == Division)
            {
                Number2 = 1;
            }
            InputString = NewValue.ToString();
            Number1 = NewValue;
            Number2 = 0;
            Calculator = new Calculation(Add);

            if (InputString.StartsWith("-"))
            {
                InputString = InputString.Remove(0, 1);
            }
            else
            {
                if (string.IsNullOrEmpty(InputString))
                {
                    InputString = "-" + Number1.ToString();
                }
                else
                {
                    InputString = "-" + InputString;
                }
            }

            GetNumbersFromInputString();
        }

        private void GetNumbersFromInputString(string insertingText = null)
        {
            try
            {
                if (InputString.Contains("+") || InputString.Contains("–") || InputString.Contains("*") || InputString.Contains("/"))
                {
                    if (InputString.EndsWith("+") || InputString.EndsWith("–") || InputString.EndsWith("*") || InputString.EndsWith("/"))
                    {
                        Number1 = double.Parse(InputString.Remove(InputString.Length - 1, 1));
                    }

                    if (insertingText != null)
                    {
                        InputString += insertingText;
                    }

                    int operandIndex = 3 + InputString.IndexOf("+") + InputString.IndexOf("–") + InputString.IndexOf("*") + InputString.IndexOf("/");

                    string number2Str = InputString.Substring(operandIndex + 1, InputString.Length - operandIndex - 1);

                    if (number2Str == ".")
                    {
                        Number2 = 0;
                    }
                    else
                    {
                        Number2 = double.Parse(number2Str);
                    }
                }
                else
                {
                    if (insertingText != null)
                    {
                        InputString += insertingText;
                    }

                    if (InputString == ".")
                    {
                        Number1 = 0;
                    }
                    else
                    {
                        Number1 = double.Parse(InputString);
                    }
                    Number2 = 0;
                }
            }
            catch
            {
                MessageBox.Show("Wrong input format");
                DialogResult = false;
                Close();
            }
        }

        private void CurrentValue_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            InputString = data.CurrentValue.ToString();
            NewValue = data.CurrentValue;
        }
    }
}
