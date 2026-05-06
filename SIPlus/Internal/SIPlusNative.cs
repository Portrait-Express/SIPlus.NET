using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SIPlus
{
    internal static partial class SIPlusNative
    {
        internal enum Errors { 
            SIPLUS_OK = 0,
            SIPLUS_ERR = 1,
            SIPLUS_INVALID_ARG = 2,
            SIPLUS_PARSE_ERROR = 3,
            SIPLUS_INVOKE_ERROR = 4
        }

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
                if(_shouldDelete && !IsInvalid && !IsClosed) {
                    return Release();
                } else {
                    return true;
                }
            }

            protected abstract bool Release();
        }

        internal class ParserHandle : BaseHandle
        {
            public ParserHandle() : base(true) { }

            protected override bool Release() {
                siplus_parser_delete(handle);
                return true;
            }
        }

        internal class ContextHandle : BaseHandle {
            public ContextHandle() : base(true) { }

            protected override bool Release()
            {
                siplus_context_unref(handle);
                return true;
            }
        }

        internal class TextConstructorHandle : BaseHandle {
            public TextConstructorHandle() : base(true) { }

            protected override bool Release()
            {
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

            protected override bool Release()
            {
                siplus_value_unref(handle);
                return true;
            }
        }

        internal class ParseOptsHandle : BaseHandle {
            public ParseOptsHandle() : base(true) { }

            protected override bool Release()
            {
                siplus_parse_opts_delete(handle);
                return true;
            }
        }

        internal class DataContainerHandle : BaseHandle {
            public DataContainerHandle() : base(true) { }
            public DataContainerHandle(IntPtr ptr) : base(true) {
                SetHandle(ptr);
            }

            protected override bool Release()
            {
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


            protected override bool Release()
            {
                siplus_invocation_unref(handle);
                return true;
            }
        }

        internal class TypeInfoHandle : BaseHandle {
            public TypeInfoHandle() : base(true) { }

            protected override bool Release()
            {
                siplus_type_unref(handle);
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

            protected override bool Release()
            {
                siplus_string_delete(handle);
                return true;
            }
        }

        internal unsafe delegate void   SIPlusFunctionDeleter(IntPtr data);
        internal unsafe delegate int    SIPlusFunctionValue(IntPtr* result, IntPtr thisData, IntPtr parent, int paramc, IntPtr* paramv);


        internal unsafe delegate void   SIPlusRetrieverDeleter(IntPtr data);
        internal unsafe delegate int    SIPlusRetrieverImpl(IntPtr* result, IntPtr data, IntPtr context);

        internal unsafe delegate void   SIPlusUnknownDataContainerDeleter(IntPtr data);

        internal unsafe delegate int    SIPlusIteratorMore(IntPtr data);
        internal unsafe delegate int    SIPlusIteratorNext(IntPtr data);
        internal unsafe delegate int    SIPlusIteratorCurrent(IntPtr* result, IntPtr data);
        internal unsafe delegate void   SIPlusIteratorDeleter(IntPtr data);

        internal unsafe delegate int    SIPlusTypeIsIterable(IntPtr data, IntPtr container);
        internal unsafe delegate int    SIPlusTypeIterate(IntPtr* result, IntPtr data, IntPtr container);
        internal unsafe delegate int    SIPlusTypeAccess(IntPtr* result, IntPtr data, IntPtr container, string name);
        internal unsafe delegate void   SIPlusTypeDeleter(IntPtr data);


        [LibraryImport("siplus.dll", EntryPoint = "siplus_error_get", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_error_get(out StringHandle message);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_error_set", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_error_set(int err, string message);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_string_delete", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial void siplus_string_delete(IntPtr str);



        [LibraryImport("siplus.dll", EntryPoint = "siplus_parser_new", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial ParserHandle siplus_parser_new();

        [LibraryImport("siplus.dll", EntryPoint = "siplus_parser_context", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_parser_context(out ContextHandle context, ParserHandle parser);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_parser_get_interpolation", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_parser_get_interpolation(out TextConstructorHandle constructor, ParserHandle parser, string text, ParseOptsHandle opts);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_parser_get_expression", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_parser_get_expression(out ValueRetrieverHandle retriever, ParserHandle parser, string expr, ParseOptsHandle opts);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_parser_delete", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial void siplus_parser_delete(IntPtr parser);




        [LibraryImport("siplus.dll", EntryPoint = "siplus_value_create", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_value_create(out ValueRetrieverHandle retriever, IntPtr context, SIPlusRetrieverImpl impl, SIPlusRetrieverDeleter deleter);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_value_retrieve", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_value_retrieve(out DataContainerHandle data, ValueRetrieverHandle value, InvocationContextHandle context);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_value_unref", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial void siplus_value_unref(IntPtr parser);




        [LibraryImport("siplus.dll", EntryPoint = "siplus_text_construct", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_text_construct(out StringHandle text, TextConstructorHandle value, InvocationContextHandle context);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_text_unref", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial void siplus_text_unref(IntPtr parser);




        [LibraryImport("siplus.dll", EntryPoint = "siplus_parse_opts_new", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial ParseOptsHandle siplus_parse_opts_new();

        [LibraryImport("siplus.dll", EntryPoint = "siplus_parse_opts_add_global", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_parse_opts_add_global(ParseOptsHandle opts, string name);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_parse_opts_delete", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial void siplus_parse_opts_delete(IntPtr opts);




        [LibraryImport("siplus.dll", EntryPoint = "siplus_context_add_function", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_context_add_function(ContextHandle context, string name, FunctionHandle handle);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_context_use_stl", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_context_use_stl(ContextHandle context);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_context_builder", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_context_builder(out InvocationContextBuilderHandle builder, ContextHandle context);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_context_unref", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial void siplus_context_unref(IntPtr context);



        [LibraryImport("siplus.dll", EntryPoint = "siplus_function_create", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_function_create(out FunctionHandle function, IntPtr data, SIPlusFunctionValue value, SIPlusFunctionDeleter deleter);
        
        [LibraryImport("siplus.dll", EntryPoint = "siplus_function_value", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_function_value(out ValueRetrieverHandle retriever, FunctionHandle function, IntPtr parent, int paramc, IntPtr[] parameters);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_function_unref", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial void siplus_function_unref(IntPtr function);





        [LibraryImport("siplus.dll", EntryPoint = "siplus_icbuilder_with", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_icbuilder_with(InvocationContextBuilderHandle builder, string name, DataContainerHandle container);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_icbuilder_default", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_icbuilder_default(InvocationContextBuilderHandle builder, DataContainerHandle container);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_icbuilder_build", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_icbuilder_build(out InvocationContextHandle context, InvocationContextBuilderHandle builder);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_icbuilder_delete", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_icbuilder_delete(IntPtr builder);
       



        [LibraryImport("siplus.dll", EntryPoint = "siplus_invocation_unref", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial void siplus_invocation_unref(IntPtr context);




        [LibraryImport("siplus.dll", EntryPoint = "siplus_type_new", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_type_new(
            out TypeInfoHandle type, IntPtr data, string name,
            SIPlusTypeIsIterable is_iterable, SIPlusTypeAccess access,
            SIPlusTypeIterate iterate, SIPlusTypeDeleter deleter);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_type_name", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_type_name(out StringHandle ptr, TypeInfoHandle typeInfo);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_type_access", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_type_access(out DataContainerHandle result, TypeInfoHandle type, DataContainerHandle data, string property);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_type_is_iterable", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_type_is_iterable([MarshalAs(UnmanagedType.I4)] out bool result, TypeInfoHandle info, DataContainerHandle data);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_type_iterate", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_type_iterate(out IteratorHandle result, TypeInfoHandle typeInfo, DataContainerHandle container);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_type_unref", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial void siplus_type_unref(IntPtr type);




        [LibraryImport("siplus.dll", EntryPoint = "siplus_iterator_new", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_iterator_new(
            out IteratorHandle iterator, IntPtr data,
            SIPlusIteratorMore more, SIPlusIteratorNext next,
            SIPlusIteratorCurrent current, SIPlusIteratorDeleter deleter);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_iterator_next", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_iterator_next(IteratorHandle iterator);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_iterator_more", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_iterator_more([MarshalAs(UnmanagedType.I4)] out bool result, IteratorHandle iterator);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_iterator_current", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_iterator_current(out DataContainerHandle result, IteratorHandle iterator);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_iterator_delete", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial void siplus_iterator_delete(IntPtr iterator);




        [LibraryImport("siplus.dll", EntryPoint = "siplus_data_make_int", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial DataContainerHandle siplus_data_make_int(long value);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_data_make_float", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial DataContainerHandle siplus_data_make_float(double value);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_data_make_string", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial DataContainerHandle siplus_data_make_string(string text);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_data_make_bool", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial DataContainerHandle siplus_data_make_bool(int value);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_data_make", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial DataContainerHandle siplus_data_make(TypeInfoHandle type, IntPtr data, SIPlusUnknownDataContainerDeleter deleter);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_data_is_int", StringMarshalling = StringMarshalling.Utf8)]
        [return:MarshalAs(UnmanagedType.I4)] internal static unsafe partial bool siplus_data_is_int(DataContainerHandle container);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_data_is_float", StringMarshalling = StringMarshalling.Utf8)]
        [return:MarshalAs(UnmanagedType.I4)] internal static unsafe partial bool siplus_data_is_float(DataContainerHandle container);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_data_is_string", StringMarshalling = StringMarshalling.Utf8)]
        [return:MarshalAs(UnmanagedType.I4)] internal static unsafe partial bool siplus_data_is_string(DataContainerHandle container);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_data_is_bool", StringMarshalling = StringMarshalling.Utf8)]
        [return:MarshalAs(UnmanagedType.I4)] internal static unsafe partial bool siplus_data_is_bool(DataContainerHandle container);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_data_is_array", StringMarshalling = StringMarshalling.Utf8)]
        [return:MarshalAs(UnmanagedType.I4)] internal static unsafe partial bool siplus_data_is_array(DataContainerHandle container);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_data_is", StringMarshalling = StringMarshalling.Utf8)]
        [return:MarshalAs(UnmanagedType.I4)] internal static unsafe partial bool siplus_data_is(DataContainerHandle container, TypeInfoHandle typeInfo);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_data_as_int", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_data_as_int(out int val, DataContainerHandle container);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_data_as_float", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_data_as_float(out double val, DataContainerHandle container);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_data_as_string", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_data_as_string(out StringHandle val, DataContainerHandle container);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_data_as_bool", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_data_as_bool(out int val, DataContainerHandle container);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_data_ptr", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_data_ptr(out IntPtr ptr, DataContainerHandle container);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_data_type", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial int siplus_data_type(out TypeInfoHandle ptr, DataContainerHandle container);

        [LibraryImport("siplus.dll", EntryPoint = "siplus_data_delete", StringMarshalling = StringMarshalling.Utf8)]
        internal static unsafe partial void siplus_data_delete(IntPtr container);
    }
}

