@echo off
REM ==============================================================
REM  PTTracker - start backend API and frontend web together.
REM  Opens two terminal windows. Close a window to stop that server.
REM ==============================================================
cd /d "%~dp0"

echo Starting PTTracker...
echo   API : http://localhost:5080   (Swagger at /swagger)
echo   Web : http://localhost:5173
echo.

REM --- Backend API (ASP.NET Core) on port 5080 ---
start "PTTracker API" cmd /k "set ASPNETCORE_ENVIRONMENT=Development&& dotnet run --project src\PTTracker.Api --urls http://localhost:5080"

REM --- Frontend (Vite). Installs deps on first run, then dev server on 5173 ---
start "PTTracker Web" cmd /k "cd web && npm run dev"

echo Two windows opened. You can close this launcher window.
