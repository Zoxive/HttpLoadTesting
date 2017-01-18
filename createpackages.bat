del nupkgs\*.nupkg

cd /d src\Zoxive.HttpLoadTesting.Client
CMD /C npm run release
cd /d ../..

dotnet restore

dotnet pack src\Zoxive.HttpLoadTesting.Framework\Zoxive.HttpLoadTesting.Framework.csproj --configuration release --output ..\..\nupkgs
dotnet pack src\Zoxive.HttpLoadTesting.Client\Zoxive.HttpLoadTesting.Client.csproj --configuration release --output ..\..\nupkgs

REM c:\Utilities\nuget.exe push *.nupkg -source "https://api.nuget.org/v3/index.json"