@echo off
echo *** makeHEX ***
echo.

rem *** set the filename and test
set filename=%1
set tmpstr=%filename:~-1%
rem *** If last char is double quote then the parameter is good
if '^%tmpstr% == '^" goto FilenameGood

rem *** build up the parameters back to a single parameter
If NOT "%2"=="" (
setlocal enableDelayedExpansion
Set filename=%filename% %2
)

If NOT "%3"=="" (
setlocal enableDelayedExpansion
Set filename=%filename% %3%
)

If NOT "%4"=="" (
setlocal enableDelayedExpansion
Set filename=%filename% %4%
)

If NOT "%5"=="" (
setlocal enableDelayedExpansion
Set filename=%filename% %5%
)

Set filename="%filename%"

:FilenameGood




REM Updated to support GCCODE IDE
REM Unrem ATMELStudio variable below to enable AVR validation - this folder must point to the exe called avrasm2.exe
REM set ATMELStudio=C:\Program Files (x86)\Atmel\Atmel Toolchain\AVR Assembler\Native\2.1.1175\avrassembler\

REM  Great Cow BASIC Copyright 2007..2022
REM  This batchfile is called from G+Stool.exe to run the CGBasic-Compiler.
REM  %1 is replaced with the sourcecode-file in double quotes i.e. "C:\My Folder\nice file.gcb"
REM  G+Stool.exe makes the GreatCowBasic folder the current folder.
echo Great Cow BASIC Pre-processing (Copyright 2007..2022)

REM revised Feb 2020 to improve performance.  Used dummy .h file called Inspecting_Libraries.h to ensure no error message is issued

REM Tidyup any existing files
if exist errors.txt del errors.txt
if exist preprocesserror.txt del preprocesserror.txt
if exist AllErrors.txt del AllErrors.txt
if exist filesofinterest.txt del filesofinterest.txt
if exist include\Inspecting_Libraries.h del include\Inspecting_Libraries.h

REM
REM Next commands prevent the first time user having to see all the new install files being validated.
REM If we do not this... first time usage would mean checking all the .h files, but they just got installed so should be ok.
REM
REM FirstRun.dat is created as part of the installation, so, we know it is there.
REM
if exist FirstRun.dat (
  echo First complilation... Welcome to Great Cow BASIC
  REM Set the archive bit on all files.
  attrib include\*.h -a /s
  del FirstRun.dat
  REM Process current source file as we do not need to check the existing files.
  goto CurrentFile
)
rem
rem Now, validate all .h files
rem
echo. > include\Inspecting_Libraries.h

dir include\*.h /b /s /aa > filesofinterest.txt

if not exist filesofinterest.txt goto CurrentFile

for /F "delims=|" %%i in (filesofinterest.txt) do (

    REM Show file being processed
    echo              %%i

    ..\G+Stools\gawk.exe -v NoHeaderMessage=1 -f ..\G+Stools\preprocess.awk  "%%i"
    REM Set the archive bit, so, we know we have processed this file.
    attrib "%%i" -A

)


:CurrentFile
REM Now compile current file.
..\G+Stools\gawk.exe -v NoHeaderMessage=1 -f ..\G+Stools\preprocess.awk %filename%
if exist Errors.txt (

  REM
  REM Check for any Error messages
  REM
  findstr /L "Error:" Errors.txt

  if errorlevel 1 (
    REM Do not exit,  there must only be warnings from the preprocessor.
    REM Show the errors
    type errors.txt

    REM
    REM Copy the error file for later processing.
    REM
    copy errors.txt preprocesserror.txt > nul
    ) ELSE (
    REM
    REM Show errors
    REM
    goto preprocesserror
  )
)

echo.
goto compilecode

:preprocesserror
rem type errors.txt
Echo.
Echo Exiting...
goto end

REM  Call GCBasic to make an asm-file and assemble to hex-file:

REM  --- edit command below (don't delete /NP) -----------------
:compilecode
gcbasic.exe /NP /S:use.ini  %filename% /F:N /P:

set file=%filename%
FOR /F "delims=" %%i IN ("%file%") DO (
set filedrive=%%~di
set filepath=%%~pi
set filename=%%~ni
set fileextension=%%~xi
)

findstr "SREG" "%filedrive%%filepath%%filename%.asm" > NUL
if errorlevel 1 goto fin1

if "%ATMELStudio%" == "" (
  rem
 ) else (
echo Validation in ATMEL Studio
"%ATMELStudio%\avrasm2.exe"  -v0 "%filedrive%%filepath%%filename%.asm" -I "%ATMELStudio%\Include"   1>>Errors.txt 2>&1
rem @timeout 2 > nul
)

REM  --- edit END ----------------------------------------------

REM  Add a minimum errorfile for others than compilererrors:

REM
REM Did the compiler throw an error?
REM
if not errorlevel 1 goto fin1
 copy /A preprocesserror.txt + Errors.txt Errors.txt > nul
 echo.>>Errors.txt
 goto END

REM
REM Did the preprocesor throw and error.  Check for the correct file.
REM
:FIN1
 if exist preprocesserror.txt  (
  REM
  REM Merge files and then show results.
  REM
  copy /Y preprocesserror.txt + Errors.txt /A AllErrors.txt > nul
  copy /Y AllErrors.txt Errors.txt > nul
) ELSE (
  REM
  REM Therefore, no errors... exit
  REM
goto End
)
 echo.>>Errors.txt
 goto END

:END
if exist filesofinterest.txt del filesofinterest.txt
if exist include\Inspecting_Libraries.h del include\Inspecting_Libraries.h