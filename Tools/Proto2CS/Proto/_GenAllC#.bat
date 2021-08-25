@echo off
for %%i in (*.proto) do (
   echo gen %%~nxi...
   protoc.exe -I=. -I=.. --csharp_out=../Client --csharp_opt=file_extension=.cs --grpc_out=../Client --plugin=protoc-gen-grpc=grpc_csharp_plugin.exe  %%~nxi)

echo finish... 
pause