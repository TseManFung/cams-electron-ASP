# cams-electron-ASP

Higher Diploma in Software Engineering - Final Year Project

Campus Asset Management System - Electron application

## Group memeber
### Tse Man Fung
### Ho Tung Wai
### Lau King Yuet
### Cheung Chun Hei


## All parts of CAMS:
[Android](https://github.com/TseManFung/crms-android) : kotlin

[Backend](https://github.com/DantehCheung/cams_backend) : kotlin + springboot

[Electron-ASP](https://github.com/TseManFung/cams-electron-ASP) : c# + electron.net

[Frontend](https://github.com/DantehCheung/cams-frontend) : JS + React

## how to use
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
