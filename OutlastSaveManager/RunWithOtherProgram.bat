@echo off
REM ================================
REM === Please change the path below
REM ================================







REM Change your Name to .exe down below after the .\
SET "IMMEDIATE_PATH=.\BetterOutlastLauncher.exe"

















REM Tool to run when OLGame.exe is found (relative to this .bat)
SET "ON_GAME_START_PATH=.\SaveManager\OutlastSaveManager.exe"

REM Process name to watch
SET "TARGET_PROCESS=OLGame.exe"

REM Maximum wait time in seconds
SET MAX_WAIT=60
SET /A ELAPSED=0

REM ===== Step 1: Run the immediate program =====
start "" "%IMMEDIATE_PATH%"
echo Running: %IMMEDIATE_PATH%

REM ===== Step 2: Wait for OLGame.exe =====
:CHECK_LOOP
tasklist /FI "IMAGENAME eq %TARGET_PROCESS%" 2>NUL | find /I "%TARGET_PROCESS%" >NUL
IF ERRORLEVEL 1 (
    REM not found, wait 2 seconds
    timeout /t 2 /nobreak >nul
    SET /A ELAPSED+=2
    IF %ELAPSED% GEQ %MAX_WAIT% (
        echo Process %TARGET_PROCESS% not found within %MAX_WAIT% seconds. Exiting.
        goto END
    )
    goto CHECK_LOOP
) ELSE (
    REM found, run the second tool

    timeout /t 5 /nobreak >nul
	start "" /D "%~dp0SaveManager" "OutlastSaveManager.exe"
   
    echo OLGame detected, running: %ON_GAME_START_PATH%
    goto END
)

:END
