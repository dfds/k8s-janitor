FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

#Set container filesystem to /build (and create folder if it doesnt exist)
WORKDIR /build

#Copy files to container file system.
COPY ./src ./src

#Set workdir to current project folder
WORKDIR /build/src/K8sJanitor.WebApi

#Restore csproj packages.
RUN dotnet restore

#Compile source code using standard Release profile
RUN dotnet publish -c Release -o /build/out

FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR /app
COPY --from=build /build/out ./

#Non-root user settings
ARG USERNAME=harald
ARG USER_UID=1000
ARG USER_GID=$USER_UID

RUN groupadd --gid $USER_GID $USERNAME \
    && useradd --uid $USER_UID --gid $USER_GID -m $USERNAME 


USER $USERNAME

ENTRYPOINT [ "dotnet", "K8sJanitor.WebApi.dll" ]
