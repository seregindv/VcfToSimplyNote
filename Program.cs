using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VcfToSimplyNote
{
    class Program
    {
        static void Main(string[] args)
        {
            var reader = new VcsReader();
            var folder = @"D:\phone\6120\calendar\todo";
            VcsItemConverter.ToXDocument(reader.ReadFolder(folder).OrderBy(item => item.Created))
                .Save(Path.Combine(folder, "notes.xml"));
        }
    }
}
