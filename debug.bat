@echo off

dotnet build src/Skybrud.Umbraco.Redirects --configuration Debug /t:rebuild /t:pack -p:BuildTools=1 -p:PackageOutputPath=c:/nuget