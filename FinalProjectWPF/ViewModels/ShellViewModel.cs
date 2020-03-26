using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using FinalProjectWPF.Models;

namespace FinalProjectWPF.ViewModels
{
    public class ShellViewModel : Screen
    {
        #region Ctor

        public ShellViewModel()
        {
            Lines += $"                     Enter the data to parse below: \r \r";
        }

        #endregion

        #region fields
        private string _inputString;
        private string _lines;
        private int linecounter = 0;
        private bool commandUsed = false;
        private BindableCollection<Line> _lineHistory = new BindableCollection<Line>();
        private BindableCollection<Variable> _variableCollection = new BindableCollection<Variable>();
        private string _internalStatus;
        
        #endregion

        #region Properties

        public string InternalStatus
        {
            get { return _internalStatus; }
            set
            {
                _internalStatus = value;
                NotifyOfPropertyChange(() => InternalStatus);
            }

        }
        public string Lines
        {
            get { return _lines; }
            set
            {
                _lines = value;
                NotifyOfPropertyChange(() => Lines);
            }
        }
        public BindableCollection<Variable> VariableCollection
        {
            get { return _variableCollection; }
            set
            {
                _variableCollection = value;
                NotifyOfPropertyChange(() => VariableCollection);
            }
        }

        public BindableCollection<Line> LineHistory
        {
            get { return _lineHistory; }
            set
            {
                _lineHistory = value;
                NotifyOfPropertyChange(() => LineHistory);
            }
        }

