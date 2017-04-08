using SharpSvn;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;

namespace WebApp.Models
{
    public sealed class SvnDirectoryType
    {
        private readonly int _value;
        private readonly string _name;
        private readonly string _className;

        public static readonly SvnDirectoryType Dev = new SvnDirectoryType(1, "Development", "danger");
        public static readonly SvnDirectoryType Stable = new SvnDirectoryType(2, "Stable", "success");
        public static readonly SvnDirectoryType Old = new SvnDirectoryType(3, "Old", "warning");
        public static readonly SvnDirectoryType Project = new SvnDirectoryType(4, "Project", "default");
        public static readonly SvnDirectoryType Unknown = new SvnDirectoryType(5, "Unknown", "info");

        private SvnDirectoryType(int value, string name, string className)
        {
            this._value = value;
            this._name = name;
            this._className = className;
        }

        public override string ToString()
        {
            return _name;
        }
    }

    public class SvnDirectory
    {
        private string _name = string.Empty;
        private SvnDirectoryType _type = null;
        private Configuration _webConfig = null;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                _webConfig = WebConfigurationManager.OpenWebConfiguration("/" + _name);
            }
        }

        public SvnDirectoryType Type
        {
            get
            {
                if (_type == null)
                {
                    int version = 0;
                    string[] nameSplited = Name.Split('_');
                    foreach (string part in nameSplited)
                    {
                        if (int.TryParse(part, out version))
                            break;
                    }

                    if (!Name.StartsWith("buyer_demo", StringComparison.InvariantCultureIgnoreCase))
                        _type = SvnDirectoryType.Project;
                    else if (!Name.StartsWith("buyer_demo_V8", StringComparison.InvariantCultureIgnoreCase))
                        _type = SvnDirectoryType.Old;
                    else if (version != 0 && (version % 2) == 0)
                        _type = SvnDirectoryType.Stable;
                    else if (version != 0 && (version % 2) != 0)
                        _type = SvnDirectoryType.Dev;
                    else if (Name.StartsWith("buyer_demo_V8_dev", StringComparison.InvariantCultureIgnoreCase))
                        _type = SvnDirectoryType.Dev;
                    else if (Name.StartsWith("buyer_demo_V8", StringComparison.InvariantCultureIgnoreCase))
                        _type = SvnDirectoryType.Stable;
                    else
                        _type = SvnDirectoryType.Unknown;
                }

                return _type;
            }
        }

        public bool HasWebConfig
        {
            get { return _webConfig != null ? _webConfig.HasFile : false; }
        }
        public string Database
        {
            get { return GetConnectionStringData("database"); }
        }
        public string SqlServer
        {
            get { return GetConnectionStringData("server"); }
        }

        public List<string> ListFiles { get; set; } = new List<string>();


        public SvnDirectory() {}

        public string GetConnectionStringData(string property)
        {
            if (HasWebConfig)
            {
                string propertyValue = string.Empty;

                Match matchProperty = Regex.Match(_webConfig.ConnectionStrings.ConnectionStrings["Main"].ConnectionString,
                    string.Format("(\\b{0}(\\s+)?=(\\s+)?\\S+\\;)", property), RegexOptions.IgnoreCase);
                if (matchProperty.Success)
                {
                    propertyValue = matchProperty.Value;

                    int propertyEqualIndex = propertyValue.IndexOf("=") + 1;
                    if (propertyEqualIndex != -1)
                        propertyValue = propertyValue.Substring(propertyEqualIndex, propertyValue.Length - propertyEqualIndex);

                    propertyValue = propertyValue.Trim(new char[] { ' ', '\"', ';' });
                }

                return propertyValue;
            }

            return string.Empty;
        }

        public void GetSvnFiles()
        {
            using (SvnClient svnClient = new SvnClient())
            {
                //var location = new Uri("http://my.example/repos/trunk");
                //svnClient.DiffSummary(
                //    new SvnUriTarget(location, 12),
                //    new SvnUriTarget(location, SvnRevision.Head),
                //    delegate (object sender, SvnDiffSummaryEventArgs e)
                //    {
                //        ListFiles.Add(e.Path);
                //    }
                //);
            }
        }
    }
}