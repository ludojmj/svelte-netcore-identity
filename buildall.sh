#!/bin/bash
step=0
STEPS=6

clear
let "step++"
printf "\n************************** $step/$STEPS : Init database...\n"
sqlite3 Server/App_Data/order.db < Server/App_Data/create_tables.sql

let "step++"
printf "\n************************** $step/$STEPS : Building client...\n"
cd client
npm install
rm -rfv dist
npm run build

let "step++"
printf "\n************************** $step/$STEPS : Copying client build to wwwroot server folder...\n"
rm -rfv ../Server/wwwroot/*
printf "\n"
cp -rfv dist/. ../Server/wwwroot

let "step++"
printf "\n************************** $step/$STEPS : Testing server...\n"
dotnet test ../Server.UnitTest

let "step++"
printf "\n************************** $step/$STEPS : Building server...\n"
dotnet build ../Server --configuration Release

let "step++"
printf "\n************************** $step/$STEPS : Finalizing...\n"
cd ..
printf "\nDone.\n\n"
# cd Server
# dotnet bin/Release/net6.0/Server.dll
