public class JsonConverters
{
    #region AddJsonConverter

    [ModuleInitializer]
    public static void Initialize() =>
        VerifierSettings.AddExtraSettings(_ => _.Converters.Add(new CompanyConverter()));

    #endregion


    #region JsonConverter

    class CompanyConverter :
        WriteOnlyJsonConverter<Company>
    {
        public override void Write(VerifyJsonWriter writer, Company company)
        {
            writer.WriteMember(company, company.Name, "Name");
            writer.WriteMember(company, company.Employees, "Employees");
        }
    }

    #endregion

    record Company(string Name, List<string> Employees);

    #region TestJsonConverter

    [Fact]
    public Task Test()
    {
        var company = new Company(
            "Company Name",
            Employees:
            [
                "Employee1",
                "Employee2"
            ]);
        var result = WriteOnlyJsonConverter.Execute<CompanyConverter>(company);
        return VerifyJson(result);
    }

    #endregion

    #region TestWithConverterInstance

    [Fact]
    public Task TestWithConverterInstance()
    {
        var company = new Company(
            "Company Name",
            Employees:
            [
                "Employee1",
                "Employee2"
            ]);
        var converter = new CompanyConverter();
        var result = WriteOnlyJsonConverter.Execute(converter, company);
        return VerifyJson(result);
    }

    #endregion
}