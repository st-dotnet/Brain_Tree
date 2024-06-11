using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using VT.Web.Components;

namespace VT.Web.Models
{
    public class FooterViewModel
    {
        public FooterViewModel()
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            AssemblyInfo = new DetailedAssemblyInfo(currentAssembly);
        }

        public DetailedAssemblyInfo AssemblyInfo { get; private set; }
    }
}