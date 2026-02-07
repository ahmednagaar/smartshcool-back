@echo off
echo ========================================
echo Running Database Migrations...
echo ========================================

cd /d "%~dp0"

echo Applying migrations to production database...
dotnet ef database update

echo ========================================
echo Migration Complete!
echo ========================================
pause
