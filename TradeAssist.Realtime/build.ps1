﻿<#
.SYNOPSIS
   Build the Console Application and Docker Image for Windows container
.DESCRIPTION
    Build the Console Application and Docker Image for Windows container
#>

Set-StrictMode -Version Latest
$ErrorActionPreference="Stop"
$ProgressPreference="SilentlyContinue"

# Docker image name for the application
$ImageName="tradeassist-realtime"

function Invoke-MSBuild ([string]$MSBuildPath, [string]$MSBuildParameters) {
    Invoke-Expression "$MSBuildPath $MSBuildParameters"
}

function Invoke-Docker-Build ([string]$ImageName, [string]$ImagePath, [string]$DockerBuildArgs = "") {
    echo "docker build -t $ImageName $ImagePath $DockerBuildArgs"
    Invoke-Expression "docker build -t $ImageName $ImagePath $DockerBuildArgs"
}

Invoke-MSBuild -MSBuildPath "dotnet msbuild" -MSBuildParameters ".\TradeAssist.Realtime.csproj /p:OutputPath=.\publish /p:Configuration=Release"
Invoke-Docker-Build -ImageName $ImageName -ImagePath "."