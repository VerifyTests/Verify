using Microsoft.CodeAnalysis;

namespace VerifyMSTest.SourceGenerator.Tests;

readonly record struct GeneratorDriverResults(GeneratorDriverResult FirstRun, GeneratorDriverResult CachedRun);

readonly record struct GeneratorDriverResult(GeneratorDriverRunResult RunResult, GeneratorDriverTimingInfo TimingInfo);