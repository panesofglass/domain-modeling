# Domain Modeling with Types

F# makes it easy to encode domain models within its type system such that it is quite possible to never allow invalid states while retaining a simple, readable code base. We'll look at a few examples, including how best to deal with inheritance vs composition, units of measure, and even language extensions for extensible domain specific languages.

## Running Locally

1. Run `create-db.sql` against your `(localdb)ProjectsV12` database instance, which should have been installed with Visual Studio 2015. (Once I figure out how to do this from the command line, I'll add it to the FAKE build.)
2. Run `build.ps1` to generate and launch the [FsReveal](https://github.com/fsprojects/FsReveal) slides locally.
3. When you get to the form to input cities, you should be able to submit requests as long as you have an internet connection for Bing Maps. You can find supported cities listed in `create-db.sql` or add your own list.

## Cool Tricks!

In addition to taking advantage of FsReveal's use of [Suave](https://suave.io/) for live reloading changes, this talk adds the [Freya](http://docs.freya.io/) demonstrated in the talk and implemented in the FsReveal slides to the Suave server. Not only are you seeing the code in the slides, the form uses the same implementation to make requests. You can do this with Suave or any other OWIN-compliant web framework:

1. [Include app in FsReveal slides](https://github.com/panesofglass/domain-modeling/blob/master/slides/index.fsx#L1020-L1091)
2. [Include FsReveal script in build.fsx](https://github.com/panesofglass/domain-modeling/blob/master/build.fsx#L9)
3. [Mount OWIN app in Suave server](https://github.com/panesofglass/domain-modeling/blob/master/build.fsx#L125)

## Pull Requests

Pull requests are encouraged! Add cities, add features to the very basic form submission, etc.
