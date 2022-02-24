<div id="GCstudio-logo" align="center">
    <br />
    <img src="./SRC/Logo/LogoSVG.svg" alt="GC Studio Logo" width="210"/>
    <h1>GC Studio</h1>
    <h3>Modern User Interface and IDE for a GREAT compiler!</h3>
</div>

GC Studio it’s a contribution to [Great Cow Basic](https://sourceforge.net/projects/gcbasic/), that brings the option to select between multiple IDE’s, integrates an automatic update system, tailors a very powerful IDE called GC Code whit specific customizations, and incorporates an integrated dev environment for the project itself, while gives a modern look to the user interface.

This project is brought to you by: Evan R. Venn and Angel Mier.

## Common Locations:

| Description | Directory |
| --- | --- |
| GCstudio Source/Project | "SRC/" |
| GCstudio Current Build | "Build/net6.0-windows/" |
| GCcode Current Build | "Build/net6.0-windows/vscode/" |
| GCB Extension | "Build/net6.0-windows/vscode/data/extensions/MierEngineering.GreatCowBasic-1.0.0/" |
| Batch files Clean/Push | "@Root" |


### OS Support
Win7 SP1**  x86/x64 to Win11 x64.

**Service Pack 1 whit [KB4474419](https://www.catalog.update.microsoft.com/Search.aspx?q=KB4474419), [KB2999226](https://www.catalog.update.microsoft.com/Search.aspx?q=KB2999226).

### Prerequisites:
[.Net 6.0 - x86](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-6.0.2-windows-x86-installer)

[optional] [GIT](https://github.com/git-for-windows/git/releases/download/v2.35.1.windows.2/Git-2.35.1.2-64-bit.exe)

[optional] [SVN](https://osdn.net/projects/tortoisesvn/storage/1.14.1/Application/TortoiseSVN-1.14.1.29085-x64-svn-1.14.1.msi/)

### Current Changes and Updates:
- [Change Log](http://www.aritaconsultores.com/GCBbug/changelog_page.php)
- [Road Map](http://www.aritaconsultores.com/GCBbug/roadmap_page.php)

### If you found a bug, please [report it here!](http://www.aritaconsultores.com/GCBbug/bug_report_page.php)


# For Pull Requests:

* Remember to always run "_Clean.bat" batch file before commit your work.
* You can clean and push current build to "C:\GCB@Syn" for testing whit "_Push to GCB@Syn.bat" (all contents of "C:\GCB@Syn" will be overwritten)
* You can run GCstudio directly from Build directory to modify the project files of the repo, then push to GCB@Syn for testing.

This repository uses Git LFS from GitHub https://git-lfs.github.com/ for *.exe files; if you are going to commit exe files you need to install and then run this command:

    git lfs install


# License
GCstudio is released under GPL-3.0 License

GCcode is released under MIT License and is based on the work of vscode from Microsoft and the vscodium project:
- https://github.com/microsoft/vscode/
- https://github.com/VSCodium/vscodium/
