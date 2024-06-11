using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace VT.Web.Components
{
    public class DetailedAssemblyInfo
    {
        #region Private Variables

        private string m_Company;
        private string m_Copyright;
        private string m_Description;
        private readonly string m_Path;
        private string m_Product;
        private string m_Title;
        private string m_Trademark;
        private string m_Version;
        private string m_InformationalVersion;
        private long m_FileSize;
        private DateTime m_CreateDate;
        private DateTime m_LastModifiedDate;

        #endregion

        #region Accessors

        public string Company
        {
            get { return m_Company; }
        }

        public string Copyright
        {
            get { return m_Copyright; }
        }

        public string Description
        {
            get { return m_Description; }
        }

        public string Path
        {
            get { return m_Path; }
        }

        public string Product
        {
            get { return m_Product; }
        }

        public string Title
        {
            get { return m_Title; }
        }

        public string Trademark
        {
            get { return m_Trademark; }
        }

        public string Version
        {
            get { return m_Version; }
        }

        public string InformationalVersion
        {
            get { return m_InformationalVersion; }
        }

        public long FileSize
        {
            get { return m_FileSize; }
        }

        public DateTime CreateDate
        {
            get { return m_CreateDate; }
        }

        public DateTime LastModifiedDate
        {
            get { return m_LastModifiedDate; }
        }

        #endregion

        #region Constructor

        public DetailedAssemblyInfo(string path)
        {
            m_Path = path;

            string assemblyFileName = System.IO.Path.GetFileNameWithoutExtension(path);

            if (assemblyFileName == null)
                return;

            var assembly = Assembly.Load(assemblyFileName);

            Setup(assembly);
        }

        public DetailedAssemblyInfo(Assembly assembly)
        {
            Setup(assembly);
        }

        #endregion

        #region Helper Methods

        private void Setup(Assembly assembly)
        {
            var fi = new FileInfo(assembly.Location);
            m_CreateDate = fi.CreationTime.ToUniversalTime();
            m_LastModifiedDate = fi.LastWriteTime.ToUniversalTime();
            m_FileSize = fi.Length;

            var assemblyName = assembly.GetName();

            m_Version = assemblyName.Version.ToString();

            foreach (Attribute attribute in Attribute.GetCustomAttributes(assembly))
            {
                switch (attribute.GetType().ToString())
                {
                    case "System.Reflection.AssemblyTitleAttribute":
                        {
                            var attr = (AssemblyTitleAttribute)attribute;
                            m_Title = attr.Title;
                            break;
                        }

                    case "System.Reflection.AssemblyDescriptionAttribute":
                        {
                            var attr = (AssemblyDescriptionAttribute)attribute;
                            m_Description = attr.Description;
                            break;
                        }

                    case "System.Reflection.AssemblyCompanyAttribute":
                        {
                            var attr = (AssemblyCompanyAttribute)attribute;
                            m_Company = attr.Company;
                            break;
                        }

                    case "System.Reflection.AssemblyProductAttribute":
                        {
                            var attr = (AssemblyProductAttribute)attribute;
                            m_Product = attr.Product;
                            break;
                        }

                    case "System.Reflection.AssemblyCopyrightAttribute":
                        {
                            var attr = (AssemblyCopyrightAttribute)attribute;
                            m_Copyright = attr.Copyright;
                            break;
                        }

                    case "System.Reflection.AssemblyTrademarkAttribute":
                        {
                            var attr = (AssemblyTrademarkAttribute)attribute;
                            m_Trademark = attr.Trademark;
                            break;
                        }

                    case "System.Reflection.AssemblyInformationalVersionAttribute":
                        {
                            var attr = (AssemblyInformationalVersionAttribute)attribute;
                            m_InformationalVersion = attr.InformationalVersion;
                            break;
                        }
                }
            }
        }

        #endregion
    }
}