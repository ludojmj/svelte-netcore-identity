#!/bin/bash
step=0
STEPS=5

clear

let "step++"
printf "\n************************** $step/$STEPS : Building client...\n"
cd client
npm install
rm -rfv public/build
npm run build

let "step++"
printf "\n************************** $step/$STEPS : Copying client build to wwwroot server folder...\n"
rm -rfv ../Server/wwwroot/*
printf "\n"
cp -rfv public/. ../Server/wwwroot

let "step++"
printf "\n************************** $step/$STEPS : Testing server...\n"
dotnet test ../Server.UnitTest

let "step++"
printf "\n************************** $step/$STEPS : Building server...\n"
dotnet build ../Server --configuration Release

cd ..
printf "\nDone.\n\n"
# cd Server
# dotnet bin/Release/net6.0/Server.dll
