@echo off
dotnet publish -r win-x64 -c Release -o "Piano2GW/bin/Publish" --self-contained true -p:PublishTrimmed=true -p:PublishSingleFile=true -p:DebugType=none
explorer "Piano2GW\bin\Publish"