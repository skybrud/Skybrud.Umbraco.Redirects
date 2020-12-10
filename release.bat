@echo off

dotnet build src/Skybrud.Umbraco.Redirects --configuration Release /t:rebuild /t:pack -p:BuildTools=1 -p:PackageOutputPath=../../releases/nuget