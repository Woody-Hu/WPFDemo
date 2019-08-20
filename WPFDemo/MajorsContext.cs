using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFDemo
{
    internal class MajorsContext
    {
        internal IReadOnlyDictionary<string, string> MajorPaths { get;}

        internal MajorsContext(MajorsContextBuilder builder)
        {
            if (builder.MajorPaths == null)
            {
                builder.MajorPaths = new Dictionary<string, string>();
            }

            MajorPaths = new ReadOnlyDictionary<string, string>(builder.MajorPaths);
        }
    }
}
