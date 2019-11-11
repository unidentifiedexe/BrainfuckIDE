using JsonHelper.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace JsonHelper
{
    public class WellKnownTypeDeserializer
    {
        private readonly Type[] _knownType;

        public WellKnownTypeDeserializer(Type[] knownType)
        {
            _knownType = knownType;
        }

        private enum JsonType
        {
            @string, number, boolean, @object, array, @null
        }

        // public static methods

        /// <summary>from JsonSring to DynamicJson</summary>
        public object Parse(string json)
        {
            return Parse(json, Encoding.Unicode);
        }

        /// <summary>from JsonSring to DynamicJson</summary>
        public object Parse(string json, Encoding encoding)
        {
            using var reader = JsonReaderWriterFactory.CreateJsonReader(encoding.GetBytes(json), XmlDictionaryReaderQuotas.Max);
            return ToValue(XElement.Load(reader));
        }

        /// <summary>from JsonSringStream to DynamicJson</summary>
        public object Parse(Stream stream)
        {
            using var reader = JsonReaderWriterFactory.CreateJsonReader(stream, XmlDictionaryReaderQuotas.Max);
            return ToValue(XElement.Load(reader));
        }

        /// <summary>from JsonSringStream to DynamicJson</summary>
        public object Parse(Stream stream, Encoding encoding)
        {
            using var reader = JsonReaderWriterFactory.CreateJsonReader(stream, encoding, XmlDictionaryReaderQuotas.Max, _ => { });
            return ToValue(XElement.Load(reader));
        }

        ///// <summary>create JsonSring from primitive or IEnumerable or Object({public property name:property value})</summary>
        //public static string Serialize(object obj)
        //{
        //    return CreateJsonString(new XStreamingElement("root", CreateTypeAttr(GetJsonType(obj)), CreateJsonNode(obj)));
        //}

        // private static methods

        private object ToValue(XElement element)
        {
            return ToValueHelper(element) switch
            {
                Info f => Deserialize(f, null),
                object x => x,
            };
        }
        private static object ToValueHelper(XElement element)
        {
            var type = (JsonType)Enum.Parse(typeof(JsonType), element.Attribute("type").Value);
            switch (type)
            {
                case JsonType.boolean:
                    return (bool)element;
                case JsonType.number:
                    return (double)element;
                case JsonType.@string:
                    return (string)element;
                case JsonType.@object:
                case JsonType.array:
                    return new Info(element, type);
                case JsonType.@null:
                default:
                    return null;
            }
        }

        private static JsonType GetJsonType(object obj)
        {
            if (obj == null) return JsonType.@null;

            switch (Type.GetTypeCode(obj.GetType()))
            {
                case TypeCode.Boolean:
                    return JsonType.boolean;
                case TypeCode.String:
                case TypeCode.Char:
                case TypeCode.DateTime:
                    return JsonType.@string;
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.SByte:
                case TypeCode.Byte:
                    return JsonType.number;
                case TypeCode.Object:
                    return (obj is IEnumerable) ? JsonType.array : JsonType.@object;
                case TypeCode.DBNull:
                case TypeCode.Empty:
                default:
                    return JsonType.@null;
            }
        }

        private static XAttribute CreateTypeAttr(JsonType type)
        {
            return new XAttribute("type", type.ToString());
        }

        private static object CreateJsonNode(object obj)
        {
            var type = GetJsonType(obj);
            switch (type)
            {
                case JsonType.@string:
                case JsonType.number:
                    return obj;
                case JsonType.boolean:
                    return obj.ToString().ToLower();
                case JsonType.@object:
                    return CreateXObject(obj);
                case JsonType.array:
                    return CreateXArray(obj as IEnumerable);
                case JsonType.@null:
                default:
                    return null;
            }
        }

        private static IEnumerable<XStreamingElement> CreateXArray<T>(T obj) where T : IEnumerable
        {
            return obj.Cast<object>()
                .Select(o => new XStreamingElement("item", CreateTypeAttr(GetJsonType(o)), CreateJsonNode(o)));
        }

        private static IEnumerable<XStreamingElement> CreateXObject(object obj)
        {
            return obj.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(pi => (pi.Name, Value: pi.GetValue(obj, null)))
                .Select(a => new XStreamingElement(a.Name, CreateTypeAttr(GetJsonType(a.Value)), CreateJsonNode(a.Value)));
        }

        //private static string CreateJsonString(XStreamingElement element)
        //{
        //    using (var ms = new MemoryStream())
        //    using (var writer = JsonReaderWriterFactory.CreateJsonWriter(ms, Encoding.Unicode))
        //    {
        //        element.WriteTo(writer);
        //        writer.Flush();
        //        return Encoding.Unicode.GetString(ms.ToArray());
        //    }
        //}
        // dynamic structure represents JavaScript Object/Array


        struct Info
        {
            public XElement Xml { get; }

            public JsonType JsonType { get; }
            public bool IsObject { get { return JsonType == JsonType.@object; } }

            public bool IsArray { get { return JsonType == JsonType.array; } }


            public Info(XElement xml, JsonType jsonType)
            {
                Xml = xml;
                JsonType = jsonType;
            }


            /// <summary>has property or not</summary>
            public bool IsDefined(string name)
            {
                return IsObject && (Xml.Element(name) != null);
            }

            /// <summary>has property or not</summary>
            public bool IsDefined(int index)
            {
                return IsArray && (Xml.Elements().ElementAtOrDefault(index) != null);
            }


            /// <summary>delete property</summary>
            public bool Delete(string name)
            {
                var elem = Xml.Element(name);
                if (elem != null)
                {
                    elem.Remove();
                    return true;
                }
                else return false;
            }

            /// <summary>delete property</summary>
            public bool Delete(int index)
            {
                var elem = Xml.Elements().ElementAtOrDefault(index);
                if (elem != null)
                {
                    elem.Remove();
                    return true;
                }
                else return false;
            }

        }


        /// <summary>mapping to Array or Class by Public PropertyName</summary>
        private T Deserialize<T>(Info info)
        {
            return (T)Deserialize(info, typeof(T));
        }

        private object Deserialize(Info info, Type type)
        {
            return (info.IsArray) ? DeserializeArray(info.Xml, type) : DeserializeObject(info.Xml, type);
        }

        private dynamic DeserializeValue(XElement element, Type elementType)
        {
            var value = ToValueHelper(element);
            if (value is Info info)
            {
                value = Deserialize(info, elementType);
                if (elementType.IsAssignableFrom(value.GetType()))
                    return value;
            }
            return Convert.ChangeType(value, elementType);
        }

        private object DeserializeObject(XElement xml, Type targetType)
        {
            targetType = TryGetKnownType(xml, targetType);
            if (targetType == null) return null;
            var result = Activator.CreateInstance(targetType);
            var dict = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite)
                .ToDictionary(pi => pi.Name, pi => pi);
            foreach (var item in xml.Elements())
            {
                if (!dict.TryGetValue(item.Name.LocalName, out var propertyInfo)) continue;
                var atr = propertyInfo.GetCustomAttribute(typeof(DefaultCollectionTypeAttribute));
                var propType = propertyInfo.PropertyType;
                if (atr is DefaultCollectionTypeAttribute attribute)
                    propType = attribute.TryUseType(propType);
                var value = DeserializeValue(item, propType);
                propertyInfo.SetValue(result, value, null);
            }
            return result;
        }

        private object DeserializeArray(XElement xml, Type targetType)
        {

            if (targetType.IsArray) // Foo[]
            {
                var elemType = targetType.GetElementType();
                dynamic array = Array.CreateInstance(elemType, xml.Elements().Count());
                var index = 0;
                foreach (var item in xml.Elements())
                {
                    array[index++] = DeserializeValue(item, elemType);
                }
                return array;
            }
            else // List<Foo>
            {
                var elemType = targetType.GetGenericArguments()[0];
                dynamic list = Activator.CreateInstance(targetType);
                foreach (var item in xml.Elements())
                {
                    list.Add(DeserializeValue(item, elemType));
                }
                return list;
            }
        }


        ///// <summary>Serialize to JsonString</summary>
        //public override string ToString()
        //{
        //    // <foo type="null"></foo> is can't serialize. replace to <foo type="null" />
        //    foreach (var elem in _xml.Descendants().Where(x => x.Attribute("type").Value == "null"))
        //    {
        //        elem.RemoveNodes();
        //    }
        //    return CreateJsonString(new XStreamingElement("root", CreateTypeAttr(_jsonType), _xml.Elements()));
        //}


        private Type TryGetKnownType(XElement xml, Type targetType)
        {
            var attribute = xml.Attribute("__type");
            if (attribute.Value != null)
                return TryGetKnownType(attribute.Value, targetType);
            else
                return targetType;
        }
        private Type TryGetKnownType(string typeName, Type targetType)
        {
            var t = typeName.Split(':').First().Split('.').Last();
            var tt = _knownType.FirstOrDefault(p => p.Name == t && (targetType?.IsAssignableFrom(p) ?? true));
            return tt ?? targetType;
        }
    }
}
