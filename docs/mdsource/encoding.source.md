# Encoding

## UseUtf8NoBom

The default encoding for snapshot files uses UTF-8 with byte order marks (BOM) enable. To disable UTF-8 BOMs, call `VerifierSettings.UseUtf8NoBom`.

snippet: UseUtf8NoBom


## UseEncoding

To override the encoding used for snapshot files, replacing the default UTF-8 encoding, call `VerifierSettings.UseEncoding` providing a `System.Text.Encoding` instance.

snippet: UseEncoding
