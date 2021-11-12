del nupkgs\*.nupkg

dotnet restore

dotnet pack src\Zoxive.HttpLoadTesting.Client\Zoxive.HttpLoadTesting.Client.csproj --configuration release --output nupkgs

f:\Utilities\nuget.exe push nupkgs\*.nupkg -source "https://api.nuget.org/v3/index.json"