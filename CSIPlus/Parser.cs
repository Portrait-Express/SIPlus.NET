using CSIPlus.Internal;
using CSIPlus.Internal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSIPlus;

public class Parser : IDisposable
{
    internal SIPlusNative.ParserHandle _parser;

    public Parser() {
        _parser = SIPlusNative.siplus_parser_new();
    }

    public ParserContext Context()
    {
        int result = SIPlusNative.siplus_parser_context(out var context, _parser);
        Util.AssertSuccess(result);
        return new(context);
    }

    public TextConstructor GetInterpolation(string text, ParseOpts? opts = null)
    {
        int result = SIPlusNative.siplus_parser_get_interpolation(
            out var handle, _parser, text, opts?.Handle ?? new ParseOpts().Handle);
        Util.AssertSuccess(result);
        return new(handle);
    }

    public IValueRetriever GetExpression(string text, ParseOpts? opts = null)
    {
        int result = SIPlusNative.siplus_parser_get_expression(
            out var handle, _parser, text, opts?.Handle ?? new ParseOpts().Handle);
        Util.AssertSuccess(result);
        return handle.FromNativeRetriever();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if(_parser != null && !_parser.IsInvalid)
        {
            _parser.Dispose();
        }
    }

    ~Parser() { 
        Dispose(false);
    }
}
