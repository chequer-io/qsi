#!/bin/bash

# Linux
if [[ "$OSTYPE" == "linux-gnu"* ]]; then
    if [ -z $(which pwsh) ] ; then
        while true; do
            read -p "[QSI] pwsh command not found. Do you wish to install powershell?: [Y/N] " yn
            case $yn in
                [Yy]* ) 
                    # install the requirements
                    sudo apk add --no-cache \
                        ca-certificates \
                        less \
                        ncurses-terminfo-base \
                        krb5-libs \
                        libgcc \
                        libintl \
                        libssl1.1 \
                        libstdc++ \
                        tzdata \
                        userspace-rcu \
                        zlib \
                        icu-libs \
                        curl

                    sudo apk -X https://dl-cdn.alpinelinux.org/alpine/edge/main add --no-cache \
                        lttng-ust

                    # Download the powershell '.tar.gz' archive
                    curl -L https://github.com/PowerShell/PowerShell/releases/download/v7.2.0/powershell-7.2.0-linux-alpine-x64.tar.gz -o /tmp/powershell.tar.gz

                    # Create the target folder where powershell will be placed
                    sudo mkdir -p /opt/microsoft/powershell/7

                    # Expand powershell to the target folder
                    sudo tar zxf /tmp/powershell.tar.gz -C /opt/microsoft/powershell/7

                    # Set execute permissions
                    sudo chmod +x /opt/microsoft/powershell/7/pwsh

                    # Create the symbolic link that points to pwsh
                    sudo ln -s /opt/microsoft/powershell/7/pwsh /usr/bin/pwsh
                    break;;
                [Nn]* ) exit;;
                * ) echo "Please answer yes or no.";;
            esac
        done
    fi

# MacOS
elif [[ "$OSTYPE" == "darwin"* ]]; then
    if [ -z $(which brew) ] ; then
        while true; do
            read -p "[QSI] brew command not found. Do you wish to install brew?: [Y/N] " yn
            case $yn in
                [Yy]* ) /bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"; break;;
                [Nn]* ) exit;;
                * ) echo "Please answer yes or no.";;
            esac
        done
    fi
    
    if [ -z $(which pwsh) ] ; then
        while true; do
            read -p "[QSI] pwsh command not found. Do you wish to install powershell?: [Y/N] " yn
            case $yn in
                [Yy]* ) brew install --cask powershell; break;;
                [Nn]* ) exit;;
                * ) echo "Please answer yes or no.";;
            esac
        done
    fi

# Windows
elif [ "$OSTYPE" -eq "cygwin" ] || [ "$OSTYPE" -eq "msys" ] || [ "$OSTYPE" -eq "win32" ]; then
    ./setup.ps1 $@
    exit 0

# Unknown
else
    echo "[QSI] Unknown OS detected '$OSTYPE'"
fi

echo "[QSI] Redirect to pwsh"
pwsh ./setup.ps1 $@
