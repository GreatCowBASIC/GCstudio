@echo off
echo *** makeASM ***
echo.

rem Unrem ATMELStudio variable below to enable AVR validation - this folder must point to the exe called avrasm2.exe
REM set ATMELStudio=C:\Program Files (x86)\Atmel\Atmel Toolchain\AVR Assembler\Native\2.1.1175\avrassembler\

REM  Great Cow BASIC Copyright 2007..2022
REM  Updated FEb 2022
REM  This batchfile is called from G+Stool.exe to run the CGBasic-Compiler.
REM  %1 is replaced with the sourcecode-file in double quotes i.e. "C:\My Folder\nice file.gcb"
REM  G+Stool.exe makes the GreatCowBasic folder the current folder.
echo Great Cow BASIC Pre-processing (Copyright 2007..2022)

REM revised Feb 2020 to improve performance.  Used dummy .h file called Inspecting_Libraries.h to ensure no error message is issued


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

REM  Call GCBasic to make an asm-file only:

REM Tidyup any existing files
if exist errors.txt del errors.txt
if exist preprocesserror.txt del preprocesserror.txt
if exist filesofinterest.txt del filesofinterest.txt
if exist include\Inspecting_Libraries.h del include\Inspecting_Libraries.h

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

REM  --- edit command below (don't delete /NP) -----------------
:compilecode
REM  --- edit command below (don't delete /NP) -----------------
echo off
gcbasic.exe /NP /S:use.ini %filename% /P: /H:N

setlocal enableDelayedExpansion
set file=%filename%
FOR /F "delims=" %%i IN ("%file%") DO (
set filedrive=%%~di
set filepath=%%~pi
set filename=%%~ni
set fileextension=%%~xi
)

findstr "SREG" "%filedrive%%filepath%%filename%.asm" > NUL
if errorlevel 1 goto end



rem Echo "%var:~1,-5%.asm"
if "%ATMELStudio%" == "" (
  rem
 ) else (
echo Validation in ATMEL Studio
"%ATMELStudio%\avrasm2.exe"  -v0 "%filedrive%%filepath%%filename%.asm" -I "%ATMELStudio%\Include"   1>>Errors.txt 2>&1
rem @timeout 2 > nul
)
REM  --- edit END ----------------------------------------------


REM  Add a minimum errorfile for others than compilererrors:
if not errorlevel 1 goto END
 echo.>>Errors.txt
:END
if exist filesofinterest.txt del filesofinterest.txt
if exist include\Inspecting_Libraries.h del include\Inspecting_Libraries.h
