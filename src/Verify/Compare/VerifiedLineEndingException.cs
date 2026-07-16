class VerifiedLineEndingException(string path) :
    Exception(
        $"""
         Verified file must use \n line endings, but it contains a \r (carriage return).
         Path: {path}
         The usual cause is git checking the file out with \r\n. Add to .gitattributes:
           *.verified.txt text eol=lf working-tree-encoding=UTF-8
         then re-checkout the files:
           git rm --cached -r .
           git reset --hard
         See https://github.com/verifytests/verify#text-file-settings
         """);
