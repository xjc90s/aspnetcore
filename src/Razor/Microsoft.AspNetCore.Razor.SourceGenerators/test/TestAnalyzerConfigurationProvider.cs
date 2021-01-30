using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.AspNetCore.Razor.SourceGenerators
{
    public class TestCompilerAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
    {
        private readonly ImmutableDictionary<object, TestCompilerAnalyzerConfigOptions> _treeDict;

        public override AnalyzerConfigOptions GlobalOptions { get; }

        internal TestCompilerAnalyzerConfigOptionsProvider(ImmutableDictionary<object, TestCompilerAnalyzerConfigOptions> treeDict)
        {
            // Set default global properties
            var builder = ImmutableDictionary.CreateBuilder<string, string>();
            builder.Add("build_property._RazorReferenceAssemblyTagHelpersOutputPath", "RazorTagHelper.refs.out.cache");
            builder.Add("build_property.RazorLangVersion", "3.0");
            var defaults = builder.ToImmutable();

            // Set item specific options
            _treeDict = treeDict;

            GlobalOptions = new TestCompilerAnalyzerConfigOptions(defaults);
        }

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
            => _treeDict.TryGetValue(tree, out var options) ? options : TestCompilerAnalyzerConfigOptions.Empty;

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
            => _treeDict.TryGetValue(textFile, out var options) ? options : TestCompilerAnalyzerConfigOptions.Empty;

    }

    internal sealed class TestCompilerAnalyzerConfigOptions : AnalyzerConfigOptions
    {
        public static TestCompilerAnalyzerConfigOptions Empty { get; } = new TestCompilerAnalyzerConfigOptions(ImmutableDictionary.Create<string, string>());

        private readonly ImmutableDictionary<string, string> _backing;

        public TestCompilerAnalyzerConfigOptions(ImmutableDictionary<string, string> properties)
        {
            _backing = properties;
        }

        public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value) => _backing.TryGetValue(key, out value);
    }
}
