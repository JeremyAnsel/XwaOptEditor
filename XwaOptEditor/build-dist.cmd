@echo off
setlocal

cd "%~dp0"

For %%a in (
"OptStructure\bin\Release\*.dll"
"OptStructure\bin\Release\*.exe"
"OptStructure\bin\Release\*.config"
) do (
xcopy /s /d "%%~a" dist\
)

For %%a in (
"OptTextures\bin\Release\*.dll"
"OptTextures\bin\Release\*.exe"
"OptTextures\bin\Release\*.config"
) do (
xcopy /s /d "%%~a" dist\
)

For %%a in (
"XwaOptExplorer\bin\Release\*.dll"
"XwaOptExplorer\bin\Release\*.exe"
"XwaOptExplorer\bin\Release\*.config"
) do (
xcopy /s /d "%%~a" dist\
)

For %%a in (
"XwaOptEditor\bin\Release\*.dll"
"XwaOptEditor\bin\Release\*.exe"
"XwaOptEditor\bin\Release\*.config"
) do (
xcopy /s /d "%%~a" dist\
)

For %%a in (
"XwaOptEditor\bin\Release\x86\*.dll"
) do (
xcopy /s /d "%%~a" dist\x86\
)

For %%a in (
"XwaOptEditor\bin\Release\x64\*.dll"
) do (
xcopy /s /d "%%~a" dist\x64\
)

For %%a in (
"patch_32bpp\XwaExePatcher\*.*"
) do (
xcopy /s /d "%%~a" dist\patch_32bpp\
)

For %%a in (
"XwaHangarMapEditor\bin\Release\*.dll"
"XwaHangarMapEditor\bin\Release\*.exe"
"XwaHangarMapEditor\bin\Release\*.config"
) do (
xcopy /s /d "%%~a" dist\
)

For %%a in (
"XwaSFoilsEditor\bin\Release\*.dll"
"XwaSFoilsEditor\bin\Release\*.exe"
"XwaSFoilsEditor\bin\Release\*.config"
) do (
xcopy /s /d "%%~a" dist\
)
