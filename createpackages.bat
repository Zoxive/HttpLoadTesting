del *.nupkg

c:\Utilities\nuget.exe pack src\Zoxive.HttpLoadTesting.Client\project.json
c:\Utilities\nuget.exe pack src\Zoxive.HttpLoadTesting.Framework\project.json

c:\Utilities\nuget.exe push *.nupkg -source "https://api.nuget.org/v3/index.json"