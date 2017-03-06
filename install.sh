#!/bin/bash

if [[ $EUID -ne 0 ]]; then
   echo "This script must be run as root" 
   exit 1
fi

echo "Compiling DblTekGoIPPwn"
xbuild src/DblTekPwn.sln
echo "Copying binary to /usr/bin/DblTekGoIPPwn.exe"
sudo cp src/DblTekPwn/bin/Debug/DblTekPwn.exe /usr/bin/DblTekGoIPPwn.exe
sudo chmod 777 /usr/bin/DblTekGoIPPwn.exe
echo "Copying run script to /usr/bin/DblTekGoIPPwn"
sudo cp DblTekGoIPPwn.sh /usr/bin/DblTekGoIPPwn
sudo chmod a+x /usr/bin/DblTekGoIPPwn
echo ""
echo "All steps completed. Run DblTekGoIPPwn --help"
