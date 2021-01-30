using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Microsoft.AspNetCore.Razor.SourceGenerators
{
    public class RazorSourceGeneratorTest
    {
        [Fact]
        public void CanProcessRazorFiles()
        {
            // Arrange
            var razorFile = new TestAdditionalText("TestComponent.razor", @"Hello world!");
            Compilation inputCompilation = CSharpCompilation.Create("TestCompilation");
            RazorSourceGenerator generator = new RazorSourceGenerator();

            var configOptions = new TestCompilerAnalyzerConfigOptions(
                new Dictionary<string, string> {
                    { "build_metadata.AdditionalFiles.TargetPath", "Foo.g.cs" }
                }
            .ToImmutableDictionary());
            var itemConfig = new Dictionary<object, TestCompilerAnalyzerConfigOptions> { { razorFile, configOptions } }.ToImmutableDictionary();

            var analyzerConfigOptionsProvider = new TestCompilerAnalyzerConfigOptionsProvider(itemConfig);

            GeneratorDriver driver = CSharpGeneratorDriver.Create(
                generators: ImmutableArray.Create(generator),
                additionalTexts: ImmutableArray.Create<AdditionalText>(razorFile),
                optionsProvider: analyzerConfigOptionsProvider
            );

            // Assert
            driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);

            Assert.Empty(diagnostics);
        }

    }
}
