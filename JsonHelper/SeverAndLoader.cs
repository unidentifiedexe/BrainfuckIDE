using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;

namespace JsonHelper
{
    public class Json
    {
        public static T Load<T>(string path, IEnumerable<Type> knownTypes = null)
        {
            try
            {
                using var sw = new StreamReader(path);
                return GetFromStream<T>( sw.BaseStream,knownTypes);
            }
            catch (IOException)
            {
                return default;
            }
            catch (InvalidCastException)
            {
                return default;
            }
        }

        private static T GetFromStream<T>(Stream stream, IEnumerable<Type> knownTypes)
        {
            var serializer = new DataContractJsonSerializer(typeof(T), knownTypes, 100, true, null, true);
            var obj = serializer.ReadObject(stream);
            return (T)obj;
        }


        public static bool Save<T>(T obj, string path, IEnumerable<Type> knownTypes = null)
        {
            try
            {
                using var sw = new FileStream(path, FileMode.Create);
                Save(obj, sw,knownTypes);
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }

        private static void Save<T>(T obj, Stream stream , IEnumerable<Type> knownTypes)
        {
            var serializer = new DataContractJsonSerializer(obj?.GetType() ?? typeof(T), knownTypes, 100, true, null, true);
            using var writer = JsonReaderWriterFactory.CreateJsonWriter(stream, Encoding.UTF8, true, true, "  ");
            serializer.WriteObject(writer, obj);
            writer.Flush();
        }
    }
}
