ARG SDK_VERSION="8.0"
FROM mcr.microsoft.com/dotnet/sdk:${SDK_VERSION}

RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
    && apt-get -y install --no-install-recommends pdftk
