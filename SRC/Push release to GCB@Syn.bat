@echo off
cd GCstudio
cd bin
cd release
cd net6.0-windows
del *.pdb
del *.config
del cvs.nfo
del mrf.dat
del mrd.dat
cd vscode
cd data
del argv.json
cd user-data
del *.* /Q
rmdir Backups /S /Q
rmdir blob_storage /S /Q
rmdir cache /S /Q
rmdir databases /S /Q
rmdir cacheddata /S /Q
rmdir cachedextensions /S /Q
rmdir "code cache" /S /Q
rmdir dictionaries /S /Q
rmdir gpucache /S /Q
rmdir "local storage" /S /Q
rmdir logs /S /Q
rmdir "service worker" /S /Q
rmdir "session storage" /S /Q
cd user
del settings.json
rmdir globalstorage /S /Q
rmdir workspacestorage /S /Q
cd ..
cd ..
cd ..
cd ..
cd ..
Xcopy /E /I /Y .\net6.0-windows "C:\GCB@Syn"
pause
