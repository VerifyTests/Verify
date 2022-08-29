// ReSharper disable ArrangeTypeModifiers

internal delegate (string receivedPrefix, string verifiedPrefix, string? directory) GetFileConvention(string uniquenessReceived, string uniquenessVerified);