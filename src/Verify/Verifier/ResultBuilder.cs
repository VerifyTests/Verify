using System;
using System.Threading.Tasks;
using VerifyTests;

readonly struct ResultBuilder
{
    public string Extension { get; }
    public Func<FilePair, Task<EqualityResult>> GetResult { get; }

    public ResultBuilder(string extension, Func<FilePair, Task<EqualityResult>> getResult)
    {
        Extension = extension;
        GetResult = getResult;
    }
}