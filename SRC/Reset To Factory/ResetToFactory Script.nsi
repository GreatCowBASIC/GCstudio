############################################################################################
#                       NSIS Reset To Factory Script For GC Studio  V.1.00
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
!define INSTALLER_NAME ".\ResetToFactory.exe"
!define MAIN_APP_EXE "ResetToFactory.exe"
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
Caption "${APP_NAME} Reset to Factory"
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
SetOverwrite on

#Clear
RMDir /r "$INSTDIR\vscode\data"
Delete "$INSTDIR\mrf.dat"
Delete "$INSTDIR\mrd.dat"
Delete "$INSTDIR\lstsz.dat"
#Reset
SetOutPath "$INSTDIR"
File /r ".\base\*"

SectionEnd


######################################################################
