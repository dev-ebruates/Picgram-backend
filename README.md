# Picgram-backend

1. ef core kurulumu
dotnet tool install --global dotnet-ef
2. dal in oldugu dizine git
cd ...../Infrastructure
3. migration
dotnet ef migrations add [migration_name] --startup-project [../project] 