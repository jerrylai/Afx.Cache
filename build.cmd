@echo off
set Build="%SYSTEMDRIVE%\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MsBuild.exe"
if exist publish rd /s /q publish
%Build% "NET461/Afx.Cache/Afx.Cache.csproj" /t:Rebuild /p:Configuration=Release
%Build% "NET472/Afx.Cache/Afx.Cache.csproj" /t:Rebuild /p:Configuration=Release
dotnet build "NETStandard2.0/Afx.Cache/Afx.Cache.csproj" -c Release 
cd publish
del /q/s *.pdb
del /q/s Microsoft*
del /q/s Newtonsoft*
del /q/s netstandard.dll
del /q/s System*
del /q/s Pipelines*
del /q/s StackExchange*
pause