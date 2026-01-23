#define _DISABLE_CONSTEXPR_MUTEX_CONSTRUCTOR

#include <msclr/marshal.h>
#include <msclr/marshal_cppstd.h>
#include <iostream>
#include "siplus.h"

#define UDTC_DATA_CAST(object, type, check, dst) \
	if(type == check::typeid) \
		return SIPlusCpp::text::make_data(ctx.marshal_as<dst>(safe_cast<check^>(object)));
#define UDTC_DATA_CAST_V(object, type, check, dst) \
	if(type == check::typeid) \
		return SIPlusCpp::text::make_data(safe_cast<check>(object));

SIPlusCpp::text::UnknownDataTypeContainer MakeData(System::Object^ object) {
	msclr::interop::marshal_context ctx;
	auto type = object->GetType();

	UDTC_DATA_CAST(object, type, System::String, std::string);
	UDTC_DATA_CAST_V(object, type, System::Char, long);
	UDTC_DATA_CAST_V(object, type, System::Byte, long);
	UDTC_DATA_CAST_V(object, type, System::Int16, long);
	UDTC_DATA_CAST_V(object, type, System::UInt16, long);
	UDTC_DATA_CAST_V(object, type, System::Int32, long);
	UDTC_DATA_CAST_V(object, type, System::UInt32, long);
	UDTC_DATA_CAST_V(object, type, System::Int64, long);
	UDTC_DATA_CAST_V(object, type, System::UInt64, long);
	UDTC_DATA_CAST_V(object, type, System::Single, double);
	UDTC_DATA_CAST_V(object, type, System::Double, double);
	
	gcroot<System::Object^> root = object;
	return SIPlusCpp::text::make_data(root);
}

System::Object^ ToObject(const SIPlusCpp::text::UnknownDataTypeContainer& container) {
	if (container.is<gcroot<System::Object^>>()) {
		return container.as<gcroot<System::Object^>>();
	}
	else if (SIPlusCpp::text::is_numeric(container.type)) {
		if (container.is<double>()) {
			return container.as<double>();
		}
		else if (container.is<float>()) {
			return container.as<float>();
		}
		else if (container.is<float>()) {
			return container.as<float>();
		}
		else if (container.is<float>()) {
			return container.as<float>();
		}
		else if (container.is<float>()) {
			return container.as<float>();
		}
		else {
			throw std::runtime_error{
				SIPlusCpp::util::to_string(
					"Unsupported numeric type ", SIPlusCpp::text::get_type_name(container.type)
				)
			};
		}
	}
	else if (container.is<std::string>()) {
		return gcnew System::String(container.as<std::string>().c_str());
	}
	else if(container.is<std::vector<SIPlusCpp::text::UnknownDataTypeContainer>>()) {
		auto list = gcnew System::Collections::Generic::List<System::Object^>(0);
		auto& vec = container.as<std::vector<SIPlusCpp::text::UnknownDataTypeContainer>>();

		for (auto& item : vec) {
			list->Add(ToObject(item));
		}

		return list;
	}

	throw gcnew System::InvalidCastException(
		System::String::Format(
			"Could not marshal {} back into System.Object",
			gcnew System::String(SIPlusCpp::text::get_type_name(container.type).c_str())
		)
	);
}

namespace SIPlusCpp {

class CSharpObjectAccessor : public text::Accessor {
	virtual text::UnknownDataTypeContainer access(const text::UnknownDataTypeContainer& value, const std::string& name) override;
	virtual bool can_access(const text::UnknownDataTypeContainer& value) override;
};

text::UnknownDataTypeContainer
CSharpObjectAccessor::access(const text::UnknownDataTypeContainer& value, const std::string& name) {
	System::String^ strName = gcnew System::String(name.c_str());
	System::Object^ object = value.as<gcroot<System::Object^>>();
	
	auto type = object->GetType();
	auto prop = type->GetProperty(strName);
	if (prop != nullptr) {
		return MakeData(prop->GetValue(object));
	}

	auto member = type->GetField(strName);
	if (member != nullptr) {
		return MakeData(member->GetValue(object));
	}

	msclr::interop::marshal_context ctx;
	throw std::runtime_error{
		util::to_string(
			"Unknown property/field name ", name, " on object of type ",
			ctx.marshal_as<std::string>(type->FullName)
		)
	};
}

bool CSharpObjectAccessor::can_access(const text::UnknownDataTypeContainer& value) {
	return value.is<gcroot<System::Object^>>();
}

class IEnumerableIterator : public text::Iterator {
public:
	IEnumerableIterator(System::Collections::IEnumerator^ enumerator);

