using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace VcfToSimplyNote
{
    public static class VcsItemConverter
    {
        public static XDocument ToXDocument(IEnumerable<VcsItem> items)
        {
            return new XDocument(
                new XElement("database",
                    new XAttribute("name", "notes"),
                    new XAttribute("version", "22"),
                    new XElement("table",
                        new XAttribute("name", "notes"),
                        items.Select(ToXElement)
                        )
                    )
                );
        }

        static XElement ToXElement(VcsItem item, int ordinal)
        {
            var modified = ConvertDate(item.Modified).ToString();
            return new XElement("row",
                      GetColElement("text", GetText(item)),
                      GetColElement("alarm", "0"),
                      GetColElement("_id", ordinal.ToString()),
                      GetColElement("gtask_modified", modified),
                      GetColElement("gtask_position", "00000000000000010000"),
                      GetColElement("created", ConvertDate(item.Created).ToString()),
                      GetColElement("gtask_deleted", "0"),
                      GetColElement("password", ""),
                      GetColElement("gtask_id", "null"),
                      GetColElement("gtask_completed", "0"),
                      GetColElement("modified", modified)
                    );

        }

        static long ConvertDate(DateTime date)
        {
            return (long)date.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        static string GetText(VcsItem item)
        {
            if (!String.IsNullOrWhiteSpace(item.Description))
                return item.Summary + Environment.NewLine + item.Description;
            return item.Summary;
        }

        static XElement GetColElement(string name, string data)
        {
            return new XElement("col",
                new XAttribute("name", name),
                new XCData(data)
                );
        }
    }
}
