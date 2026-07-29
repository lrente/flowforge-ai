Write-Host ""
Write-Host "==============================="
Write-Host " FlowForge AI Bootstrap"
Write-Host "==============================="
Write-Host ""

# -----------------------------
# Git
# -----------------------------
if (!(Get-Command git -ErrorAction SilentlyContinue)) {
    Write-Host "❌ Git não encontrado."
} else {
    Write-Host "✅ Git instalado"
}

# -----------------------------
# Docker
# -----------------------------
if (!(Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Host "❌ Docker não encontrado."
} else {
    Write-Host "✅ Docker instalado"
}

# -----------------------------
# Node
# -----------------------------
if (!(Get-Command node -ErrorAction SilentlyContinue)) {

    Write-Host ""
    Write-Host "Node.js não encontrado."
    Write-Host ""
    Write-Host "Instala a versão LTS:"
    Write-Host ""
    Write-Host "https://nodejs.org"
    exit

}

Write-Host "✅ Node encontrado"

# -----------------------------
# NPM
# -----------------------------
if (!(Get-Command npm -ErrorAction SilentlyContinue)) {

    Write-Host "❌ npm não encontrado"
    exit

}

Write-Host "✅ npm encontrado"

# -----------------------------
# .NET
# -----------------------------
if (!(Get-Command dotnet -ErrorAction SilentlyContinue)) {

    Write-Host "❌ .NET SDK não encontrado"
    exit

}

Write-Host "✅ .NET encontrado"

# -----------------------------
# Criar Frontend
# -----------------------------
if (!(Test-Path ".\frontend")) {

    Write-Host ""
    Write-Host "Criando Frontend..."

    npm create vite@latest frontend -- --template react-ts

}

# -----------------------------
# Instalar Frontend
# -----------------------------
Set-Location frontend

npm install

npm install react-router-dom axios

npm install -D tailwindcss @tailwindcss/vite

Set-Location ..

# -----------------------------
# Backend
# -----------------------------
if (!(Test-Path ".\backend")) {

    mkdir backend | Out-Null

}

Set-Location backend

if (!(Test-Path ".\FlowForge.sln")) {

    dotnet new sln -n FlowForge

    dotnet new webapi -n FlowForge.Api

    dotnet new classlib -n FlowForge.Domain

    dotnet new classlib -n FlowForge.Application

    dotnet new classlib -n FlowForge.Infrastructure

    dotnet sln add **/*.csproj

}

Set-Location ..

Write-Host ""
Write-Host "==============================="
Write-Host "Projeto criado com sucesso!"
Write-Host "==============================="