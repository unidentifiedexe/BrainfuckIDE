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
        public static T Load<T>(string path)
        {
            try
            {
                using var sw = new StreamReader(path);
                return GetFromStream<T>( sw.BaseStream);
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

        private static T GetFromStream<T>(Stream stream)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            var obj = serializer.ReadObject(stream);
            return (T)obj;
        }


        public static bool Save<T>(T obj, string path)
        {
            try
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                using var sw = new StreamWriter(path, false);
                Save(obj, sw.BaseStream);
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }

        private static void Save<T>(T obj, Stream stream)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            serializer.WriteObject(stream, obj);
        }
    }
}
