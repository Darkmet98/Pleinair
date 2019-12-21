#!/bin/bash
dotnet publish -r win-x64 -f netcoreapp3.1 -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true
dotnet publish -r linux-x64 -f netcoreapp3.1 -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true
dotnet publish -r osx-x64 -f netcoreapp3.1 -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true