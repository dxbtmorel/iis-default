using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Web.Configuration;
using System.Configuration;
using System.Reflection;

public sealed class EnvType
{
	private readonly string name;
	private readonly int value;

	public static readonly EnvType Dev = new EnvType(1, "danger");
	public static readonly EnvType Stable = new EnvType(2, "success");
	public static readonly EnvType Old = new EnvType(3, "warning");
	public static readonly EnvType Project = new EnvType(4, "default");
	public static readonly EnvType Unknown = new EnvType(5, "info");

	private EnvType(int value, string name)
	{
		this.name = name;
		this.value = value;
	}

	public override string ToString()
	{
		return name;
	}
}

public partial class IisDefault : System.Web.UI.Page
{
	#region Properties
	public string ServerName
	{
		get { return Request.ServerVariables["SERVER_NAME"]; }
	}

	protected List<string> listDevDirectories = new List<string>();
	public string RenderedDevDirectories { get; set; }

	protected List<string> listStableDirectories = new List<string>();
	public string RenderedStableDirectories { get; set; }

	protected List<string> listProjectDirectories = new List<string>();
	public string RenderedProjectDirectories { get; set; }

	protected List<string> listUnknownDirectories = new List<string>();
	public string RenderedUnknownDirectories { get; set; }

	protected List<string> listOldDirectories = new List<string>();
	public string RenderedOldDirectories { get; set; }
	#endregion

	#region Events
	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);

		Response.Redirect("localhost/WebApp/");

		Response.AddHeader("Pragma", "No-Cache");
		Response.CacheControl = "Private";

		string physicalPath = Server.MapPath("/");
		string[] directories = Directory.GetDirectories(physicalPath, "buyer*");
		foreach (string directory in directories)
		{
			DirectoryInfo di = new DirectoryInfo(directory);

			int version = 0;
			string[] nameSplited = di.Name.Split('_');
			foreach (string part in nameSplited)
			{
				if (int.TryParse(part, out version))
					break;
			}
			if (!di.Name.StartsWith("buyer_demo", StringComparison.InvariantCultureIgnoreCase))
				listProjectDirectories.Add(directory);
			else if (!di.Name.StartsWith("buyer_demo_V8", StringComparison.InvariantCultureIgnoreCase))
				listOldDirectories.Add(directory);
			else if (version != 0 && (version % 2) == 0)
				listStableDirectories.Add(directory);
			else if (version != 0 && (version % 2) != 0)
				listDevDirectories.Add(directory);
			else if (di.Name.StartsWith("buyer_demo_V8_dev", StringComparison.InvariantCultureIgnoreCase))
				listDevDirectories.Add(directory);
			else if (di.Name.StartsWith("buyer_demo_V8", StringComparison.InvariantCultureIgnoreCase))
				listStableDirectories.Add(directory);
		}
		RenderedDevDirectories = RenderDirectories(listDevDirectories, EnvType.Dev);
		RenderedStableDirectories = RenderDirectories(listStableDirectories, EnvType.Stable);
		RenderedProjectDirectories = RenderDirectories(listProjectDirectories, EnvType.Project);
		RenderedOldDirectories = RenderDirectories(listOldDirectories, EnvType.Old);
		RenderedUnknownDirectories = RenderDirectories(listUnknownDirectories, EnvType.Old);
	}
	#endregion

	protected string RenderDirectories(List<string> directories, EnvType type)
	{
		StringBuilder sb = new StringBuilder();
		foreach (string directory in directories)
		{
			string database = string.Empty;
			string server = string.Empty;
			string[] files = Directory.GetFiles(directory, "Web.config");
			
			foreach (string file in files)
			{
				database = GetWebConfigProperty(file, "database");
				server = GetWebConfigProperty(file, "server");
				if (!string.IsNullOrEmpty(database) || !string.IsNullOrEmpty(server))
					break;
			}

			DirectoryInfo di = new DirectoryInfo(directory);

			if (type == EnvType.Project)
				sb.AppendLine("<div class='col-sm-4'>");
			sb.AppendLine(string.Format("<div class='panel panel-{0}'>", type.ToString()));
			sb.AppendLine("<div class='panel-heading'>");
			sb.AppendLine("<h3 class='panel-title'>");
			if (files.Length > 0)
			{
				sb.AppendFormat("<a href='{0}'>{0}</a>", di.Name);
				sb.AppendLine("<span class='badge'>");
				sb.AppendLine(string.Format("<a role='button' data-toggle='collapse' href='#collapse-panel-body-{0}' aria-expanded='false' aria-controls='collapse-panel-body-{0}'>", di.Name));
				sb.AppendLine("<i class='panel-title-btn-open fa fa-plus' aria-hidden='true'></i>");
				sb.AppendLine("</a>");
				sb.AppendLine("</span>");
			}
			else
				sb.Append(di.Name);
			sb.AppendLine("</h3>");
			sb.AppendLine("</div>");
			sb.AppendLine(string.Format("<div id='collapse-panel-body-{0}' class='panel-collapse collapse'>", di.Name));
			sb.AppendLine("<div class='panel-body'>");
			sb.AppendLine("<div class='input-group input-group-sm'>");
			sb.AppendLine(string.Format("<span class='input-group-addon' id='basic-addon-sqlserver-{0}'>SQL Server</span>", di.Name));
			sb.AppendLine(string.Format("<input type='text' class='form-control' aria-describedby='basic-addon-sqlserver-{0}' readonly value='{1}'>", di.Name, server));
			sb.AppendLine("</div>");
			sb.AppendLine("<br>");
			sb.AppendLine("<div class='input-group input-group-sm'>");
			sb.AppendLine(string.Format("<span class='input-group-addon' id='basic-addon-database-{0}'>Database</span>", di.Name));
			sb.AppendLine(string.Format("<input type='text' class='form-control' aria-describedby='basic-addon-database-{0}' readonly value='{1}'>", di.Name, database));
			sb.AppendLine("</div>");
			sb.AppendLine("</div>");
			sb.AppendLine("</div>");
			sb.AppendLine("</div>");
			if (type == EnvType.Project)
				sb.AppendLine("</div>");
		}
		return sb.ToString();
	}

	private string GetWebConfigProperty(string filePath, string property)
	{
		string[] lines = File.ReadAllLines(filePath);

		try
		{
			foreach (string line in lines)
			{
				string value = GetPropertyValue(line, property);
				if (value != null)
					return value;
			}
		}
		catch (Exception ex)
		{
		}

		return null;
	}

	private string GetPropertyValue(string line, string property)
	{
		string propertyValue = null;

		Match matchProperty = Regex.Match(line, string.Format("(\\b{0}(\\s+)?=(\\s+)?\"?\\w+\"?)", property), RegexOptions.IgnoreCase);
		if (matchProperty.Success)
		{
			propertyValue = matchProperty.Value;

			int propertyEqualIndex = propertyValue.IndexOf("=") + 1;
			if (propertyEqualIndex != -1)
				propertyValue = propertyValue.Substring(propertyEqualIndex, propertyValue.Length - propertyEqualIndex);

			propertyValue = propertyValue.Trim(new char[] { ' ', '\"' });
		}

		return propertyValue;
	}
}