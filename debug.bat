@echo off
dotnet build src/Skybrud.Umbraco.Redirects --configuration Debug /t:rebuild /t:pack -p:PackageOutputPath=c:\nuget\Umbraco10