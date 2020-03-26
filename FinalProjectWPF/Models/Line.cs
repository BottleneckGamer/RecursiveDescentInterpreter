using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectWPF.Models
{
    public class Line
    {
        #region Fields
        private string _lineContent;
        private static int _lineNumber;
        private string _linestatus;


        #endregion

        #region Properties
        public string LineStatus
        {
            get { return _linestatus; }
            set { _linestatus = value; }
        }

        public Line()
        {
            LineNumber++;
        }
        public int LineNumber
        {
            get { return _lineNumber; }
            private set { _lineNumber = value; }
        }


        public string LineContent
        {
            get { return _lineContent; }
            set { _lineContent = value; }
        }
        #endregion
    }
}
