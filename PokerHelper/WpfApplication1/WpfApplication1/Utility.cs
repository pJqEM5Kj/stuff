using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WpfApplication1
{
    static class Utility
    {
        //private static int BufferSize = 4096;


        public static string FormatStr(this string s, params object[] args)
        {
            if (s == null)
            {
                return null;
            }

            return string.Format(s, args);
        }

        public static bool IsNullOrEmptyStr(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static bool IsNullOrEmpty(this IEnumerable collection)
        {
            if (collection == null)
            {
                return true;
            }

            IEnumerator enumerator = collection.GetEnumerator();
            bool res = !enumerator.MoveNext();
            IDisposable disposable = enumerator as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
            return res;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
            {
                return true;
            }

            return !collection.Any();
        }

        public static void CopyStream(Stream input, Stream output, int bufferSize = 4096)
        {
            ReadStream(input,
                (buffer, bytesRead) => output.Write(buffer, 0, bytesRead),
                bufferSize);
        }

        public static byte[] ReadStream(Stream stream, int bufferSize = 4096)
        {
            var res = new List<byte>();

            ReadStream(stream,
                (buffer, bytesRead) => res.AddRange(buffer.Take(bytesRead)),
                bufferSize);

            return res.ToArray();
        }

        public static void ReadStream(Stream stream, Action<byte[], int> onRead, int bufferSize = 4096)
        {
            var buffer = new byte[bufferSize];
            int bytesRead = 0;
            while ((bytesRead = stream.Read(buffer, 0, bufferSize)) != 0)
            {
                onRead(buffer, bytesRead);
            }
        }

        public static Exception GetUnknownEnumValueException<T>(T enumValue)
        {
            Type type = typeof(T);

            if (!type.IsEnum)
            {
                throw new ArgumentException();
            }

            return new InvalidOperationException("Enum value '{0}' is unknown.".FormatStr(enumValue));
        }
    }
}
