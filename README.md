<div id="GCstudio-logo" align="center">
    <br />
    <img src="./SRC/Logo/LogoSVG.svg" alt="GC Studio Logo" width="210"/>
    <h1>GC Studio</h1>
    <h3>Modern User Interface and IDE for a GREAT compiler!</h3>
</div>

GC Studio it’s a contribution to [Great Cow Basic](https://sourceforge.net/projects/gcbasic/), that brings the option to select between multiple IDE’s, integrates an automatic update system, tailors a very powerful IDE called GC Code whit specific customizations, and incorporates an integrated dev environment for the project itself, while gives a modern look to the user interface.

This project is brought to you by: Evan R. Venn and Angel Mier.


This repository uses Git LFS from GitHub https://git-lfs.github.com/ for *.exe files because code.exe binary is greater than 100mb github file size limit. install and then run this command:

    git lfs install


## Common Locations:

| Description | Directory |
| --- | --- |
| GCstudio Source/Project | "SRC/" |
| GCstudio Current Build | "SRC/GCstudio/bin/Release/" |
| GCcode Current Build | "SRC/GCstudio/bin/Release/vscode/" |
| GCB Extension | "SRC/GCstudio/bin/Release/vscode/data/extensions/MierEngineering.GreatCowBasic-1.0.0/" |
| Batch files Clean/Push | "SRC/" |

# Important:
* Remember to always run "clean for release.bat" batch file before commit your work.
* You can clean and push current build to "C:\GCB@Syn" for testing whit "Push release to GCB@Syn.bat" (all contents of "C:\GCB@Syn" will be overwritten)
* You can run GCstudio directly from release directory to modify the project files of the repo, then push to GCB@Syn for testing.


### OS Support
Win7 SP2  x86/x64 to Win11 x64, All binary’s compiled in 32 bit.

### Prerequisites:
[.Net Framework 4.8](https://dotnet.microsoft.com/en-us/download/dotnet-framework/thank-you/net48-web-installer)

[optional] [GIT](https://github.com/git-for-windows/git/releases/download/v2.35.1.windows.2/Git-2.35.1.2-64-bit.exe)

[optional] [SVN](https://osdn.net/projects/tortoisesvn/storage/1.14.1/Application/TortoiseSVN-1.14.1.29085-x64-svn-1.14.1.msi/)

### Current Changes and Updates:
- [Change Log](http://www.aritaconsultores.com/GCBbug/changelog_page.php)
- [Road Map](http://www.aritaconsultores.com/GCBbug/roadmap_page.php)

### If you found a bug, please [report it here!](http://www.aritaconsultores.com/GCBbug/bug_report_page.php)



# License
GCstudio is released under GPL-3.0 License

GCcode is released under MIT License and is based on the work of vscode from Microsoft and the vscodium project:
- https://github.com/microsoft/vscode/
- https://github.com/VSCodium/vscodium/
