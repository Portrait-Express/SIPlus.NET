using CSIPlus.Internal.Extensions;
using System.Runtime.InteropServices;

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

        private static unsafe SIPlusNative.DataContainerHandle MakeInt<T>(T data) {
            return SIPlusNative.siplus_data_make_int(Convert.ToInt64(data));
        }

        private static unsafe SIPlusNative.DataContainerHandle MakeFloat<T>(T data) {
            return SIPlusNative.siplus_data_make_float(Convert.ToDouble(data));
        }

        private static unsafe SIPlusNative.DataContainerHandle MakeString(string data) {
            return SIPlusNative.siplus_data_make_string(data);
        }

        private static unsafe SIPlusNative.DataContainerHandle MakeBool(bool data) {
            return SIPlusNative.siplus_data_make_bool(data ? 1 : 0);
        }

        private static void ObjectDataDeleter(nint data) {
            var handle = GCHandle.FromIntPtr(data);
            handle.Free();
            GlobalStaticStorage.Release(handle);
        }

        public static unsafe SIPlusNative.DataContainerHandle MakeData(object? data) {
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

                var handle = GCHandle.Alloc(data);
                GlobalStaticStorage.Store(handle);

                return SIPlusNative.siplus_data_make(type.Handle, GCHandle.ToIntPtr(handle), ObjectDataDeleter);
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
