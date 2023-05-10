**Source control settings**

All text extensions of `*.verified.*` should have:

 * `eol` set to `lf`
 * `working-tree-encoding` set to `UTF-8`

eg add the following to `.gitattributes`

```
*.verified.txt text eol=lf working-tree-encoding=UTF-8
*.verified.xml text eol=lf working-tree-encoding=UTF-8
*.verified.json text eol=lf working-tree-encoding=UTF-8
```

**EditorConfig**

https://editorconfig.org/

```
# Verify settings
[*.{received,verified}.{txt,xml,json}]
charset = "utf-8-bom"
end_of_line = lf
indent_size = unset
indent_style = unset
insert_final_newline = false
tab_width = unset
trim_trailing_whitespace = false
```


*Note that this is a suggested subset of text extension. Add others as required based on the file types being verified.*