############################################################################################
#                       NSIS Post Update Script For GC Studio  V.1.00
#                                   By Angel Mier                              
############################################################################################

######################################################################
# Includes


######################################################################
# Installer Configuration

!define APP_NAME "GC Studio"
!define COMP_NAME "Mier Engineering"
!define WEB_SITE "https://www.gcbasic.com"
!define VERSION "1.02.00.00"
!define COPYRIGHT "Copyright © 2007 - 2022"
!define DESCRIPTION "Application"
!define INSTALLER_NAME ".\postupdate.exe"
!define MAIN_APP_EXE "postupdate.exe"
!define INSTALL_TYPE "SetShellVarContext current"
######################################################################
# Bynary Information

VIProductVersion  "${VERSION}"
VIAddVersionKey "ProductName"  "${APP_NAME}"
VIAddVersionKey "CompanyName"  "${COMP_NAME}"
VIAddVersionKey "LegalCopyright"  "${COPYRIGHT}"
VIAddVersionKey "FileDescription"  "${DESCRIPTION}"
VIAddVersionKey "FileVersion"  "${VERSION}"

######################################################################
# Compression and installer settings

SetCompressor LZMA
Name "${APP_NAME}"
Caption "${APP_NAME} Post Update"
OutFile "${INSTALLER_NAME}"
BrandingText "${APP_NAME}"
XPStyle on
InstallDir "$EXEDIR"
RequestExecutionLevel user

######################################################################
# Main program, Post Update Files
Icon ".\res\GCstudio.ico"
Section -MainProgram
${INSTALL_TYPE}
SetOverwrite ifnewer

#Post
SetOutPath "$INSTDIR"
File /r ".\post\*"

#Prereq Net8
SetOutPath "$INSTDIR"
File /r ".\Redist\Net8x86.exe"
ExecWait "$INSTDIR\Net8x86.exe /install /quiet /norestart /log Log\Net8.log"
Delete "$INSTDIR\Net8x86.exe"

SectionEnd


######################################################################
