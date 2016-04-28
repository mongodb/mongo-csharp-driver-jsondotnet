@echo off
echo Installing FAKE
tools\nuget.exe install FAKE -OutputDirectory tools -ExcludeVersion -Verbosity quiet
echo Done
tools\FAKE\tools\FAKE.exe build\build.fsx %*
