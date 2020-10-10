@echo off
dotnet publish -c Release -o "Piano2GW/bin/Publish/FD" -r win-x64 --self-contained false -p:PublishSingleFile=true -p:DebugType=none
dotnet publish -c Release -o "Piano2GW/bin/Publish/SC" -r win-x64 --self-contained true -p:PublishTrimmed=true -p:PublishSingleFile=true -p:DebugType=none
explorer "Piano2GW\bin\Publish"