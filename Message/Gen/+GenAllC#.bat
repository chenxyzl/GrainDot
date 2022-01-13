@echo off
for %%i in (*.proto) do (
   echo gen %%~nxi...
   protoc.exe -I=. -I=.. --csharp_out=../../Server/Model/Module/Message --csharp_opt=file_extension=.cs --grpc_out=../../Server/Model/Module/Message --plugin=protoc-gen-grpc=grpc_csharp_plugin.exe  %%~nxi)

echo finish... 
pause