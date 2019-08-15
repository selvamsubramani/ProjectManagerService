@echo off

rem CodeCoverage using Open Cover

"%~dp0packages\OpenCover.4.6.166\tools\OpenCover.Console.exe" -target:"%PROGRAMFILES(X86)%\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\MSTest.exe" -targetargs:"/noisolation /testcontainer:testing\ProjectManagement.Test\bin\Release\ProjectManagement.Test.dll" -register:user -filter:"+[ProjectManagement.Service]* -[ProjectManagement.BusinessLayer]*" -output:TestResult.xml -excludeByAttribute:"System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage" -mergebyhash -skipautoprops

exit