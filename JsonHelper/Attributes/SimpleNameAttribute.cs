using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonHelper.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class SimpleNameAttribute : Attribute
    {
        public string SimpleName { get; }
        public SimpleNameAttribute(string simpleName)
        {
            SimpleName = simpleName;
        }


    }
}
