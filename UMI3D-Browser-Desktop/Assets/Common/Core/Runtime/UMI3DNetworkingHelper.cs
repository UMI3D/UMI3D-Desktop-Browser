﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace umi3d.common
{
    public static class UMI3DNetworkingHelper
    {

        static List<Umi3dNetworkingHelperModule> modules = new List<Umi3dNetworkingHelperModule>();

        public static void AddModule(Umi3dNetworkingHelperModule module)
        {
            modules.Add(module);
        }

        public static void RemoveModule(Umi3dNetworkingHelperModule module)
        {
            modules.Remove(module);
        }

        public static void AddModule(List<Umi3dNetworkingHelperModule> moduleList)
        {
            foreach(var module in moduleList)
                modules.Add(module);
        }

        public static void RemoveModule(List<Umi3dNetworkingHelperModule> moduleList)
        {
            foreach (var module in moduleList)
                modules.Remove(module);
        }

        public static T Read<T>(ByteContainer container)
        {
            T result;
            TryRead<T>(container, out result);
            return result;
        }

        public static bool TryRead<T>(ByteContainer container, out T result)
        {
            switch (true)
            {
                case true when typeof(T) == typeof(char):
                    if (container.length >= sizeof(char))
                    {
                        result = (T)Convert.ChangeType(BitConverter.ToChar(container.bytes,container.position), typeof(T));
                        container.position += sizeof(char);
                        container.length -= sizeof(char);
                        return true;
                    }
                    break;
                case true when typeof(T) == typeof(bool):
                    if (container.length >= sizeof(bool))
                    {
                        result = (T)Convert.ChangeType(BitConverter.ToBoolean(container.bytes, container.position), typeof(T));
                        container.position += sizeof(bool);
                        container.length -= sizeof(bool);
                        return true;
                    }
                    break;
                case true when typeof(T) == typeof(byte):
                    if (container.length >= sizeof(byte))
                    {
                        result = (T)Convert.ChangeType(container.bytes[container.position], typeof(T));
                        container.position += 1;
                        container.length -= 1;
                        return true;
                    }
                    break;
                case true when typeof(T) == typeof(short):
                    if (container.length >= sizeof(short))
                    {
                        result = (T)Convert.ChangeType(BitConverter.ToInt16(container.bytes, container.position), typeof(T));
                        container.position += sizeof(short);
                        container.length -= sizeof(short);
                        return true;
                    }
                    break;
                case true when typeof(T) == typeof(ushort):
                    if (container.length >= sizeof(ushort))
                    {
                        result = (T)Convert.ChangeType(BitConverter.ToUInt16(container.bytes, container.position), typeof(T));
                        container.position += sizeof(ushort);
                        container.length -= sizeof(ushort);
                        return true;
                    }
                    break;
                case true when typeof(T) == typeof(int):
                    if (container.length >= sizeof(int))
                    {
                        result = (T)Convert.ChangeType(BitConverter.ToInt32(container.bytes, container.position), typeof(T));
                        container.position += sizeof(int);
                        container.length -= sizeof(int);
                        return true;
                    }
                    break;
                case true when typeof(T) == typeof(uint):
                    if (container.length >= sizeof(uint))
                    {
                        result = (T)Convert.ChangeType(BitConverter.ToUInt32(container.bytes, container.position), typeof(T));
                        container.position += sizeof(uint);
                        container.length -= sizeof(uint);
                        return true;
                    }
                    break;
                case true when typeof(T) == typeof(float):
                    if (container.length >= sizeof(float))
                    {
                        result = (T)Convert.ChangeType(BitConverter.ToSingle(container.bytes, container.position), typeof(T));
                        container.position += sizeof(float);
                        container.length -= sizeof(float);
                        return true;
                    }
                    break;
                case true when typeof(T) == typeof(long):
                    if (container.length >= sizeof(long))
                    {
                        result = (T)Convert.ChangeType(BitConverter.ToInt64(container.bytes, container.position), typeof(T));
                        container.position += sizeof(long);
                        container.length -= sizeof(long);
                        return true;
                    }
                    break;
                case true when typeof(T) == typeof(ulong):
                    if (container.length >= sizeof(ulong))
                    {
                        result = (T)Convert.ChangeType(BitConverter.ToUInt64(container.bytes, container.position), typeof(T));
                        container.position += sizeof(ulong);
                        container.length -= sizeof(ulong);
                        return true;
                    }
                    break;
                case true when typeof(T) == typeof(SerializableVector2):
                case true when typeof(T) == typeof(Vector2):
                    if (container.length >= 2 * sizeof(float))
                    {
                        result = (T)Convert.ChangeType(new Vector2(BitConverter.ToSingle(container.bytes, container.position), BitConverter.ToSingle(container.bytes, container.position + sizeof(float))), typeof(T));
                        container.position += 2 * sizeof(float);
                        container.length -= 2 * sizeof(float);
                        return true;
                    }
                    break;
                case true when typeof(T) == typeof(SerializableVector3):
                case true when typeof(T) == typeof(Vector3):
                    if (container.length >= 3 * sizeof(float))
                    {
                        result = (T)Convert.ChangeType(new Vector3(BitConverter.ToSingle(container.bytes, container.position), BitConverter.ToSingle(container.bytes, container.position + sizeof(float)), BitConverter.ToSingle(container.bytes, container.position + 2 * sizeof(float))), typeof(T));
                        container.position += 3 * sizeof(float);
                        container.length -= 3 * sizeof(float);
                        return true;
                    }
                    break;
                case true when typeof(T) == typeof(Quaternion):
                    if (container.length >= 4 * sizeof(float))
                    {
                        float x, y, z, w;
                        x = BitConverter.ToSingle(container.bytes, container.position);
                        y = BitConverter.ToSingle(container.bytes, container.position + sizeof(float));
                        z = BitConverter.ToSingle(container.bytes, container.position + 2 * sizeof(float));
                        w = BitConverter.ToSingle(container.bytes, container.position + 3 * sizeof(float));
                        result = (T)Convert.ChangeType(new Quaternion(x, y, z, w), typeof(T));
                        container.position += 4 * sizeof(float);
                        container.length -= 4 * sizeof(float);
                        return true;
                    }
                    break;
                case true when typeof(T) == typeof(SerializableColor):
                case true when typeof(T) == typeof(Color):
                    if (container.length >= 4 * sizeof(float))
                    {
                        float x, y, z, w;
                        x = BitConverter.ToSingle(container.bytes, container.position);
                        y = BitConverter.ToSingle(container.bytes, container.position + sizeof(float));
                        z = BitConverter.ToSingle(container.bytes, container.position + 2 * sizeof(float));
                        w = BitConverter.ToSingle(container.bytes, container.position + 3 * sizeof(float));
                        result = (T)Convert.ChangeType(new Color(x, y, z, w), typeof(T));
                        container.position += 4 * sizeof(float);
                        container.length -= 4 * sizeof(float);
                        return true;
                    }
                    break;
                case true when typeof(T) == typeof(SerializableVector4):
                case true when typeof(T) == typeof(Vector4):
                    if (container.length >= 4 * sizeof(float))
                    {
                        float x, y, z, w;
                        TryRead(container, out x);
                        TryRead(container, out y);
                        TryRead(container, out z);
                        TryRead(container, out w);
                        result = (T)Convert.ChangeType(new Vector4(x, y, z, w), typeof(T));
                        return true;
                    }
                    break;
                case true when typeof(T) == typeof(SerializableMatrix4x4):
                    if (container.length >= 4 * 4 * sizeof(float))
                    {
                        Vector4 c0, c1, c2, c3;

                        TryRead(container, out c0);
                        TryRead(container, out c1);
                        TryRead(container, out c2);
                        TryRead(container, out c3);

                        result = (T)Convert.ChangeType(new SerializableMatrix4x4(c0, c1, c2, c3), typeof(T));
                        return true;
                    }
                    break;
                case true when typeof(T) == typeof(string):
                    if (container.length == 0) throw new Exception($"String length should not be 0");
                    result = default(T);
                    uint s;
                    string r = "";
                    if (TryRead<uint>(container, out s))
                    {
                        for (uint i = 0; i < s; i++)
                        {
                            char c;
                            if (TryRead<char>(container, out c))
                            {
                                r += c;
                            }
                            else return false;
                        }
                    }
                    else return false;
                    result = (T)Convert.ChangeType(r, typeof(T));
                    return true;
                default:
                    bool read;
                    foreach (var module in modules)
                        if (module.Read<T>(container, out read, out result))
                            return read;
                    throw new Exception($"Missing case [{typeof(T)} was not catched]");
            }
            result = default(T);
            return false;
        }

        public static T[] ReadArray<T>(ByteContainer container)
        {
            return ReadList<T>(container).ToArray();
        }
        public static List<T> ReadList<T>(ByteContainer container)
        {
            var res = new List<T>();
            var Length = container.bytes.Length;
            for (; container.position < Length && container.length > 0;)
            {
                T result;
                if (TryRead<T>(container, out result))
                    res.Add(result);
                else
                    break;
            }
            return res;
        }

        public static List<T> ReadList<T>(ByteContainer container, int count)
        {
            var res = new List<T>();
            var Length = container.bytes.Length;
            for (int i = 0; container.position < Length && container.length > 0 && i < count; i++)
            {
                T result;
                if (TryRead<T>(container, out result))
                    res.Add(result);
                else
                    break;
            }
            return res;
        }

        public static Bytable Write<T>(T value)
        {
            Func<byte[], int, int, (int, int)> f;
            Bytable bc;
            switch (value)
            {
                case char c:
                    f = (by, i, bs) =>
                    {
                        BitConverter.GetBytes(c).CopyTo(by, i);
                        var s = sizeof(char);
                        return ( i +s, bs + s);
                    };
                    return new Bytable(sizeof(char), f);
                case bool b:
                    f = (by, i, bs) =>
                    {
                        BitConverter.GetBytes(b).CopyTo(by, i);
                        var s = sizeof(bool);
                        return (i + s, bs + s);
                    };
                    return new Bytable(sizeof(bool), f);
                case byte b:
                    f = (by, i, bs) =>
                    {
                        BitConverter.GetBytes(b).CopyTo(by, i);
                        var s = sizeof(byte);
                        return (i + s, bs + s);
                    };
                    return new Bytable(sizeof(byte), f);
                case short b:
                    f = (by, i, bs) =>
                    {
                        BitConverter.GetBytes(b).CopyTo(by, i);
                        var s = sizeof(short);
                        return (i + s, bs + s);
                    };
                    return new Bytable(sizeof(short), f);
                case ushort b:
                    f = (by, i, bs) =>
                    {
                        BitConverter.GetBytes(b).CopyTo(by, i);
                        var s = sizeof(ushort);
                        return (i + s, bs + s);
                    };
                    return new Bytable(sizeof(ushort), f);
                case int b:
                    f = (by, i, bs) =>
                    {
                        BitConverter.GetBytes(b).CopyTo(by, i);
                        var s = sizeof(int);
                        return (i + s, bs + s);
                    };
                    return new Bytable(sizeof(int), f);
                case uint b:
                    f = (by, i, bs) =>
                    {
                        BitConverter.GetBytes(b).CopyTo(by, i);
                        var s = sizeof(uint);
                        return (i + s, bs + s);
                    };
                    return new Bytable(sizeof(uint), f);
                case float b:
                    f = (by, i, bs) =>
                    {
                        BitConverter.GetBytes(b).CopyTo(by, i);
                        var s = sizeof(float);
                        return (i + s, bs + s);
                    };
                    return new Bytable(sizeof(float), f);
                case long b:
                    f = (by, i, bs) =>
                    {
                        BitConverter.GetBytes(b).CopyTo(by, i);
                        var s = sizeof(long);
                        return (i + s, bs + s);
                    };
                    return new Bytable(sizeof(long), f);
                case ulong b:
                    f = (by, i, bs) =>
                    {
                        BitConverter.GetBytes(b).CopyTo(by, i);
                        var s = sizeof(ulong);
                        return (i + s, bs + s);
                    };
                    return new Bytable(sizeof(ulong), f);
                case SerializableVector2 v2:
                    bc = Write(v2.X);
                    bc += Write(v2.Y);
                    return bc;
                case Vector2 v2:
                    bc = Write(v2.x);
                    bc += Write(v2.y);
                    return bc;
                case SerializableVector3 v3:
                    bc = Write(v3.X);
                    bc += Write(v3.Y);
                    bc += Write(v3.Z);
                    return bc;
                case Vector3 v3:
                    bc = Write(v3.x);
                    bc += Write(v3.y);
                    bc += Write(v3.z);
                    return bc;
                case SerializableVector4 v4:
                    bc = Write(v4.X);
                    bc += Write(v4.Y);
                    bc += Write(v4.Z);
                    bc += Write(v4.W);
                    return bc;
                case Vector4 v4:
                    bc = Write(v4.x);
                    bc += Write(v4.y);
                    bc += Write(v4.z);
                    bc += Write(v4.w);
                    return bc;
                case Quaternion q:
                    bc = Write(q.x);
                    bc += Write(q.y);
                    bc += Write(q.z);
                    bc += Write(q.w);
                    return bc;
                case Color q:
                    bc = Write(q.r);
                    bc += Write(q.g);
                    bc += Write(q.b);
                    bc += Write(q.a);
                    return bc;
                case SerializableColor q:
                    bc = Write(q.R);
                    bc += Write(q.G);
                    bc += Write(q.B);
                    bc += Write(q.A);
                    return bc;
                case SerializableMatrix4x4 v4:
                    bc = Write(v4.c0);
                    bc += Write(v4.c1);
                    bc += Write(v4.c2);
                    bc += Write(v4.c3);
                    return bc;
                case Matrix4x4 v4:
                    return Write((SerializableMatrix4x4)v4);
                case string str:
                    bc = Write((uint)str.Length);
                    foreach (char ch in str)
                    {
                        bc += Write(ch);
                    }
                    return bc;
                case T t when typeof(T) == typeof(string):
                    return Write((uint)0);
                default:
                    foreach (var module in modules)
                        if (module.Write<T>(value, out bc))
                            return bc;
                    break;
            }
            throw new Exception($"Missing case [{typeof(T)} was not catched]");
        }

        public static Bytable WriteArray<T>(IEnumerable<T> value)
        {
            Bytable b = new Bytable();
            foreach (var v in value)
                b = Write(v);
            return b;
        }

        public static Bytable ToBytes(IEnumerable<IByte> operations, params object[] parameters)
        {
            Func<byte[], int, int, (int, int, int)> f3 = (byte[] by, int i, int j) =>
            {
                return (0, i, j);
            };
            if (operations.Count() > 0)
            {
                int size = operations.Count() * sizeof(int);
                var func = operations
                    .Select(o => o.ToBytableArray(parameters))
                    .Select(c =>
                    {
                        Func<byte[], int, int, (int, int, int)> f1 = (byte[] by, int i, int j) => { var cr = c.function(by, i,0); return (cr.Item1, i, j); };
                        return (c.size, f1);
                    })
                    .Aggregate((0, f3)
                    , (a, b) =>
                    {
                        Func<byte[], int, int, (int, int, int)> f2 = (byte[] by, int i, int j) =>
                        {
                            int i2,sj;
                            (i2, i, j) = a.Item2(by, i, j);
                            (i2, i, j) = b.Item2(by, i, j);
                            (j,sj) = UMI3DNetworkingHelper.Write(i).function(by,j,0);
                            i = i2;
                            return (i2, i, j);
                        };
                        return (a.Item1 + b.Item1, f2);
                    });
                var length = size + func.Item1;

                Func<byte[], int, int, (int, int)> f5 = (byte[] by, int i,int bs) =>
                {
                    var couple = func.Item2(by, i + size, i);
                    return (couple.Item1,couple.Item2);
                };
                return new Bytable(length, f5);
            }
            return new Bytable();
        }


    }

    public interface IByte
    {
        Bytable ToBytableArray(params object[] parameters);
    }

    public class ByteContainer
    {
        public byte[] bytes { get; private set; }
        public int position;
        public int length;

        public ByteContainer(byte[] bytes)
        {
            this.bytes = bytes;
            position = 0;
            length = bytes.Length;
        }

        public override string ToString()
        {
            return $"{bytes.ToString<byte>()} [{position} : {length}]";
        }
    }

    public class Bytable
    {
        public int size { get; private set; }
        public Func<byte[], int, int, (int, int)> function { get; private set; }

        public Bytable(int size, Func<byte[], int, int, (int, int)> function)
        {
            this.size = size;
            this.function = function;
        }

        public Bytable()
        {
            this.size = 0;
            this.function = (by, i, bs) => (i, bs);
        }

        public byte[] ToBytes()
        {
            var b = new byte[size];
            var c = function(b, 0, 0);
            if (c.Item2 != size) Debug.LogError($"Size requested [{size}] and size used [{c.Item2}] have a different value. Last position is {c.Item1}. {b.ToString<byte>()}");
            return b;
        }

        public byte[] ToBytes(byte[] bytes, int position = 0)
        {
            var c = function(bytes, position, 0);
            if (c.Item2 != size) Debug.LogError($"Size requested [{size}] and size used [{c.Item2}] have a different value. Last position is {c.Item1}. {bytes.ToString<byte>()}");
            return bytes;
        }

        public static Bytable operator +(Bytable a, Bytable b)
        {
            if (a == null) return b;
            if (b == null) return a;

            Func<byte[], int, int, (int, int)> f = (by, i, bs) =>
            {
                (i,bs) = a.function(by, i, bs);
                return b.function(by, i, bs);
            };
            return new Bytable(a.size + b.size, f);
        }

    }

    public abstract class Umi3dNetworkingHelperModule
    {
        public abstract bool Write<T>(T value, out Bytable bytable);

        public abstract bool Read<T>(ByteContainer container, out bool readable, out T result);
    }
}