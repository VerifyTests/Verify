using System;
using System.Collections.Generic;

// Non-nullable field is uninitialized.
#pragma warning disable CS8618
public class Person
{
    public string? GivenNames;
    public string FamilyName;
    public string Spouse;
    public Address Address;
    public List<string> Children;
    public Title Title;
    public Guid Id;
    public DateTimeOffset Dob;
}

public class Address
{
    public string Street;
    public string Suburb;
    public string Country;
}

public enum Title
{
    Mr, Mrs
}