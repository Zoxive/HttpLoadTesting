del *.nupkg

REM cd /d src\Zoxive.HttpLoadTesting.Client
REM CMD /C npm run release
REM cd /d ../..

dotnet restore

dotnet pack src\Zoxive.HttpLoadTesting.Framework\Zoxive.HttpLoadTesting.Framework.csproj--configuration release
dotnet pack src\Zoxive.HttpLoadTesting.Client\Zoxive.HttpLoadTesting.Client.csproj --configuration release

REM c:\Utilities\nuget.exe push *.nupkg -source "https://api.nuget.org/v3/index.json"