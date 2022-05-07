@echo off
setlocal

cd "%~dp0"

For %%a in (
"OptStructure\bin\Release\net48\*.dll"
"OptStructure\bin\Release\net48\*.exe"
"OptStructure\bin\Release\net48\*.config"
) do (
xcopy /s /d "%%~a" dist\
)

For %%a in (
"OptTextures\bin\Release\net48\*.dll"
"OptTextures\bin\Release\net48\*.exe"
"OptTextures\bin\Release\net48\*.config"
) do (
xcopy /s /d "%%~a" dist\
)

For %%a in (
"XwaOptExplorer\bin\Release\net48\*.dll"
"XwaOptExplorer\bin\Release\net48\*.exe"
"XwaOptExplorer\bin\Release\net48\*.config"
) do (
xcopy /s /d "%%~a" dist\
)

For %%a in (
"XwaOptEditor\bin\Release\net48\*.dll"
"XwaOptEditor\bin\Release\net48\*.exe"
"XwaOptEditor\bin\Release\net48\*.config"
"XwaOptEditor\bin\Release\net48\XwaOptEditor.pdb"
) do (
xcopy /s /d "%%~a" dist\
)

For %%a in (
"XwaOptEditor\bin\Release\net48\Win32\*.dll"
) do (
xcopy /s /d "%%~a" dist\Win32\
)

For %%a in (
"XwaOptEditor\bin\Release\net48\Win64\*.dll"
) do (
xcopy /s /d "%%~a" dist\Win64\
)

For %%a in (
"XwaHangarMapEditor\bin\Release\net48\*.dll"
"XwaHangarMapEditor\bin\Release\net48\*.exe"
"XwaHangarMapEditor\bin\Release\net48\*.config"
) do (
xcopy /s /d "%%~a" dist\
)

For %%a in (
"XwaOptProfilesViewer\bin\Release\net48\*.dll"
"XwaOptProfilesViewer\bin\Release\net48\*.exe"
"XwaOptProfilesViewer\bin\Release\net48\*.config"
) do (
xcopy /s /d "%%~a" dist\
)

For %%a in (
"XwaSFoilsEditor\bin\Release\net48\*.dll"
"XwaSFoilsEditor\bin\Release\net48\*.exe"
"XwaSFoilsEditor\bin\Release\net48\*.config"
) do (
xcopy /s /d "%%~a" dist\
)
