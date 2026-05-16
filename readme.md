# FufloBank - Модуль банковских транзакций

Модуль сделан в рамках учебной практики.

## Выполняемые функции

- Регистрация и аутентификация пользователей (по номеру телефона и паролю)
- Управление счетами в трех валютах: RUB, USD, CRYPTO
- P2P переводы между пользователями с проверкой лимитов и подтверждением
- Оплата товаров/услуг по MCC кодам с начислением кэшбека
- Конвертация валют по актуальным курсам между своими счетами
- История транзакций с фильтрацией по дате, типу, категории и сумме
- Формирование выписок за период
- Автоматическое обновление курсов валют из внешнего источника
- Кэшбек до 5% по категориям (Рестораны, Такси, Другие)

## Технологии

- C# .NET 10.0
- ASP.NET Core Web API (REST контроллеры)
- Dapper/Npgsql для работы с БД
- PostgreSQL
- HTML/CSS/JavaScript для фронтенда (динамические страницы)

## База данных

При первом запуске автоматически создаются таблицы:

- **users** - профили пользователей (id, full_name, phone, password_hash, status, created_at, monthly_cashback_limit, current_monthly_cashback)
- **accounts** - счета пользователей (id, user_id, currency, balance, created_at, is_active)
- **exchange_rates** - курсы валют (id, from_currency, to_currency, rate, updated_at)
- **mcc_codes** - MCC коды категорий (code, category, description)
- **cashback_categories** - категории кэшбэка (id, category, percentage, is_active)
- **transactions** - история транзакций (id, from_account_id, to_account_id, from_user_id, to_user_id, amount, currency, converted_amount, category, status, created_at, completed_at, description, cashback_amount, mcc_code)

## Установка и запуск

1. Клонировать репозиторий:

```bash
git clone https://github.com/charderrrrr/FufloBank
cd FufloBank/App 
```

2. Установить зависимости:

```bash
dotnet restore
dotnet add package Dapper
dotnet add package BCrypt.Net-Next
dotnet add package Npgsql
```

3. Создать бд в Postgre, настроить подключение в Program.cs
```sql
CREATE DATABASE fuflobank;
```

4. Запустить проект

```bash
dotnet run
```

5. Вбить этот адрес в браузере

```text
http://localhost:5002
```

## Структура проекта

```text
App/
├── Controllers/       # REST API контроллеры
│   ├── AuthController.cs
│   ├── BalanceController.cs
│   ├── ConversionController.cs
│   ├── HistoryController.cs
│   ├── PaymentController.cs
│   └── TransferController.cs
├── Data/
│   ├── DatabaseInitializer.cs
│   └── Repositories/
├── Models/
│   └── Enums/
├── Services/          # Бизнес-логика
├── UI/
│   └── Services/
│       └── SessionManager.cs
├── Validators/
├── wwwroot/           # Статические файлы фронтенда
│   ├── css/
│   ├── js/
│   │   └── app.js
│   ├── index.html
│   ├── login.html
│   ├── register.html
│   ├── balance.html
│   ├── payment.html
│   ├── transfer.html
│   ├── conversion.html
│   └── history.html
├── FufloBankModule.cs
├── Program.cs
└── App.csproj
```

## API Эндпоинты

| Метод | Эндпоинт | Описание |
|-------|----------|----------|
| POST | `/api/auth/login` | Вход по телефону и паролю |
| POST | `/api/auth/register` | Регистрация нового пользователя |
| POST | `/api/auth/logout` | Выход из системы |
| POST | `/api/payment` | Оплата по MCC коду |
| POST | `/api/transfer` | P2P перевод |
| GET | `/api/transfer/check-limit` | Проверка лимита (нужен ли код подтверждения) |
| POST | `/api/conversion` | Конвертация валют |
| GET | `/api/balance` | Получение балансов по всем счетам |
| GET | `/api/history` | История транзакций с фильтрацией |
| GET | `/api/statement` | Выписка за период |
