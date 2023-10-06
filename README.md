# Task: ASP.NET Core Web API

## Описание

Этот проект представляет собой ASP.NET Core Web API, разработанный в соответствии с задачей, представленной в тестовом задании. Он предоставляет CRUD (Create, Read, Update, Delete) операции для управления списком пользователей и их ролями. Каждый пользователь имеет атрибуты Id, Имя (Name), Возраст (Age), Email и связанную сущность "Роль" (Role).

## Требования

- ASP.NET Core Web API
- C# и .NET Core
- Модель данных для пользователя (User) с атрибутами Id, Name, Age, Email и связанной сущностью "Роль" (Role)
- Контроллер (UserController) с методами для выполнения CRUD операций и бизнес-логикой валидации данных
- Использование Entity Framework Core для доступа к данным и сохранения пользователей и ролей в базе данных
- Документация API с использованием инструментов Swagger

# Инструкции

Для запуска этого проекта, выполните следующие шаги:

1. Склонируйте репозиторий на свой компьютер.
2. Откройте проект в вашей среде разработки (например, Visual Studio).

### Настройка подключения к базе данных

3. В файле `appsettings.json` измените строку подключения к базе данных в разделе `ConnectionStrings`. Замените следующую строку:

```json
"PostgreSQL": "Server=YOUR_SERVER;Port=YOUR_PORT;Database=TestTask;User Id=postgres;Password=YOUR_PASSWORD;"