	bool more() override;
	void next() override;
	text::UnknownDataTypeContainer current() override;

private:
	bool more_;
	text::UnknownDataTypeContainer current_ = MakeData(nullptr);
	text::UnknownDataTypeContainer between_ = MakeData(nullptr);
	gcroot<System::Collections::IEnumerator^> enumerator_;
};

IEnumerableIterator::IEnumerableIterator(System::Collections::IEnumerator^ enumerator) {
	enumerator_ = enumerator;
	more_ = enumerator_->MoveNext();
	if (more_) {
		between_ = MakeData(enumerator->Current);
	}
}

bool IEnumerableIterator::more() {
	return more_;
}

void IEnumerableIterator::next() {
	if (!more_) {
		throw std::runtime_error{"next() called after iterator finished"};
	}

	current_ = between_;
	more_ = enumerator_->MoveNext();
	if (more_) {
		between_ = MakeData(enumerator_->Current);
	}
}

text::UnknownDataTypeContainer IEnumerableIterator::current() {
	return current_;
}


class IEnumerableIteratorProvider : public text::IteratorProvider {
public:
	std::unique_ptr<text::Iterator> iterator(const text::UnknownDataTypeContainer& value) override;
	bool can_iterate(const text::UnknownDataTypeContainer& value) override;
};

std::unique_ptr<text::Iterator> IEnumerableIteratorProvider::iterator(const text::UnknownDataTypeContainer& container) {
	System::Object^ value = container.as<gcroot<System::Object^>>();
	auto enumerable = safe_cast<System::Collections::IEnumerable^>(value);

	return std::unique_ptr<text::Iterator>{
		new IEnumerableIterator(enumerable->GetEnumerator())
	};
}

bool IEnumerableIteratorProvider::can_iterate(const text::UnknownDataTypeContainer& container) {
	if (!container.is<gcroot<System::Object^>>()) { return false; }
	auto type = container.as<gcroot<System::Object^>>()->GetType();
	return System::Collections::IEnumerable::typeid->IsAssignableFrom(type);
}

} /* SIPlusCpp */

namespace SIPlus {

SIPlusCpp::ParseOpts Convert(ParseOpts^ opt) {
	SIPlusCpp::ParseOpts opts;

	if (opt == nullptr) {
		return opts;
	}

	msclr::interop::marshal_context ctx;

	auto i = opt->Globals->GetEnumerator();
	while (i->MoveNext()) {
		opts.globals.push_back(ctx.marshal_as<std::string>(i->Current));
	}

	return opts;
}

namespace Text {

public ref class NativeValueRetrieverWrapper : IValueRetriever {
internal:
	NativeValueRetrieverWrapper(std::shared_ptr<SIPlusCpp::text::ValueRetriever> other);
	NativeValueRetrieverWrapper^ operator=(std::shared_ptr<SIPlusCpp::text::ValueRetriever> other);

public:
	virtual System::Object^ Retrieve(InvocationContext^ context);

	~NativeValueRetrieverWrapper();

private:
	std::shared_ptr<SIPlusCpp::text::ValueRetriever>* retriever_;
};

NativeValueRetrieverWrapper::NativeValueRetrieverWrapper(std::shared_ptr<SIPlusCpp::text::ValueRetriever> other) {
	retriever_ = new std::shared_ptr<SIPlusCpp::text::ValueRetriever>(other);
}

NativeValueRetrieverWrapper^ NativeValueRetrieverWrapper::operator=(std::shared_ptr<SIPlusCpp::text::ValueRetriever> retriever) {
	*retriever_ = retriever;
	return this;
}

NativeValueRetrieverWrapper::~NativeValueRetrieverWrapper() {
	delete retriever_;
}

System::Object^ NativeValueRetrieverWrapper::Retrieve(InvocationContext^ context) {
	try {
		return ToObject((*retriever_)->retrieve(**context->context_));
	} catch (std::runtime_error& e) {
		throw gcnew SIPlusException(gcnew System::String(e.what()));
	}
}

class ManagedValueRetrieverWrapper : public SIPlusCpp::text::ValueRetriever {
public:
	ManagedValueRetrieverWrapper(IValueRetriever^ retriever);

