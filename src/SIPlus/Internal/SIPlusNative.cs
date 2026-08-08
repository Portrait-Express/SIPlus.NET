using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SIPlus.NET
{
    /// <summary>
    /// Native library wrapper.
    /// </summary>
    internal static partial class SIPlusNative
    {

        /// <summary>
        /// Error codes that native can return.
        /// </summary>
        internal enum Errors { 
            SIPLUS_OK = 0,
            SIPLUS_ERR = 1,
            SIPLUS_INVALID_ARG = 2,
            SIPLUS_PARSE_ERROR = 3,
            SIPLUS_INVOKE_ERROR = 4
        }

        /*
         * Due to WASM compatibility for .NET, native callbacks must be handed manually
         * using UnmanagedCallersOnlyAttribute, and DllImport instead of LibraryImport.
         * Also declaring delegate types is not possible and you will have to use a 
         * delegate* unmanaged[Cdecl]<> manually.
         */

        /// <summary>
        /// </summary>

        
        public const string LIBNAME = "siplus";

        [LibraryImport(LIBNAME, EntryPoint = "siplus_error_get", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_error_get(out StringHandle message);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_error_set", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_error_set(int err, string message);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_string_delete", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial void siplus_string_delete(IntPtr str);



        [LibraryImport(LIBNAME, EntryPoint = "siplus_parser_new", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial ParserHandle siplus_parser_new();

        [LibraryImport(LIBNAME, EntryPoint = "siplus_parser_context", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_parser_context(out ContextHandle context, ParserHandle parser);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_parser_get_interpolation", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_parser_get_interpolation(out TextConstructorHandle constructor, ParserHandle parser, string text, ParseOptsHandle opts);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_parser_get_expression", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_parser_get_expression(out ValueRetrieverHandle retriever, ParserHandle parser, string expr, ParseOptsHandle opts);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_parser_delete", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial void siplus_parser_delete(IntPtr parser);




        [DllImport(LIBNAME)]
        internal static unsafe extern int siplus_value_create(
            out ValueRetrieverHandle retriever, IntPtr context, 
            delegate* unmanaged[Cdecl]<nint*, nint, nint, int> impl, 
            delegate* unmanaged[Cdecl]<nint, void> deleter);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_value_data_ptr", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial nint siplus_value_data_ptr(ValueRetrieverHandle handle);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_value_retrieve", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_value_retrieve(out DataContainerHandle data, ValueRetrieverHandle value, InvocationContextHandle context);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_value_unref", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial void siplus_value_unref(IntPtr parser);




        [LibraryImport(LIBNAME, EntryPoint = "siplus_text_construct", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_text_construct(out StringHandle text, TextConstructorHandle value, InvocationContextHandle context);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_text_unref", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial void siplus_text_unref(IntPtr parser);




        [LibraryImport(LIBNAME, EntryPoint = "siplus_parse_opts_new", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial ParseOptsHandle siplus_parse_opts_new();

        [LibraryImport(LIBNAME, EntryPoint = "siplus_parse_opts_add_global", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_parse_opts_add_global(ParseOptsHandle opts, string name);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_parse_opts_delete", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial void siplus_parse_opts_delete(IntPtr opts);




        [LibraryImport(LIBNAME, EntryPoint = "siplus_context_add_function", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_context_add_function(ContextHandle context, string name, FunctionHandle handle);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_context_add_converter", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_context_add_converter(ContextHandle context, ConverterHandle converter);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_context_use_stl", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_context_use_stl(ContextHandle context);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_context_builder", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_context_builder(out InvocationContextBuilderHandle builder, ContextHandle context);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_context_unref", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial void siplus_context_unref(IntPtr context);



        [DllImport(LIBNAME)]
        internal static unsafe extern int siplus_function_create(
            out FunctionHandle function, IntPtr data, 
            delegate* unmanaged[Cdecl]<IntPtr*, IntPtr, IntPtr, int, IntPtr*, int> value,
            delegate* unmanaged[Cdecl]<nint, void> deleter);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_function_data_ptr", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial nint siplus_function_data_ptr(FunctionHandle handle);
        
        [LibraryImport(LIBNAME, EntryPoint = "siplus_function_value", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_function_value(out ValueRetrieverHandle retriever, FunctionHandle function, IntPtr parent, int paramc, IntPtr[] parameters);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_function_unref", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial void siplus_function_unref(IntPtr function);





        [LibraryImport(LIBNAME, EntryPoint = "siplus_icbuilder_with", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_icbuilder_with(InvocationContextBuilderHandle builder, string name, DataContainerHandle container);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_icbuilder_default", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_icbuilder_default(InvocationContextBuilderHandle builder, DataContainerHandle container);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_icbuilder_build", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_icbuilder_build(out InvocationContextHandle context, InvocationContextBuilderHandle builder);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_icbuilder_delete", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_icbuilder_delete(IntPtr builder);
       



        [LibraryImport(LIBNAME, EntryPoint = "siplus_invocation_unref", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial void siplus_invocation_unref(IntPtr context);


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct SIPlusTypeInfoData {
            public nint data;
            public nint name;
            public delegate* unmanaged[Cdecl]<nint*, nint, nint, nint, int> access;
            public delegate* unmanaged[Cdecl]<nint*, nint, nint, nint, nint, int> index;
            public delegate* unmanaged[Cdecl]<nint*, nint, nint, int> iterate;
            public delegate* unmanaged[Cdecl]<nint, nint, int> is_iterable;
            public delegate* unmanaged[Cdecl]<nint, void> delete;
        }

        [LibraryImport(LIBNAME)]
        internal static unsafe partial int siplus_type_new_s(out TypeInfoHandle handle, SIPlusTypeInfoData data);

        [DllImport(LIBNAME, EntryPoint = "siplus_type_new", CallingConvention = CallingConvention.Cdecl)]
        internal static unsafe extern int siplus_type_new(
            out nint type, nint data, [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            delegate* unmanaged[Cdecl]<nint, nint, int> is_iterable,
            delegate* unmanaged[Cdecl]<nint*, nint, nint, nint, int> access,
            delegate* unmanaged[Cdecl]<nint*, nint, nint, nint, int> index,
            delegate* unmanaged[Cdecl]<nint*, nint, nint, int> iterate,
            delegate* unmanaged[Cdecl]<nint, void> deleter);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_type_data_ptr", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial nint siplus_type_data_ptr(TypeInfoHandle handle);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_type_name", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_type_name(out StringHandle ptr, TypeInfoHandle typeInfo);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_type_access", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_type_access(out DataContainerHandle result, TypeInfoHandle type, DataContainerHandle data, string property);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_type_is_iterable", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_type_is_iterable([MarshalAs(UnmanagedType.I4)] out bool result, TypeInfoHandle info, DataContainerHandle data);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_type_iterate", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_type_iterate(out IteratorHandle result, TypeInfoHandle typeInfo, DataContainerHandle container);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_type_unref", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial void siplus_type_unref(IntPtr type);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_type_int", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial TypeInfoHandle siplus_type_int();
        [LibraryImport(LIBNAME, EntryPoint = "siplus_type_float", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial TypeInfoHandle siplus_type_float();
        [LibraryImport(LIBNAME, EntryPoint = "siplus_type_string", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial TypeInfoHandle siplus_type_string();
        [LibraryImport(LIBNAME, EntryPoint = "siplus_type_bool", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial TypeInfoHandle siplus_type_bool();
        [LibraryImport(LIBNAME, EntryPoint = "siplus_type_array", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial TypeInfoHandle siplus_type_array();
        [LibraryImport(LIBNAME, EntryPoint = "siplus_type_null", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial TypeInfoHandle siplus_type_null();



        [DllImport(LIBNAME)]
        internal static unsafe extern int siplus_converter_new(
            out ConverterHandle converter, IntPtr data, 
            delegate* unmanaged[Cdecl]<nint, nint, nint, nint, int> can,
            delegate* unmanaged[Cdecl]<nint*, nint, nint, nint, int> impl,
            delegate* unmanaged[Cdecl]<nint, void> deleter);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_converter_can_convert", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial nint siplus_converter_data_ptr(ConverterHandle handle);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_converter_can_convert", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_converter_can_convert(
            [MarshalAs(UnmanagedType.U4)] out bool result, 
            ConverterHandle converter, TypeInfoHandle from, TypeInfoHandle to);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_converter_convert", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_converter_convert(
            out DataContainerHandle result, 
            ConverterHandle converter, DataContainerHandle from, TypeInfoHandle to);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_converter_unref", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial void siplus_converter_unref(IntPtr converter);



        [DllImport(LIBNAME)]
        internal static unsafe extern int siplus_iterator_new(
            out IteratorHandle iterator, IntPtr data,
            delegate* unmanaged[Cdecl]<nint, int> more, 
            delegate* unmanaged[Cdecl]<nint, int> next,
            delegate* unmanaged[Cdecl]<nint*, nint, int> current,
            delegate* unmanaged[Cdecl]<nint, void> deleter);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_iterator_can_convert", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial nint siplus_iterator_data_ptr(IteratorHandle handle);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_iterator_next", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_iterator_next(IteratorHandle iterator);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_iterator_more", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_iterator_more([MarshalAs(UnmanagedType.I4)] out bool result, IteratorHandle iterator);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_iterator_current", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_iterator_current(out DataContainerHandle result, IteratorHandle iterator);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_iterator_delete", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial void siplus_iterator_delete(IntPtr iterator);




        [LibraryImport(LIBNAME, EntryPoint = "siplus_data_make_int", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial DataContainerHandle siplus_data_make_int(long value);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_data_make_float", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial DataContainerHandle siplus_data_make_float(double value);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_data_make_string", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial DataContainerHandle siplus_data_make_string(string text);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_data_make_bool", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial DataContainerHandle siplus_data_make_bool(int value);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_data_make_null", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial DataContainerHandle siplus_data_make_null();

        [DllImport(LIBNAME)]
        internal static unsafe extern DataContainerHandle siplus_data_make(
            TypeInfoHandle type, IntPtr data, 
            delegate* unmanaged[Cdecl]<nint, void> deleter);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_data_is_int", StringMarshalling = StringMarshalling.Utf8)]
        [return:MarshalAs(UnmanagedType.I4)] 
        internal static unsafe partial bool siplus_data_is_int(DataContainerHandle container);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_data_is_float", StringMarshalling = StringMarshalling.Utf8)]
        [return:MarshalAs(UnmanagedType.I4)] 
        internal static unsafe partial bool siplus_data_is_float(DataContainerHandle container);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_data_is_string", StringMarshalling = StringMarshalling.Utf8)]
        [return:MarshalAs(UnmanagedType.I4)] 
        internal static unsafe partial bool siplus_data_is_string(DataContainerHandle container);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_data_is_bool", StringMarshalling = StringMarshalling.Utf8)]
        [return:MarshalAs(UnmanagedType.I4)] 
        internal static unsafe partial bool siplus_data_is_bool(DataContainerHandle container);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_data_is_array", StringMarshalling = StringMarshalling.Utf8)]
        [return:MarshalAs(UnmanagedType.I4)] 
        internal static unsafe partial bool siplus_data_is_array(DataContainerHandle container);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_data_is_null", StringMarshalling = StringMarshalling.Utf8)]
        [return: MarshalAs(UnmanagedType.I4)] 
        internal static unsafe partial bool siplus_data_is_null(DataContainerHandle container);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_data_is", StringMarshalling = StringMarshalling.Utf8)]
        [return:MarshalAs(UnmanagedType.I4)] 
        internal static unsafe partial bool siplus_data_is(DataContainerHandle container, TypeInfoHandle typeInfo);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_data_as_int", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_data_as_int(out int val, DataContainerHandle container);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_data_as_float", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_data_as_float(out double val, DataContainerHandle container);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_data_as_string", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_data_as_string(out StringHandle val, DataContainerHandle container);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_data_as_bool", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_data_as_bool(out int val, DataContainerHandle container);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_data_ptr", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_data_ptr(out IntPtr ptr, DataContainerHandle container);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_data_type", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_data_type(out TypeInfoHandle ptr, DataContainerHandle container);

        [LibraryImport(LIBNAME, EntryPoint = "siplus_data_delete", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial void siplus_data_delete(IntPtr container);



        internal abstract class BaseHandle : SafeHandleZeroOrMinusOneIsInvalid {
            private bool _shouldDelete = true;

            public BaseHandle() : base(true) { }
            public BaseHandle(bool ownsHandle) : base(ownsHandle) { }

            /// <summary>
            /// Mark this as not needing cleanup. Don't actually delete the managed object.
            /// Useful for when this handle has peen passed to another owner.
            /// </summary>
            public void DangerousReleaseHandle() {
                _shouldDelete = false;
            }

            protected override bool ReleaseHandle() {
                if (_shouldDelete && !IsInvalid && !IsClosed) {
                    return Release();
                } else {
                    return true;
                }
            }

            protected abstract bool Release();
        }

        internal class ParserHandle : BaseHandle {
            public ParserHandle() : base(true) { }

            protected override bool Release() {
                siplus_parser_delete(handle);
                return true;
            }
        }

        internal class ContextHandle : BaseHandle {
            public ContextHandle() : base(true) { }
            public ContextHandle(nint ptr) : base(true) {
                SetHandle(ptr);
            }
            public ContextHandle(nint ptr, bool ownsHandle) : base(ownsHandle) {
                SetHandle(ptr);
            }

            protected override bool Release() {
                siplus_context_unref(handle);
                return true;
            }
        }

        internal class TextConstructorHandle : BaseHandle {
            public TextConstructorHandle() : base(true) { }

            protected override bool Release() {
                siplus_text_unref(handle);
                return true;
            }
        }

        internal class FunctionHandle : BaseHandle {
            public FunctionHandle() : base(true) { }
            public FunctionHandle(IntPtr handle) : base(true) { SetHandle(handle); }

            protected override bool Release() {
                //siplus_value_delete(handle);
                return true;
            }
        }

        internal class ValueRetrieverHandle : BaseHandle {
            public ValueRetrieverHandle() : base(true) { }
            public ValueRetrieverHandle(IntPtr handle) : base(true) { SetHandle(handle); }

            protected override bool Release() {
                siplus_value_unref(handle);
                return true;
            }
        }

        internal class ParseOptsHandle : BaseHandle {
            public ParseOptsHandle() : base(true) { }

            protected override bool Release() {
                siplus_parse_opts_delete(handle);
                return true;
            }
        }

        internal class DataContainerHandle : BaseHandle {
            public DataContainerHandle() : base(true) { }
            public DataContainerHandle(IntPtr ptr) : base(true) {
                SetHandle(ptr);
            }
            public DataContainerHandle(IntPtr ptr, bool ownsHandle) : base(ownsHandle) {
                SetHandle(ptr);
            }

            protected override bool Release() {
                siplus_data_delete(handle);
                return true;
            }
        }

        internal class InvocationContextBuilderHandle : BaseHandle {
            public InvocationContextBuilderHandle() : base(true) { }
            public InvocationContextBuilderHandle(IntPtr ptr) : base(true) {
                SetHandle(ptr);
            }

            protected override bool Release() {
                siplus_icbuilder_delete(handle);
                return true;
            }
        }

        internal class InvocationContextHandle : BaseHandle {
            public InvocationContextHandle() : base(true) { }
            public InvocationContextHandle(IntPtr ptr) : base(true) {
                SetHandle(ptr);
            }


            protected override bool Release() {
                siplus_invocation_unref(handle);
                return true;
            }
        }

        internal class TypeInfoHandle : BaseHandle {
            public TypeInfoHandle() : base(true) { }
            public TypeInfoHandle(IntPtr ptr) : base(true) {
                SetHandle(ptr);
            }

            protected override bool Release() {
                siplus_type_unref(handle);
                return true;
            }
        }

        internal class ConverterHandle : BaseHandle {
            public ConverterHandle() : base(true) { }
            public ConverterHandle(IntPtr ptr) : base(true) {
                SetHandle(ptr);
            }

            protected override bool Release() {
                siplus_context_unref(handle);
                return true;
            }
        }

        internal class IteratorHandle : BaseHandle {
            public IteratorHandle() : base(true) { }

            protected override bool Release() {
                siplus_iterator_delete(handle);
                return true;
            }
        }

        internal class StringHandle : BaseHandle {
            public StringHandle() : base(true) { }

            public string? Value => Marshal.PtrToStringAnsi(handle);

            protected override bool Release() {
                siplus_string_delete(handle);
                return true;
            }
        }

    }
}

