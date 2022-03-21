############################################################################################
#                       NSIS File Associations Tool Script For GC Studio  V.1.01
#                                   By Angel Mier                              
############################################################################################

######################################################################
# Includes

!include "LogicLib.nsh"

######################################################################
# Installer Configuration

!define APP_NAME "GC Studio"
!define COMP_NAME "Mier Engineering"
!define WEB_SITE "https://www.gcbasic.com"
!define VERSION "1.02.00.00"
!define COPYRIGHT "Copyright © 2007 - 2022"
!define DESCRIPTION "Application"
!define INSTALLER_NAME ".\fassoc.exe"
!define MAIN_APP_EXE "fassoc.exe"
!define INSTALL_TYPE "SetShellVarContext current"
!define REG_ROOT "HKCU"
!define REG_CLASSES "HKCR"
!define REG_APP_PATH "Software\Microsoft\Windows\CurrentVersion\App Paths\${MAIN_APP_EXE}"
!define UNINSTALL_PATH "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}"
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
Caption "${APP_NAME} File Associations Tool"
OutFile "${INSTALLER_NAME}"
BrandingText "${APP_NAME}"
XPStyle on
InstallDirRegKey "${REG_ROOT}" "${REG_APP_PATH}" ""
InstallDir "$EXEDIR"


######################################################################
# Write Reg Keys
Section -Icons_Reg

#Add Path
; Check if the path entry already exists and write result to $0
nsExec::Exec 'echo %PATH% | find "$INSTDIR\vscode\bin"'
Pop $0   ; gets result code

${If} $0 = 0
    nsExec::Exec 'setx PATH %PATH%;$INSTDIR\vscode\bin'
${EndIf}


#File Associations
WriteRegStr ${REG_CLASSES} ".gcb" "" "GCB File"
WriteRegStr ${REG_CLASSES} "GCB File\shell\open\command" ""  "$INSTDIR\GCstudio.exe $\"%1$\""
WriteRegStr ${REG_CLASSES} "GCB File\Defaulticon" "" "$INSTDIR\GCstudio.exe,0"

WriteRegStr ${REG_CLASSES} ".asm" "" "ASM File"
WriteRegStr ${REG_CLASSES} "ASM File\shell\open\command" ""  "$INSTDIR\GCstudio.exe $\"%1$\""
WriteRegStr ${REG_CLASSES} "ASM File\Defaulticon" "" "$INSTDIR\FileIcons\asm.ico,0"

WriteRegStr ${REG_CLASSES} ".s" "" "S File"
WriteRegStr ${REG_CLASSES} "S File\shell\open\command" ""  "$INSTDIR\GCstudio.exe $\"%1$\""
WriteRegStr ${REG_CLASSES} "S File\Defaulticon" "" "$INSTDIR\FileIcons\asm.ico,0"

WriteRegStr ${REG_CLASSES} ".bas" "" "FBasic File"
WriteRegStr ${REG_CLASSES} "FBasic File\shell\open\command" ""  "$INSTDIR\GCstudio.exe $\"%1$\""
WriteRegStr ${REG_CLASSES} "FBasic File\Defaulticon" "" "$INSTDIR\FileIcons\bas.ico,0"

WriteRegStr ${REG_CLASSES} ".bi" "" "Fbasic Library"
WriteRegStr ${REG_CLASSES} "Fbasic Library\shell\open\command" ""  "$INSTDIR\GCstudio.exe $\"%1$\""
WriteRegStr ${REG_CLASSES} "Fbasic Library\Defaulticon" "" "$INSTDIR\FileIcons\h.ico,0"

WriteRegStr ${REG_CLASSES} ".h" "" "GCB Library"
WriteRegStr ${REG_CLASSES} "GCB Library\shell\open\command" ""  "$INSTDIR\GCstudio.exe $\"%1$\""
WriteRegStr ${REG_CLASSES} "GCB Library\Defaulticon" "" "$INSTDIR\FileIcons\h.ico,0"

WriteRegStr ${REG_CLASSES} ".json" "" "Json File"
WriteRegStr ${REG_CLASSES} "Json File\shell\open\command" ""  "$INSTDIR\GCstudio.exe $\"%1$\""
WriteRegStr ${REG_CLASSES} "Json File\Defaulticon" "" "$INSTDIR\FileIcons\json.ico,0"

WriteRegStr ${REG_CLASSES} ".code-workspace" "" "GCB Project"
WriteRegStr ${REG_CLASSES} "GCB Project\shell\open\command" ""  "$INSTDIR\GCstudio.exe $\"%1$\""
WriteRegStr ${REG_CLASSES} "GCB Project\Defaulticon" "" "$INSTDIR\FileIcons\project.ico,0"


#Windows Context Menu
#shell
WriteRegStr ${REG_CLASSES} "Directory\shell\GCstudio" "" "Open with GC Studio"
WriteRegStr ${REG_CLASSES} "Directory\shell\GCstudio\command" ""  "$INSTDIR\GCstudio.exe $\"%V$\""
WriteRegStr ${REG_CLASSES} "Directory\shell\GCstudio" "Icon" "$INSTDIR\GCstudio.exe,0"

WriteRegStr ${REG_CLASSES} "Directory\Background\shell\GCstudio" "" "Open with GC Studio"
WriteRegStr ${REG_CLASSES} "Directory\Background\shell\GCstudio\command" ""  "$INSTDIR\GCstudio.exe $\"%V$\""
WriteRegStr ${REG_CLASSES} "Directory\Background\shell\GCstudio" "Icon" "$INSTDIR\GCstudio.exe,0"

WriteRegStr ${REG_ROOT} "Software\Classes\Directory\Background\shell\GCstudio" "" "Open with GC Studio"
WriteRegStr ${REG_ROOT} "Software\Classes\Directory\Background\shell\GCstudio\command" ""  "$INSTDIR\GCstudio.exe $\"%V$\""
WriteRegStr ${REG_ROOT} "Software\Classes\Directory\Background\shell\GCstudio" "Icon" "$INSTDIR\GCstudio.exe,0"

#drive
WriteRegStr ${REG_CLASSES} "Drive\shell\GCstudio" "" "Open with GC Studio"
WriteRegStr ${REG_CLASSES} "Drive\shell\GCstudio\command" ""  "$INSTDIR\GCstudio.exe $\"%V$\""
WriteRegStr ${REG_CLASSES} "Drive\shell\GCstudio" "Icon" "$INSTDIR\GCstudio.exe,0"

#files
WriteRegStr ${REG_CLASSES} "*\shell\GCstudio" "" "Open with GC Studio"
WriteRegStr ${REG_CLASSES} "*\shell\GCstudio\command" ""  "$INSTDIR\GCstudio.exe $\"%1$\""
WriteRegStr ${REG_CLASSES} "*\shell\GCstudio" "Icon" "$INSTDIR\GCstudio.exe,0"

SectionEnd

######################################################################
