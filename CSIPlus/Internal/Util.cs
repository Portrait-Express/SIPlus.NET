using CSIPlus.Internal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CSIPlus.Internal
{
    internal static class Util
    {
        public static void AssertSuccess(int resultCode)
        {
            if (resultCode == 0) return;
            SIPlusNative.siplus_error_get(out var msgPtr);
            var msg = Marshal.PtrToStringAnsi(msgPtr.DangerousGetHandle());

            switch (resultCode)
            {
                case (int)SIPlusNative.Errors.SIPLUS_INVALID_ARG:
                    throw new SIPlusException(msg ?? "Invalid Argument");

                case (int)SIPlusNative.Errors.SIPLUS_PARSE_ERROR:
                    throw new ParseException(msg ?? "Parse Error");

                case (int)SIPlusNative.Errors.SIPLUS_INVOKE_ERROR:
                    throw new InvocationException(msg ?? "Error during template invocation");

                case (int)SIPlusNative.Errors.SIPLUS_ERR:
                    throw new SIPlusException(msg ?? "Unknown Error");
            }
        }

        private static unsafe nint MakeInt<T>(T data) {
            if (data is long l) {
                var a = SIPlusNative.siplus_data_make_int(l);
                a.DangerousRelease();
                return a.DangerousGetHandle();
            } else {
                throw new InvalidCastException("data was not castable to long");
            }
        }

        private static unsafe nint MakeFloat<T>(T data) {
            if (data is long l) {
                var a = SIPlusNative.siplus_data_make_float(l);
                a.DangerousRelease();
                return a.DangerousGetHandle();
            } else {
                throw new InvalidCastException("data was not castable to long");
            }
        }

        private static unsafe nint MakeString(string data) {
            var a = SIPlusNative.siplus_data_make_string(data);
            a.DangerousRelease();
            return a.DangerousGetHandle();
        }

        private static unsafe nint MakeBool(bool data) {
            var a = SIPlusNative.siplus_data_make_bool(data ? 1 : 0);
            a.DangerousRelease();
            return a.DangerousGetHandle();
        }

        private static void ObjectDataDeleter(nint data) {
            var handle = GCHandle.FromIntPtr(data);
            handle.Free();
            GlobalStaticStorage.Release(handle);
        }

        public static unsafe nint MakeData(object? data) {
            switch(data) {
            case long i: return MakeInt(i);
            case ulong i: return MakeInt(i);
            case int i: return MakeInt(i);
            case uint i: return MakeInt(i);
            case short i: return MakeInt(i);
            case ushort i: return MakeInt(i);
            case char i: return MakeInt(i);
            case byte i: return MakeInt(i);

            case float f: return MakeFloat(f);
            case double f: return MakeFloat(f);

            case string s: return MakeString(s);

            case bool b: return MakeBool(b);

            case null:
            case object _: {
                using var type = new NETTypeInfo().GetNativeTypeInfo();

                var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
                GlobalStaticStorage.Store(handle);

                var dataHandle = SIPlusNative.siplus_data_make(type.Handle, 
                    GCHandle.ToIntPtr(handle), ObjectDataDeleter);

                dataHandle.DangerousRelease();
                return dataHandle.DangerousGetHandle();
            }

            default:
                throw new Exception($"Unsure how to convert {data.GetType().Name}");
            }
        }

        public static unsafe object FromData(SIPlusNative.DataContainerHandle data) {
            throw new NotImplementedException();
        }
    }
}
