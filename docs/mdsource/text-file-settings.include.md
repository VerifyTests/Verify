Text variants of verified and received have the following characteristics:

 * UTF8 with a [Byte order mark (BOM)](https://en.wikipedia.org/wiki/Byte_order_mark)
 * Newlines as line-feed (lf)
 * No trailing newline

This manifests in several ways:


#### Source control settings

All text extensions of `*.verified.*` should have:

 * `eol` set to `lf`
 * `working-tree-encoding` set to `UTF-8`

Note: `working-tree-encoding=UTF-8` is correct even though Verify writes files with a BOM. Git does not strip or add the BOM — it passes through transparently. The `UTF-8-BOM` encoding would explicitly add a BOM on checkout and strip it on commit (so the internal blob differs from the working tree), but that is not the desired behavior since Verify writes the BOM itself and it should be preserved in the blob.

All Binary files should also be marked to avoid merging and line ending issues with binary files.

eg add the following to `.gitattributes`

```
*.verified.txt text eol=lf working-tree-encoding=UTF-8
*.verified.xml text eol=lf working-tree-encoding=UTF-8
*.verified.json text eol=lf working-tree-encoding=UTF-8
*.verified.bin binary
```


#### Line ending autocrlf

On Windows, if `core.autocrlf` is set to `true`, files may show as modified with no actual content changes. To fix this:

```
git config --global core.autocrlf input
```

`core.autocrlf=true` normalizes line endings to lf on commit, and converts them back to crlf on checkout. So the blobs stored in git are lf while the files on disk are crlf. Verify rejects a verified file containing a carriage return, so tests fail even though the committed content is correct.

Adding the `.gitattributes` entries above overrides `core.autocrlf` for those paths, but it does not update files that are already checked out. Git re-applies line ending filters only when a file's blob changes, and since `core.autocrlf` already normalized those blobs to lf, adding the attributes changes no content. A one-time refresh of the working tree is required after committing `.gitattributes`:

```
git rm --cached -r .
git reset --hard
```

This discards uncommitted changes, so commit or stash first.

Where crlf was committed to the blobs (`core.autocrlf` unset or `false`), the content itself needs normalizing instead. In that case `git add --renormalize .` produces a commit that converts the blobs, and checkout updates each working tree as it is pulled:

```
git add --renormalize .
git commit -m "Normalize line endings"
```

Build servers that reuse a cached working directory between runs need the same refresh as a developer machine. A build that clones fresh each run is unaffected.


#### EditorConfig settings

If modifying text verified/received files in an editor, it is desirable for the editor to respect the above conventions. For [EditorConfig](https://editorconfig.org/) enabled the following can be used:

```
# Verify settings
[*.{received,verified}.{json,txt,xml}]
charset = utf-8-bom
end_of_line = lf
indent_size = unset
indent_style = unset
insert_final_newline = false
tab_width = unset
trim_trailing_whitespace = false

[*.{received,verified}.{json,xml,html,htm,yaml,svg}]
indent_size = 2
indent_style = space
```

**Note that the above are suggested for subset of text extension. Add others as required based on the text file types being verified.**