using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace VcfToSimplyNote
{
    public class VcsReader
    {
        readonly string[] _tags = new[] { "SUMMARY", "DUE", "DESCRIPTION", "LAST-MODIFIED " };
        readonly char[] _tokens = new[] { ':', ';' };
        readonly string[] _dateformats = new[] { "yyyyMMddTHHmmssZ", "yyyyMMddTHHmmss" };

        public IEnumerable<VcsItem> ReadFolder(string folder)
        {
            return ReadFiles(Directory.EnumerateFiles(folder, "*.vcs"));
        }

        public VcsItem ReadFile(string file)
        {
            System.Diagnostics.Debug.WriteLine("Reading {0}", file);
            var result = new VcsItem();
            using (var stream = new StreamReader(file))
            {
                string line;
                while ((line = stream.ReadLine()) != null)
                {
                    var index = line.IndexOfAny(_tokens);
                    if (index != -1)
                    {
                        string value = line.Substring(index + 1);
                        switch (line.Substring(0, index))
                        {
                            case "DUE":
                                result.Created = ParseDate(value);
                                break;
                            case "SUMMARY":
                                result.Summary = Decode(GetText(value, stream));
                                break;
                            case "DESCRIPTION":
                                result.Description = Decode(GetText(value, stream));
                                break;
                            case "LAST-MODIFIED":
                                result.Modified = ParseDate(value);
                                break;
                        }
                    }
                }
            }
            return result;
        }

        private string Decode(string s)
        {
            if (Regex.Matches(s, @"\=([0-9A-F]){2}").Count == 0)
                return s;
            var attachment = Attachment.CreateAttachmentFromString(String.Empty, "=?utf-8?Q?" + s + "?=");
#if DEBUG
            if (attachment.Name.StartsWith("=?utf-8?Q?"))
            {
                System.Diagnostics.Debug.WriteLine("  From {0}\n  To {1}\n  ------------", s, attachment.Name);
            }
#endif
            return attachment.Name;
        }

        private string GetText(string s, StreamReader sr)
        {
            string afterColon = GetAfter(s, ":");
            if (LastString(afterColon))
                return afterColon;
            return afterColon.Substring(0, afterColon.Length - 1) + GetRemaining(sr);
        }

        private string GetAfter(string s, string after)
        {
            var index = s.IndexOf(after);
            if (index != -1)
                return s.Substring(index + 1);
            return s;
        }

        private bool LastString(string s)
        {
            return !s.EndsWith("=");
        }

        private string GetRemaining(StreamReader reader)
        {
            var result = new StringBuilder();
            do
            {
                string s = reader.ReadLine();
                if (s == null)
                    return result.ToString();
                var lastString = LastString(s);
                result.Append(lastString ? s : s.Substring(0, s.Length - 1));
                if (lastString)
                    return result.ToString();
            } while (true);

        }

        private DateTime ParseDate(string s)
        {
            return DateTime.ParseExact(s, _dateformats, CultureInfo.InvariantCulture, DateTimeStyles.None);
        }

        public IEnumerable<VcsItem> ReadFiles(IEnumerable<string> files)
        {
            return files.Select(ReadFile);
        }
    }
}
