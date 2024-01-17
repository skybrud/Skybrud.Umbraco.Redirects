@echo off

dotnet build src/Skybrud.Umbraco.Redirects --configuration Release /t:rebuild /t:pack -p:PackageOutputPath=../../releases/nuget