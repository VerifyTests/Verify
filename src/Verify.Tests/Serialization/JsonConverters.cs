public class JsonConverters
{
    void AddConverter() =>

        #region JsonConverter

        VerifierSettings.AddExtraSettings(_ => _.Converters.Add(new CompanyConverter()));

    #endregion


    #region CompanyConverter

    class CompanyConverter :
        WriteOnlyJsonConverter<Company>
    {
        bool ignoreEmployees;

        public CompanyConverter()
        {
        }

        public CompanyConverter(bool ignoreEmployees) =>
            this.ignoreEmployees = ignoreEmployees;

        public override void Write(VerifyJsonWriter writer, Company company)
        {
            writer.WriteMember(company, company.Name, "Name");

            if (!ignoreEmployees)
            {
                if (writer.Context.TryGetValue("IgnoreCompanyEmployees", out var value))
                {
                    if (value is true)
                    {
                        return;
                    }
                }

                writer.WriteMember(company, company.Employees, "Employees");
            }
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

    #region TestJsonConverterWithSettings

    [Fact]
    public Task TestWithSettings()
    {
        var company = new Company(
            "Company Name",
            Employees:
            [
                "Employee1",
                "Employee2"
            ]);
        var settings = new VerifySettings();
        settings.IgnoreCompanyEmployees();
        var result = WriteOnlyJsonConverter.Execute<CompanyConverter>(company, settings);
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
        var converter = new CompanyConverter(ignoreEmployees: true);
        var result = WriteOnlyJsonConverter.Execute(converter, company);
        return VerifyJson(result);
    }

    #endregion
}

public static class CompanyConverterSettings
{
    public static void IgnoreCompanyEmployees(this VerifySettings settings) =>
        settings.Context["IgnoreCompanyEmployees"] = true;

    public static SettingsTask IgnoreCompanyEmployees(this SettingsTask settings)
    {
        settings.CurrentSettings.IgnoreCompanyEmployees();
        return settings;
    }
}