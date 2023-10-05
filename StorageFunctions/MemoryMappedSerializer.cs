using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace LisaCore.StorageFunctions
{
    public class MemoryMappedSerializer
    {
        // Helper function to get serialized size
        private long GetSerializedSize<T>(T data)
        {
            string json = JsonSerializer.Serialize(data);
            return System.Text.Encoding.UTF8.GetByteCount(json);
        }

        // Serialize data to a memory-mapped file
        public void Serialize<T>(T data, string fileName)
        {
            long size = GetSerializedSize(data);
            using (MemoryMappedFile mmf = MemoryMappedFile.CreateNew(fileName, size))
            {
                using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                {
                    StreamWriter writer = new StreamWriter(stream);
                    JsonSerializer.Serialize(new Utf8JsonWriter(writer.BaseStream), data);
                    writer.Flush();
                }
            }
        }

        // Deserialize data from a memory-mapped file
        public T Deserialize<T>(string fileName)
        {
            T data;
            using (MemoryMappedFile mmf = MemoryMappedFile.CreateOrOpen(fileName, 1))  // Open with minimum size
            {
                using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                {
                    StreamReader reader = new StreamReader(stream);
                    string json = reader.ReadToEnd();
                    data = JsonSerializer.Deserialize<T>(json);
                }
            }
            return data;
        }
    }

}