        public string InputString
        {
            get { return _inputString; }
            set
            {
                _inputString = value;
                NotifyOfPropertyChange(() => InputString);

                string tbentry = InputString;

                //Condition to test for Variable or statements
                if (tbentry.Length > 0)
                {

                    if (tbentry[tbentry.Length - 1] == ';')
                    {
                        linecounter++;
                        Lines += $"\r{linecounter}:               {tbentry}";
                        tbentry = tbentry.TrimEnd(';');
                        tbentry = tbentry.TrimStart(' ');

                        if (tbentry == "status")
                        {
                            commandUsed = true;
                            if (VariableCollection.Count != 0)
                            {
                                Lines += "\r________________________________________________";
                                Lines += "\r              Variables = Values;";
                                foreach (var variable in VariableCollection)
                                {
                                    Lines += $"\r               {variable.Name} = {variable.Value};";
                                }

                                InternalStatus = "Variable Status Printed";
                            }
                            else
                            {
                                Lines += $"\r               The variable collection is empty";
                                InternalStatus = "Empty variable collection";
                            }
                        }
                        else if (tbentry.Contains("print") || tbentry.Contains("Print"))
                        {
                            commandUsed = true;
                            if (tbentry.Substring(0, 6).ToLower() != "print ")
                            {
                                InternalStatus = "Invalid print command";
                            }
                            else
                            {
                                tbentry = tbentry.Remove(0, 6);
                                bool varNotFound = true;
                                foreach (Variable variable in VariableCollection)
                                {
                                    if (tbentry.ToLower() == variable.Name.ToLower())
                                    {
                                        varNotFound = false;
                                        linecounter++;
                                        Lines += $"\r{linecounter}:               {variable.Name} = {variable.Value};";
                                    }

                                }
                                if (varNotFound == true)
                                {
                                    Lines += $"\r{linecounter}:               Variable not found";
                                    InternalStatus = "Variable not declared!";
                                    varNotFound = false;
                                }
                            }

                        }


                        //Time to Parse the line hehe
                        var splitcurrent = tbentry.Split(' ');
                        var variablename = "";
                        var EqualitySymbol = "";
                        List<string> expressionsToParse = new List<string>();
                        Variable v = new Variable();
                        Statement st = new Statement();
                        //Use variablename and equalsign to check if for correct syntax declaration
                        for (int i = 0; i < splitcurrent.Length; i++)
                        {
                            if (i == 0)
                            {
                                variablename = splitcurrent[i];// takes the value of the identifier or variable
                            }
                            else if (i == 1)
                            {
                                EqualitySymbol = splitcurrent[i]; // equates with the correct syntax location of the equal symbol
                            }
                            else
                            {
                                expressionsToParse.Add(splitcurrent[i]); // grabs all the other stuff equated to for parsing later on
                            }
                        }

                        //Syntax Checking
                        var charVariableName = variablename.ToCharArray();
                        bool varNameStartsWithNumber = false;
                        bool startsWithOperator = false;
                        bool improperUseOfOperator = false;
                        bool incompleteParenthesis = false;

                        for (int i = 0; i < expressionsToParse.Count; i++)
                        {
                            if (expressionsToParse.First() == "+"
                                || expressionsToParse.First() == "-"
                                || expressionsToParse.First() == "*"
                                || expressionsToParse.First() == "/")
                            {
                                startsWithOperator = true;
                                break;
                            }

                            if (expressionsToParse[i] == "+"
                                || expressionsToParse[i] == "-"
                                || expressionsToParse[i] == "/"
                                || expressionsToParse[i] == "*")
                            {
                                if (i < expressionsToParse.Count - 1)
                                {
                                    if (expressionsToParse[i + 1] == "+"
                                        || expressionsToParse[i + 1] == "-"
                                        || expressionsToParse[i + 1] == "*"
                                        || expressionsToParse[i + 1] == "/")
                                    {
                                        improperUseOfOperator = true;
                                        break;
                                    }
                                }
                                else
                                {
                                    improperUseOfOperator = true;
                                    break;
                                }

                            }

                            bool closedParenthesis = false;
                            if (expressionsToParse[i] == "(")
                            {
                                foreach (var term in expressionsToParse)
                                {
                                    if (term == ")")
                                    {
                                        closedParenthesis = true;
                                        break;
                                    }
                                }
                            }

                            if (closedParenthesis == true)
                            {
                                incompleteParenthesis = false;
                            }
                        }

                        //for the variable syntax check for starting with numbers
                        if (Char.IsDigit(charVariableName.First()))
                        {
                            varNameStartsWithNumber = true;
                        }
                        //Equal symbol checking
                        if (EqualitySymbol != "=" ) 
                        {
                            //print Internal Status Error and reset error checking
                            InternalStatus = "Invalid syntax, No equal signs. Retry Input";
                            if (commandUsed)
                            {
                                InternalStatus = "Command Verified";
                            }
                            varNameStartsWithNumber = false;
                            startsWithOperator = false;
                            improperUseOfOperator = false;
                            incompleteParenthesis = false;
                        }
                        
                        //variablenameChecking for special symbols
                        else if (variablename.Contains('.')
                                 || variablename.Contains(')')
                                 || variablename.Contains('(')
                                 || variablename.Contains('=')
                                 || variablename.Contains('+')
                                 || variablename.Contains('-')
                                 || variablename.Contains('[')
                                 || variablename.Contains(']')
                                 || variablename.Contains('{')
                                 || variablename.Contains('}')
                                 || varNameStartsWithNumber)
                        {
                            //print internal Status Error and reset error checking
                            InternalStatus = "Special Symbols in Variable not allowed, Retry Input";
                            varNameStartsWithNumber = false;
                            startsWithOperator = false;
                            improperUseOfOperator = false;
                            incompleteParenthesis = false;
                        }
                        // for expressions
                        else if (improperUseOfOperator
                                 || incompleteParenthesis
                                 || startsWithOperator)
                        {
                            //print internal Status Error and reset error checking
                            InternalStatus = "Invalid Syntax, Retry Input";
                            varNameStartsWithNumber = false;
                            startsWithOperator = false;
                            improperUseOfOperator = false;
                            incompleteParenthesis = false;
                        }
                        //Proceed To Parsing all entries
                        else
                        {
                            //grabs data before initializing
                            v.Name = variablename;
                            st.ListOfExpressionsToParse = expressionsToParse;
                            st.ListOfVariables = VariableCollection;
                            st.Initialize();
                            //After processing
                            v.Value = st.OutputValue;
                            InternalStatus = st.InternalStatusMessage;

                            bool Variablehistory = false;
                            foreach (var variable in VariableCollection)
                            {
                                if (variable.Name == v.Name)
                                {
                                    variable.Value = v.Value;
                                    Variablehistory = true;
                                }
                            }

                            if (Variablehistory == false)
                            {
                                VariableCollection.Add(v);
                            }

                        }
                        //adding current line to history
                        //Syncs up with Error List
                        var currentLine = new Line() { LineContent = tbentry, LineStatus = InternalStatus };
                        LineHistory.Add(currentLine);


                        //Erase The Current Entry for new entry and moves all to history
                        InputString = "";
                    }

                }


            }
        }

        #endregion

        //Empty because it's much easier to read inputs than use a method to take values and push back to view
        #region Methods

        #endregion
    }
    #region Special Method Controls(Scrolling History)

    #endregion
}