	SIPlusCpp::text::UnknownDataTypeContainer retrieve(SIPlusCpp::InvocationContext& value) const override;

private:
	gcroot<IValueRetriever^> retriever_;
};

ManagedValueRetrieverWrapper::ManagedValueRetrieverWrapper(IValueRetriever^ retriever) : retriever_(retriever) {}

SIPlusCpp::text::UnknownDataTypeContainer ManagedValueRetrieverWrapper::retrieve(SIPlusCpp::InvocationContext& value) const {
	auto ctx = gcnew InvocationContext(value.shared_from_this());
	return MakeData(retriever_->Retrieve(ctx));
}

class ManagedFunctionWrapper : public SIPlusCpp::Function {
public:
	ManagedFunctionWrapper(IFunction^ function);

	std::shared_ptr<SIPlusCpp::text::ValueRetriever> value(
		std::shared_ptr<SIPlusCpp::text::ValueRetriever> parent,
		std::vector<std::shared_ptr<SIPlusCpp::text::ValueRetriever>> parameters
	) const override;

private:
	gcroot<IFunction^> function_;
};

ManagedFunctionWrapper::ManagedFunctionWrapper(IFunction^ function) : function_(function) {}

std::shared_ptr<SIPlusCpp::text::ValueRetriever>
ManagedFunctionWrapper::value(
	std::shared_ptr<SIPlusCpp::text::ValueRetriever> parent,
	std::vector<std::shared_ptr<SIPlusCpp::text::ValueRetriever>> parameters
) const {
	IValueRetriever^ mParent = nullptr;
	array<IValueRetriever^>^ mParameters = gcnew array<IValueRetriever^>(parameters.size());

	if(parent) {
		mParent = gcnew NativeValueRetrieverWrapper(parent);
	}

	for (int i = 0; i < parameters.size(); i++) {
		mParameters[i] = gcnew NativeValueRetrieverWrapper(parameters[i]);
	}

	return std::shared_ptr<SIPlusCpp::text::ValueRetriever>{
		new ManagedValueRetrieverWrapper(function_->Value(mParent, mParameters))
	};
}

public ref class TextConstructor : ITextConstructor {
internal:
	TextConstructor(SIPlusCpp::text::TextConstructor constructor);
	TextConstructor^ operator=(SIPlusCpp::text::TextConstructor constructor);

public:
	virtual System::String^ Construct(InvocationContext^ context);

	~TextConstructor();

private:
	SIPlusCpp::text::TextConstructor* constructor_;
};

TextConstructor::TextConstructor(SIPlusCpp::text::TextConstructor other) {
	constructor_ = new SIPlusCpp::text::TextConstructor(other);
}

TextConstructor^ TextConstructor::operator=(SIPlusCpp::text::TextConstructor other) {
	*constructor_ = other;
	return this;
}

TextConstructor::~TextConstructor() {
	delete constructor_;
}

System::String^ TextConstructor::Construct(InvocationContext^ context) {
	try {
		auto result = constructor_->construct_with(*context->context_);
		return gcnew System::String(result.c_str());
	} catch (std::runtime_error& e) {
		throw gcnew SIPlusException(gcnew System::String(e.what()));
	}
}

} /* Text */


InvocationContext::InvocationContext(std::shared_ptr<SIPlusCpp::InvocationContext> context) {
	context_ = new std::shared_ptr<SIPlusCpp::InvocationContext>(context);
}

InvocationContext^ InvocationContext::operator=(std::shared_ptr<SIPlusCpp::InvocationContext> context) {
	*context_ = context;
	return this;
}

InvocationContext::~InvocationContext() {
	delete context_;
}

ContextInvocationContextBuilder::ContextInvocationContextBuilder(SIPlusCpp::ContextInvocationContextBuilder other) {
	builder_ = new SIPlusCpp::ContextInvocationContextBuilder(other);
}

ContextInvocationContextBuilder^ ContextInvocationContextBuilder::operator=(SIPlusCpp::ContextInvocationContextBuilder other) {
	*builder_ = other;
	return this;
}

ContextInvocationContextBuilder::~ContextInvocationContextBuilder() {
	delete builder_;
}

ContextInvocationContextBuilder^ ContextInvocationContextBuilder::UseDefault(System::Object^ object) {
	builder_->use_default(MakeData(object));
	return this;
}

ContextInvocationContextBuilder^ ContextInvocationContextBuilder::With(System::String^ name, System::Object^ data) {
	msclr::interop::marshal_context ctx;
	builder_->with(ctx.marshal_as<std::string>(name), MakeData(data));
	return this;
}

InvocationContext^ ContextInvocationContextBuilder::Build() {
	return gcnew InvocationContext(builder_->build());
}


SIPlusParserContext::SIPlusParserContext(std::shared_ptr<SIPlusCpp::SIPlusParserContext> other) {
	context_ = new std::shared_ptr<SIPlusCpp::SIPlusParserContext>(other);
}

SIPlusParserContext^ SIPlusParserContext::operator=(std::shared_ptr<SIPlusCpp::SIPlusParserContext> other) {
	*context_ = other;
	return this;
}

SIPlusParserContext::~SIPlusParserContext() {
	delete context_;
}

#ifdef SIPLUS_INCLUDE_STDLIB
void SIPlusParserContext::UseSTL() {
	(*context_)->use_stl();
}
#endif

ContextInvocationContextBuilder^ SIPlusParserContext::Builder() {
	return gcnew ContextInvocationContextBuilder((*context_)->builder());
}

void SIPlusParserContext::AddFunction(System::String^ name, IFunction^ function) {
	msclr::interop::marshal_context mctx;

	(*context_)->emplace_function<Text::ManagedFunctionWrapper>(
		mctx.marshal_as<std::string>(name), function);
}

SIPlusParser::SIPlusParser() {
	parser_ = new SIPlusCpp::Parser();
	auto a = reinterpret_cast<std::unique_ptr<SIPlusCpp::ParserImpl>*>(parser_); // FUCKING EVIL
	std::cout << a->get() << std::endl;
	parser_->context().emplace_accessor<SIPlusCpp::CSharpObjectAccessor>();
	parser_->context().emplace_iterator<SIPlusCpp::IEnumerableIteratorProvider>();
}

SIPlusParser::~SIPlusParser() {
	delete parser_;
}

Text::ITextConstructor^ SIPlusParser::GetInterpolation(System::String^ text) {
	return GetInterpolation(text, nullptr);
}

Text::ITextConstructor^ SIPlusParser::GetInterpolation(System::String^ text, ParseOpts^ opts) {
	msclr::interop::marshal_context ctx;

	try {
		std::string marshalledText = ctx.marshal_as<std::string>(text);
		auto constructor = parser_->get_interpolation(marshalledText, Convert(opts));
		return gcnew Text::TextConstructor(constructor);
	} catch (std::runtime_error& e) {
		throw gcnew SIPlusException(gcnew System::String(e.what()));
	}

}

Text::IValueRetriever^ SIPlusParser::GetExpression(System::String^ expression) {
	return GetExpression(expression, nullptr);
}

Text::IValueRetriever^ SIPlusParser::GetExpression(System::String^ expression, ParseOpts^ opts) {
	msclr::interop::marshal_context ctx;

	try {
		return gcnew Text::NativeValueRetrieverWrapper(
			parser_->get_expression(ctx.marshal_as<std::string>(expression), Convert(opts))
		);
	} catch (std::runtime_error& e) {
		throw gcnew SIPlusException(gcnew System::String(e.what()));
	}
}

SIPlusParserContext^ SIPlusParser::Context() {
	return gcnew SIPlusParserContext(
		parser_->context().shared_from_this()
	);
}

} /* SIPlus */