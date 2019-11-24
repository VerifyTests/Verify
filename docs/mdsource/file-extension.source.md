# File extension

The default file extension is `.txt`. So the resulting verified file will be `TestClass.TestMethod.verified.txt`.

It can be overridden at two levels:

 * Method: Change the extension for the current test method.
 * Class: Change the extension all verifications in all test methods for a test class.

Usage:

snippet: ExtensionSample.cs

Result in two files:

snippet: ExtensionSample.InheritedFromClass.verified.json

snippet: ExtensionSample.AtMethod.verified.xml