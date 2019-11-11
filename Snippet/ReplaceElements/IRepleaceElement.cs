using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Snippets.RepleaceElements
{
    interface IRepleaceElement
    {
        public string Name { get; }
        public string DefaultText { get; }

    }

    [DataContract]
    class RepleaceElement : IRepleaceElement
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string DefaultText { get; set; }
    }
}
