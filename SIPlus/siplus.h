#pragma once
#include <siplus/siplus.hxx>

namespace SIPlus {

public ref class SIPlusException : System::Exception {
public:
    SIPlusException(System::String^ message) : System::Exception(message) {}
};

public ref class InvocationContext {
internal:
    InvocationContext(std::shared_ptr<SIPlusCpp::InvocationContext> context);
    InvocationContext^ operator=(std::shared_ptr<SIPlusCpp::InvocationContext> context);

	std::shared_ptr<SIPlusCpp::InvocationContext> *context_;

public:
    ~InvocationContext();
};

namespace Text {

public interface class IValueRetriever {
    System::Object^ Retrieve(InvocationContext^ context);
};

public interface class ITextConstructor {
    System::String^ Construct(InvocationContext^ context);
};

} /* Text */

public interface class IFunction {
    Text::IValueRetriever^ Value(
        Text::IValueRetriever^ parent,
        System::Collections::Generic::IEnumerable<Text::IValueRetriever^>^ parameters
    );
};

public ref class ContextInvocationContextBuilder {
internal:
    ContextInvocationContextBuilder(SIPlusCpp::ContextInvocationContextBuilder other);
    ContextInvocationContextBuilder^ operator=(SIPlusCpp::ContextInvocationContextBuilder other);

public:

    /**
     * @brief Set the default data for this invocation. The data accessible at `$0` or `.`.
     * Calling `with("0", data)`, is an equivalent call, but prefer this.
     *
     * @param data The data to use
     */
    ContextInvocationContextBuilder^ UseDefault(System::Object^ object);

    /**
     * @brief Add an additional variable accessible at `$name`
     *
     * @param name The name of the variable to add
     * @param data The data to use at the variable
     */
    ContextInvocationContextBuilder^ With(System::String^ name, System::Object^ data);

    /**
     * @brief Return the build `InvocationContext`
     */
    InvocationContext^ Build();

    ~ContextInvocationContextBuilder();

private:
    SIPlusCpp::ContextInvocationContextBuilder* builder_;
};

public ref class SIPlusParserContext {
internal:
	SIPlusParserContext(std::shared_ptr<SIPlusCpp::SIPlusParserContext>);
	SIPlusParserContext^ operator=(std::shared_ptr<SIPlusCpp::SIPlusParserContext>);

	std::shared_ptr<SIPlusCpp::SIPlusParserContext>* context_;

public:

#ifdef SIPLUS_INCLUDE_STDLIB
    /**
     * @brief Attaches the STL library of functions and converters, and iterators to this context.
     */
    void UseSTL();
#endif

    /**
     * @brief Create a builder to make an `InvocationContext`
     *
     * @return A builder object
     */
    ContextInvocationContextBuilder^ Builder();

    void AddFunction(System::String^ name, IFunction^ function);

    ~SIPlusParserContext();
};

public ref class ParseOpts {
public:
	System::Collections::Generic::IEnumerable<System::String^>^ Globals;
};

public ref class SIPlusParser {
public:
	SIPlusParser();

	Text::ITextConstructor^ GetInterpolation(System::String^ text);
	Text::ITextConstructor^ GetInterpolation(System::String^ text, ParseOpts^ opts);

	Text::IValueRetriever^ GetExpression(System::String^  expression);
	Text::IValueRetriever^ GetExpression(System::String^ expression, ParseOpts^ opts);


	SIPlusParserContext^ Context();

	~SIPlusParser();

private:
	SIPlusCpp::Parser* parser_;
};

}