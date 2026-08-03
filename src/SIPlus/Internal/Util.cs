using SIPlus.NET.Internal.Extensions;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
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

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
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

        case null: return MakeNull();

        case DataContainer container: {
            using var type = container.Type.GetNativeTypeInfo();

            var handle = GCHandle.Alloc(container.Value);
            GlobalStaticStorage.Store(handle);

            return SIPlusNative.siplus_data_make(type.Handle, GCHandle.ToIntPtr(handle), &ObjectDataDeleter);
        }

        case object obj:
            NativeTypeInfo typeInfo;

            var attr = obj.GetType().GetCustomAttribute<SIPlusTypeAttribute>();
            if(attr != null) {
                if(!attr.TypeInfoType.IsAssignableTo(typeof(ITypeInfo))) {
                    throw new SIPlusException(
                        $"Type '{attr.TypeInfoType}' from SIPlusType attribute " +
                        $"on type '{obj.GetType()}' does not inherit from ITypeInfo."
                    );
                }

                typeInfo = ((ITypeInfo)Activator.CreateInstance(attr.TypeInfoType)!)
                            .GetNativeTypeInfo();
            } else {
                typeInfo = new NETTypeInfo().GetNativeTypeInfo();
            }

            try {
                var handle = GCHandle.Alloc(data);
                GlobalStaticStorage.Store(handle);

                return SIPlusNative.siplus_data_make(typeInfo.Handle, GCHandle.ToIntPtr(handle), &ObjectDataDeleter);
            } finally {
                typeInfo.Dispose();
            }
        }

        throw new Exception($"Unsure how to convert {data.GetType().Name}");
    }

    public static unsafe object? FromData(SIPlusNative.DataContainerHandle data) {
        if (SIPlusNative.siplus_data_is_bool(data)) {
            AssertSuccess(SIPlusNative.siplus_data_as_bool(out var result, data));
            return result;
        } else if (SIPlusNative.siplus_data_is_float(data)) {
            AssertSuccess(SIPlusNative.siplus_data_as_float(out var result, data));
            return result;
        } else if (SIPlusNative.siplus_data_is_int(data)) {
            AssertSuccess(SIPlusNative.siplus_data_as_int(out var result, data));
            return result;
        } else if (SIPlusNative.siplus_data_is_string(data)) {
            AssertSuccess(SIPlusNative.siplus_data_as_string(out var result, data));
            return result.Value;
        } else if (SIPlusNative.siplus_data_is_null(data)) {
            return null;
        } else if (SIPlusNative.siplus_data_is(data, NETTypeInfo.NativeInstance.Handle)) {
            AssertSuccess(SIPlusNative.siplus_data_ptr(out var ptr, data));
            return GCHandle.FromIntPtr(ptr).Target;
        }

        AssertSuccess(SIPlusNative.siplus_data_type(out var handle, data));
        var type = new NativeTypeInfo(handle);

        if(type.IsIterable(data)) {
            AssertSuccess(
                SIPlusNative.siplus_type_iterate(out var iterator, type.Handle, data)
            );

            return new IteratorEnumerator(new NativeIterator(iterator))
                .ToEnumerable()
                .Cast<object>()
                .ToList(); //Enumerate once to store since Iterators are only usable one time.
        }

        throw new NotImplementedException($"Not sure how to convert from {type.Name} back to object.");
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
