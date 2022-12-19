# AppendFile

Allows extra files to be verified in addition to the primary target.


## AppendFile

snippet: AppendFile

Will result in two files being verified:

 * `Tests.AppendFile#00.verified.txt` containing `Foo`
 * `Tests.AppendFile#sample.verified.png`


## AppendContentAsFile

snippet: AppendContentAsFile

Will result in two files being verified:

 * `Tests.AppendContentAsFile#00.verified.txt` containing `Foo`
 * `Tests.AppendContentAsFile#01.verified.txt` containing `extra content`
