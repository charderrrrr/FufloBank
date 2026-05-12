# FinTech Core - Модуль банковских транзакций

Модуль сделан в рамках учебной практики.
Выполняемые функции: обработка банковских транзакций с поддержкой комиссий, валидацией и историей операций.

## Технологии

- C# .NET 10.0
- Spectre.Console для CLI интерфейса
- Dapper/Npgsql для работы с БД
- PostgreSQL

## База данных

При первом запуске автоматически создаются таблицы:

- **users** - профили пользователей (id, full_name, phone, status, created_at, monthly_cashback_limit, current_monthly_cashback)
- **accounts** - счета пользователей (id, user_id, currency, balance, created_at, is_active)
- **exchange_rates** - курсы валют (id, from_currency, to_currency, rate, updated_at)
- **mcc_codes** - MCC коды категорий (code, category, description)
- **cashback_categories** - категории кэшбэка (id, category, percentage, is_active)
- **transactions** - история транзакций (id, from_account_id, to_account_id, from_user_id, to_user_id, amount, currency, converted_amount, category, status, created_at, completed_at, description, cashback_amount, mcc_code)

## Установка

Клонировать репозиторий:

```
git clone 
cd 
cd 
```

Установить зависимости:

```
dotnet restore
```
Перед запуском или ручками создать файл .env со своими настройками (см. файл .example_env), либо запустить и отредачить созданный файлик. 

Запуск:

```
dotnet run
```
