this is a electron app and use ASP for connect RFID reader
need to use with crms-web-frontend

dependency:
electron.net

B4 start
> dotnet tool install ElectronNET.CLI -g

first time
> electronize init

for start the app
> electronize start

for build app
> electronize build /target win

In addition to win, you can also specify osx, linux
