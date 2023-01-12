All text extensions of `*.verified.*` and have eol set to `lf`.

eg add the following to `.gitattributes`

```
*.verified.txt text eol=lf
*.verified.xml text eol=lf
*.verified.json text eol=lf
```

Note that this is a suggested subset of verified text extension. Add others as required based on the file types being verified.