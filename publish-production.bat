@echo off
echo ========================================
echo Building Nafes.API for Production...
echo ========================================

cd /d "%~dp0"

echo Cleaning previous builds...
if exist "publish" rmdir /s /q "publish"

echo Building in Release mode...
dotnet publish -c Release -o ./publish

echo Copying web.config to publish folder...
copy web.config publish\web.config

echo ========================================
echo Build Complete!
echo Files ready in: %cd%\publish
echo ========================================
pause
