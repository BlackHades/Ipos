using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using SmartWr.Ipos.Core.Enums;
using SmartWr.Ipos.Core.Settings;
using System.Threading.Tasks;

namespace SmartWr.Ipos.Core.Utilities
{
    public static class Extension
    {
        public static string GenerateEmailPath(string email)
        {
            return string.Format("{0}/{1}", AppKeys.EmailTemplateLocation, email);
        }

        public static string Shorten(this string characters, int length, string replacement = "...")
        {
            if (string.IsNullOrEmpty(characters))
                return string.Empty;

            return characters.Length <= length ? characters : characters.Remove(length) + replacement;
        }

        public static bool ValidEmail(this string input)
        {
            return string.IsNullOrEmpty(input) ? false : Regex.IsMatch(input
                , @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        }
        public static string SetActiveClass(this UrlHelper url, string controllerName, string className = "active")
        {
            var cssClass = string.Empty;
            if (string.Equals(url.RequestContext.RouteData.Values["Controller"] as string, controllerName.Trim(), StringComparison.InvariantCultureIgnoreCase))
                cssClass = className;
            return cssClass;
        }

        public static List<SelectListItem> CreateSelectList(Type enumType, string selectedItem)
        {
            return (from object item in Enum.GetValues(enumType)
                    let fi = enumType.GetField(item.ToString())
                    let attribute = fi.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault()
                    let title = attribute == null ? item.ToString() : ((DescriptionAttribute)attribute).Description
                    select new SelectListItem
                    {
                        Value = ((int)(item)).ToString(),
                        Text = title,
                        Selected = selectedItem == item.ToString()
                    }).ToList();
        }

        public static string GetEnumDescription(Type enumType, object Selected)
        {
            var item = Enum.ToObject(enumType, Selected);
            var fi = enumType.GetField(item.ToString());
            var attribute = fi.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
            return attribute == null ? item.ToString() : ((DescriptionAttribute)attribute).Description;
        }

        public static string GetReportExtension(ReportType reportType)
        {
            return reportType == ReportType.EXCEL ? "xls" : reportType == ReportType.WORD ? "doc" : "pdf";
        }

        public static Task<byte[]> GenerateReport(string filePath, ReportType type, Action<LocalReport> viewer)
        {
            string[] streamids;
            Warning[] warnings;
            string mimeType = String.Empty;
            string encoding = String.Empty;
            string extention = String.Empty;
            string filenameExt = String.Empty;

            var rptVw = new ReportViewer();
            rptVw.ProcessingMode = ProcessingMode.Local;
            rptVw.LocalReport.ReportPath = filePath;

            if (viewer != null)
                viewer(rptVw.LocalReport);


            return Task.Factory.StartNew(() =>
              {
                  var bytes = rptVw.LocalReport.Render(type.ToString()
                   , null
                   , out mimeType, out encoding, out filenameExt, out streamids, out warnings);

                  return bytes;

              });
        }
    }
}