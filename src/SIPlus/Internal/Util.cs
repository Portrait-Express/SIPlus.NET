using System.Collections;
using System.Runtime.InteropServices;

namespace SIPlus.NET.Internal;

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

            default:
                throw new SIPlusException("Unknown error: " + msg);
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

    private static unsafe SIPlusNative.DataContainerHandle MakeNull()
    {
        return SIPlusNative.siplus_data_make_null();
    }

    public static ITypeInfo TypeInfoFor(object? value) {
        switch (value) {
            case long i: return new NativeTypeInfo(SIPlusNative.siplus_type_int()); 
            case ulong i: return new NativeTypeInfo(SIPlusNative.siplus_type_int());
            case int i: return new NativeTypeInfo(SIPlusNative.siplus_type_int());
            case uint i: return new NativeTypeInfo(SIPlusNative.siplus_type_int());
            case short i: return new NativeTypeInfo(SIPlusNative.siplus_type_int());
            case ushort i: return new NativeTypeInfo(SIPlusNative.siplus_type_int());
            case char i: return new NativeTypeInfo(SIPlusNative.siplus_type_int());
            case byte i: return new NativeTypeInfo(SIPlusNative.siplus_type_int());

            case float f: return new NativeTypeInfo(SIPlusNative.siplus_type_float());
            case double f: return new NativeTypeInfo(SIPlusNative.siplus_type_float());

            case string s: return new NativeTypeInfo(SIPlusNative.siplus_type_string());

            case bool b: return new NativeTypeInfo(SIPlusNative.siplus_type_bool());

            case null: return new NativeTypeInfo(SIPlusNative.siplus_type_null());

            case SIValue container: return container.Type;
            case object obj: return new NETTypeInfo();
        }
    }

    /// <summary>
    /// Convert a NET object into a native type.
    /// </summary>
    /// <param name="data"></param>
    /// <returns>A handle if conversion is successful, otherwise null.</returns>
    public static unsafe SIPlusNative.DataContainerHandle? TryToNativeType(object? data) {
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

        case null: return MakeNull();
        }

        return null;
    }

    /// <summary>
    /// Convert from A DataContainerHandle to a NET object type. Tries
    /// to convert back to a NET object if not a native type.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static bool TryFromNativeType(SIPlusNative.DataContainerHandle data, out object? result) {
        if (SIPlusNative.siplus_data_is_bool(data)) {
            AssertSuccess(SIPlusNative.siplus_data_as_bool(out var bresult, data));
            result = bresult;
            return true;
        } else if (SIPlusNative.siplus_data_is_float(data)) {
            AssertSuccess(SIPlusNative.siplus_data_as_float(out var fresult, data));
            result = fresult;
            return true;
        } else if (SIPlusNative.siplus_data_is_int(data)) {
            AssertSuccess(SIPlusNative.siplus_data_as_int(out var iresult, data));
            result = iresult;
            return true;
        } else if (SIPlusNative.siplus_data_is_string(data)) {
            AssertSuccess(SIPlusNative.siplus_data_as_string(out var sresult, data));
            result = sresult.Value;
            return true;
        } else if (SIPlusNative.siplus_data_is_null(data)) {
            result = null;
            return true;
        }

        AssertSuccess(SIPlusNative.siplus_data_type(out var type, data));
        var typeData = SIPlusNative.siplus_type_data_ptr(type);
        if (typeData != IntPtr.Zero) {
            AssertSuccess(SIPlusNative.siplus_data_ptr(out var ptr, data));
            result = GCHandle.FromIntPtr(ptr).Target;
            return true;
        }

        result = null;
        return false;
    }

    public static IEnumerable ToEnumerable(this IEnumerator enumerator) {
        while(enumerator.MoveNext()) {
            yield return enumerator.Current;
        }
    }

    public static object? ElementAt(this IEnumerable enumerable, int index) {
        var i = 0;
        var enumerator = enumerable.GetEnumerator();
        while (enumerator.MoveNext()) {
            if(i == index) {
                return enumerator.Current;
            }

            i++;
        }

        throw new IndexOutOfRangeException($"Index '{index}' is out of range of the collection.");
    }
}
