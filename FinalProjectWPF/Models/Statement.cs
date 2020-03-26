using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Caliburn.Micro;

namespace FinalProjectWPF.Models
{
    public class Statement
    {
        #region Fields
        private string _inputChar;
        public int counter = 0;
        public List<string> ListOfExpressionsToParse = new List<string>();
        public BindableCollection<Variable> ListOfVariables;
        private double _outputValue;
        private bool presentVariable = false;
        private bool internalStatus = true;
        private bool hasMoreDigits = true;
        private string _internalStatusMessage;

        #endregion
        
        #region Ctor

        public Statement()
        {
        }

        #endregion
        
        #region Properties
        public string InternalStatusMessage

        {
            get { return _internalStatusMessage; }
            set
            {
                _internalStatusMessage = value;

            }
        }
        public double OutputValue
        {
            get { return _outputValue; }
            set { _outputValue = value; }
        }
        public string InputChar
        {
            get { return _inputChar; }
            set { _inputChar = value; }
        }
        #endregion

        #region Methods

        public void Initialize()
        {
            Expression();
        }

        private double Expression()
        {
            double t = Term();
            while (true)
            {
                if (counter >= ListOfExpressionsToParse.Count)
                {
                    OutputValue = t;
                    return t;
                }

                switch (ListOfExpressionsToParse[counter])
                {
                    case "+":
                        counter++;
                        t += Term();
                        break;
                    case "-":
                        counter++;
                        t -= Term();
                        break;
                    default:
                        OutputValue = t;
                        return t;
                }
            }
        }

        private double Term()
        {
            double f = Factor();
            while (true)
            {
                if (counter >= ListOfExpressionsToParse.Count)
                {
                    return f;
                }

                switch (ListOfExpressionsToParse[counter])
                {
                    case "*":
                        counter++;
                        f *= Factor();
                        break;
                    case "/":
                        counter++;
                        f /= Factor();
                        break;
                    default:
                        return f;
                }
            }
        }

        private double Factor()
        {
            double num = 0, negate = 1.0;

            //takes the first char and checks if it is a '-' for negatives
            var signCheck = ListOfExpressionsToParse[counter].Contains('-');
            if (signCheck)
            {
                negate *= -1.0;
            }

            //Check for Identifiers
            if (ListOfExpressionsToParse[counter].All(char.IsDigit))
            {
                num = double.Parse(ListOfExpressionsToParse[counter]);
            }
            else if (ListOfExpressionsToParse[counter].All(char.IsDigit) == false)
            {
                hasMoreDigits = ListOfExpressionsToParse[counter].Remove(0, 1).All(char.IsDigit);
                if (hasMoreDigits || ListOfExpressionsToParse[counter].Contains('.'))
                {
                    if (negate == -1.0 && ListOfExpressionsToParse[counter].Contains('.'))
                    {
//                        if (ListOfExpressionsToParse[counter].Contains(".") ||
//                            ListOfExpressionsToParse[counter].Contains("/") ||
//                            ListOfExpressionsToParse[counter].Contains("*") ||
//                            ListOfExpressionsToParse[counter].Contains("+") ||
//                            ListOfExpressionsToParse[counter].Contains("-"))
//                        {
//                            InternalStatusMessage = "Invalid Syntax. Use Spaces to separate operations";
//                        }
                        num = double.Parse(ListOfExpressionsToParse[counter].Remove(0, 1));
                    }
                    else if (ListOfExpressionsToParse[counter].Contains('.'))
                    {
//                        if (ListOfExpressionsToParse[counter].Contains(".") ||
//                            ListOfExpressionsToParse[counter].Contains("/") ||
//                            ListOfExpressionsToParse[counter].Contains("*") ||
//                            ListOfExpressionsToParse[counter].Contains("+") ||
//                            ListOfExpressionsToParse[counter].Contains("-"))
//                        {
//                            InternalStatusMessage = "Invalid Syntax. Use Spaces to separate operations";
//                        }
//                        else
//                        {
//                        }

                        num = double.Parse(ListOfExpressionsToParse[counter]);
                    }
                    else
                    {
//                        if (ListOfExpressionsToParse[counter].Contains(".") ||
//                            ListOfExpressionsToParse[counter].Contains("/") ||
//                            ListOfExpressionsToParse[counter].Contains("*") ||
//                            ListOfExpressionsToParse[counter].Contains("+") ||
//                            ListOfExpressionsToParse[counter].Contains("-"))
//                        {
//                            InternalStatusMessage = "Invalid Syntax. Use Spaces to separate operations";
//                        }
                        if (ListOfExpressionsToParse[counter].Remove(0, 1) == "")
                        {
                            num = 0;
                        }
                        else
                        {
                            num = double.Parse(ListOfExpressionsToParse[counter].Remove(0, 1));
                        }
                    }
                }

                else
                {
                    //looks for negative variables
                    if (ListOfExpressionsToParse[counter].First() == '-')
                    {
                        foreach (Variable v in ListOfVariables)
                        {
                            if (ListOfExpressionsToParse[counter].Remove(0, 1) == v.Name)
                            {
                                num = v.Value;
                                presentVariable = true;
                                InternalStatusMessage += $"Operation done with variable found ";
                            }
                            //For no pre variable declaration cases
                            if (presentVariable == false)
                            {
                                internalStatus = false;
                                if (ListOfExpressionsToParse[counter].First() == '-')
                                {
                                    //print error message for negative case
                                    InternalStatusMessage = $"There was no negative variable pre declared! ";

                                }
                                else
                                {
                                    //print for positive case
                                    InternalStatusMessage = $"There was no variable pre declared! ";
                                }
                            }
                        }
                    }
                }
                //Natural Check for present Variables
                foreach (Variable v in ListOfVariables)
                {
                    if (ListOfExpressionsToParse[counter] == v.Name)
                    {
                        num = v.Value;
                        presentVariable = true;
                    }
                    //For no pre variable declaration cases
                    if (presentVariable == false)
                    {
                        internalStatus = false;
                        if (ListOfExpressionsToParse[counter].First() == '-')
                        {
                            //print error message for negative case
                            InternalStatusMessage = $"There was no negative variable pre declared! ";
                        }
                        else
                        {
                            //print for positive case
                            InternalStatusMessage = $"There was no variable pre declared! ";
                        }
                    }
                }
            }
            if (ListOfExpressionsToParse.Count == 1 && hasMoreDigits == false)
            {
                InternalStatusMessage = "Invalid input, Initializing variable to 0";
            }

            bool hitParenthesis = false;
            if (ListOfExpressionsToParse[counter] == "(" || ListOfExpressionsToParse[counter] == "-(")
            {
                hitParenthesis = true;
                counter++;
                num = Expression();
            }

            if (hitParenthesis == false)
            {
                counter++;
            }

            if (internalStatus == true)
            {
                //No errors in syntax 
                InternalStatusMessage += $"\r ProcessFinished! ";
            }

            hasMoreDigits = false;
            return negate * num;
        }
        #endregion

    }
}
