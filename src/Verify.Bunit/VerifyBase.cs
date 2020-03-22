using System.Runtime.CompilerServices;
using Bunit;
using Verify;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace VerifyBunit
{
    public partial class VerifyBase :
        ComponentTestFixture
    {
        static VerifyBase()
        {
            InnerVerifier.Init(
                message => new XunitException(message),
                input => XunitContext.Context.IntOrNext(input),
                input => XunitContext.Context.IntOrNext(input),
                input => XunitContext.Context.IntOrNext(input));
            SharedVerifySettings.RegisterFileConverter<IRenderedFragment>("html", FragmentToStream.Convert);
            SharedVerifySettings.ModifySerialization(settings =>
            {
                settings.AddExtraSettings(serializerSettings =>
                {
                    var converters = serializerSettings.Converters;
                    converters.Add(new RenderedFragmentConverter());
                });
            });
        }

        public VerifyBase(
            ITestOutputHelper output,
            [CallerFilePath] string sourceFile = "")
        {
            XunitContext.Register(output, sourceFile);
        }

        InnerVerifier GetVerifier()
        {
            var context = XunitContext.Context;
            return new InnerVerifier(context.TestType, context.SourceDirectory, context.UniqueTestName);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            XunitContext.Flush();
        }
    }
}