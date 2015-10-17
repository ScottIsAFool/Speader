﻿using System;
using System.Collections.Generic;
using System.IO;
using Speader.Interfaces;

namespace Speader.Extensions
{
    public static class BinarySerializerExtensions
    {
        public static void WriteList<T>(this BinaryWriter writer, IList<T> list) where T : IBinarySerializable
        {
            if (list != null)
            {
                writer.Write(list.Count);
                foreach (T item in list)
                {
                    item.Write(writer);
                }
            }
            else
            {
                writer.Write(0);
            }
        }

        public static void WriteList(this BinaryWriter writer, IList<string> list)
        {
            if (list != null)
            {
                writer.Write(list.Count);
                foreach (string item in list)
                {
                    writer.Write(item);
                }
            }
            else
            {
                writer.Write(0);
            }
        }

        public static void Write<T>(this BinaryWriter writer, T value) where T : class, IBinarySerializable
        {
            if (value != null)
            {
                writer.Write(true);
                value.Write(writer);
            }
            else
            {
                writer.Write(false);
            }
        }

        public static void Write(this BinaryWriter writer, DateTime value)
        {
            writer.Write(value.Ticks);
        }

        public static void WriteString(this BinaryWriter writer, string value)
        {
            writer.Write(value ?? string.Empty);
        }

        public static T ReadGeneric<T>(this BinaryReader reader) where T : IBinarySerializable, new()
        {
            if (reader.ReadBoolean())
            {
                var result = new T();
                result.Read(reader);
                return result;
            }
            return default(T);
        }

        public static List<string> ReadList(this BinaryReader reader)
        {
            int count = reader.ReadInt32(); //9
            if (count > 0)
            {
                var list = new List<string>();
                for (int i = 0; i < count; i++)
                {
                    list.Add(reader.ReadString());
                }
                return list;
            }

            return null;
        }

        public static List<T> ReadList<T>(this BinaryReader reader) where T : class, IBinarySerializable, new()
        {
            int count = reader.ReadInt32();
            if (count > 0)
            {
                var list = new List<T>();
                for (int i = 0; i < count; i++)
                {
                    try
                    {
                        var item = new T();
                        list.Add(item.InitFromCache<T>(reader));
                    }
                    catch { }
                }
                return list;
            }

            return null;
        }

        public static DateTime ReadDateTime(this BinaryReader reader)
        {
            long int64 = reader.ReadInt64();
            return new DateTime(int64);
        }
    }
}