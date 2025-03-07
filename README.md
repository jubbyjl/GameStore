# GameStore
A simple store that allows users to publish, purchase and review games.

This project was purely a learning experience and served as an introduction to web development.

#### Limitations
- Games do not have display images yet

#### Screenshot
![ss](https://github.com/user-attachments/assets/6ff61b7d-144d-4934-8831-62e42bfc2cdc)

## Built With
- ASP.NET Core
- EF Core
- Bootstrap

## Setup
### Prerequisites
- [.NET SDK 8.0](https://dotnet.microsoft.com/download)
### Install
```
git clone https://github.com/jubbyjl/GameStore.git
```
### Run locally
- Trust the HTTPS development certificate
```
dotnet dev-certs https --trust
```
- Build and run the app locally
```
dotnet run --launch-profile https
```
- Go to `https://localhost:7188` to use the app
- Press Ctrl + C at the command prompt to stop the app
