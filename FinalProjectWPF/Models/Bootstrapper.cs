using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using FinalProjectWPF.ViewModels;

namespace FinalProjectWPF.Models
{
    public class Bootstrapper : BootstrapperBase
    {
        #region Ctor

        public Bootstrapper()
        {
            Initialize();
        }

        #endregion

        #region Method

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }

        #endregion
    }
}
