# SpecBoxTMS.Api

Этот репозиторий является частью [SpecBoxTMS](https://github.com/spec-box-tms)
системы управления функциональными требованиями (УФТы!)

В этом репозитории находится БД и веб-API для структурированного хранения информации о функциональных требованиях продукта. Приложение реализовано на C# (.NET7) и хранит данные в PostgreSQL.

## Концепция системы

Цель проекта: объединить проектирование требований к программному обеспечению с задачей на разработку и с планами тестирования.

Главная идей проекта заключается в переходе к описанию функциональных требований в виде простых утверждений, 
которые легко поддаются автоматизации тестирования или ручному тестированию.

Такие спецификации должны храниться вместе с исходным кодом приложения в виде yaml файлов, подробнее о которых можно прочитать в 
документации к репозиторию [SpecBoxTMS.Sync](https://github.com/spec-box-tms/sync).

Размещение спецификаций вместе с исходным кодом позволяет:

- Согласовать изменения кода и спецификаций
- Гарантировать сохранность спецификаций
- Хранить историю изменений спецификаций за счет системы контроля версий
- Производить сопоставлений имен утверждений с отчетом об автоматизированных тестах

Проект состоит из трех частей:

- Консольная утилита [SpecBoxTMS.Sync](https://github.com/spec-box-tms/sync) - выполняет валидацию содержимого yaml файлов спецификаций, 
сопоставление с отчетом об автотестах и синхронизацию с сервером требований.
- Сервер требований [SpecBoxTMS.Api](https://github.com/spec-box-tms/api) - обеспечивает хранение требований и истории тестовых запусков.
- Пользовательский интерфейс [SpecBoxTMS.Web](https://github.com/spec-box-tms/web) - пользовательский интерфейс для взаимодействия с системой
позволяет просматривать требования и объемы покрытия автоматизированными тестами, а так же выполнять тестовые запуски.
- Документация и Roadmap проекта [SpecBoxTMS.Docs](https://github.com/spec-box-tms/docs)

Данный проект является ответвлением от оригинального [SpecBox](https://github.com/spec-box)

## Как запустить

1. Соберите проект
   ```shell
   dotnet restore
   dotnet build
   ```
2. Запустите СУБД
   ```shell
   docker run --name postgres -e POSTGRES_PASSWORD=123 -e POSTGRES_DB=tms -p 5432:5432 -d postgres
   ```
3. Обновите структуру БД
   ```shell
   # установите утилиту для миграции структуры БД
   dotnet tool install -g thinkinghome.migrator.cli
   
   # обновите структуру БД
   migrate-database postgres "host=localhost;port=5432;database=tms;user name=postgres;password=123" ./SpecBox.Migrations/bin/Debug/net7.0/SpecBox.Migrations.dll
   ```
4. Запустите приложение
   ```shell
   export ASPNETCORE_ENVIRONMENT=Development
   export ConnectionStrings__default="host=localhost;port=5432;database=tms;user name=postgres;password=123"
   dotnet ./SpecBox.WebApi/bin/Debug/net7.0/SpecBox.WebApi.dll --urls=http://+:8080
   ```
5. Откройте в браузере адрес http://localhost:8080/swagger

## Публичный контейнер

```bash
docker run -p 8080:80 -ti \
 --link postgres:postgres \
 -e ConnectionStrings__default='host=postgres;port=5432;database=tms;user name=postgres;password=123' \
 snitkody/spec-box-tms-api:latest
 ```

Можно использовать переменную AUTO_MIGRATE=true для автоматического применения миграции БД перед запуском

## Самостоятельная сборка контейнера

```bash
# сборка
docker build -t spec-box-tms-api:0.0.1 .

# локальный запуск
docker run -p 8080:80 -ti \
 --link postgres:postgres \
 -e ConnectionStrings__default='host=postgres;port=5432;database=tms;user name=postgres;password=123' \
 spec-box-api:0.0.1
```

### Информация

- документация API: http://localhost:8080/swagger
