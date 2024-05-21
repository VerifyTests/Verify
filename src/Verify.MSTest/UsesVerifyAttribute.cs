namespace VerifyMSTest;

// Define the marker attribute in the main project rather than emit it as part of the source generator to
// avoid issues where the user ends up with multiple conflicting definitions of the attribute
// (commonly when using InternalsVisibleTo).

[AttributeUsage(AttributeTargets.Class)]
public sealed class UsesVerifyAttribute : Attribute;