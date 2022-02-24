@echo off
echo *** foINI ***
echo.
REM  <>
REM  This batchfile is called from G+Stool.exe to run the CGBasic-Compiler.
REM  %1 is replaced with the surcecode-file in double quotes i.e. "C:\My Folder\nice file.gcb"
REM  G+Stool.exe makes the GreatCowBasic folder the current folder.

echo Great Cow BASIC Pre-processing (Copyright 2007..2019)

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

:CurrentFile
REM Now compile current file.
..\G+Stools\gawk.exe -v NoHeaderMessage=1 -f ..\G+Stools\preprocess.awk %filename%
if exist Errors.txt (

  REM
  REM Check for any Error messages
  REM
  findstr /L "Error:" Errors.txt

  if errorlevel 1 (
    REM Do not exit,  there must only be warnings from the preprocesor.
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

REM  --- edit command below (don't delete /NP) -----------------
:compilecode

  REM Handle FLASH
  echo Search for existing hexFile, compile if hexFile is not found, then program.
  gcbasic.exe  %filename% /NP /S:use.ini /FO

REM time to read the messages
@timeout 2 > nul

REM  --- edit END ----------------------------------------------

REM  Add an minimum errorfile for others than compilererrors:
if %errorlevel% == 0 goto END
 echo An error occured - exit code: %errorlevel%
 echo An error occured - exit code: %errorlevel% >>Errors.txt
:END
