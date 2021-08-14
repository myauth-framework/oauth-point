# MyAuth.OAuthPoint

[![Docker image](https://img.shields.io/docker/v/ozzyext/myauth-oauth-point?sort=semver)](https://hub.docker.com/r/ozzyext/myauth-oauth-point)

## Обзор 

Серверное приложение для организации инфраструктуры авторизации на базе протокола [OAuth2](https://tools.ietf.org/html/rfc6749) и [JWT](https://tools.ietf.org/html/rfc7519) токенах. Отвечает за работу с токенами:

* выпуск токенов доступа
* перевыпуск токенов доступа
* отзыв токенов обновления



Ниже изображена блок-схема участников процесса авторизации на примере простейшей конфигурации.

![Блок-схема](./doc/mylab-oauth-blocks.png)



На схеме:

* **Client** - клиентское приложение: 
  * классическое веб-приложение
  * SPA (одностраничное приложение)
  * desktop приложение;

* **Browser** - интернет браузер, хостящий веб-приложение или встроенный браузер в desktop приложение;

* **Nginx** - веб-сервер, предоставляющий доступ к сервисам авторизации;

* **Login service** - сервис, осуществляющий идентификацию пользователя и передачу данных о нём в `MyLab.OAuthPoint`;

* **MyLab.OAuthPoint** - выпускает, перевыпускает токены доступа (для `Client`), отзывает токены обновления (для `Login service`);

* **Redis** - хранилище данных для работы с токенами.



Для реализации инфраструктуры авторизации необходимо:

* реализовать `Login service`, который должен принимать от пользователя параметры авторизации, идентифицировать его, передавать информацию о пользователе в `MyAuth.OAuthPoint`;
* развернуть все серверные компоненты: `Login service`, `MyAuth.OAuthPoint`, `Redis`
* настроить ` MyAuth.OAuthPoint`:
  * подключение к `Redis`
  * время жизни токена доступа
  * время жизни токена обновления
  * ключ подписи токенов

## Авторизация

 

![Диаграма последовательности](./doc/mylab-oauth-sequence.png)



Алгоритм доступа к ресурсу со стороны пользователя:

1. Клиент проверяет наличие валидного токена доступа. Валидность проверяется по времени экспирации;
2. Если валидный токен не обнаружен, но есть токен перевыпуска, клиент отправляет запрос в `MyAuth.OAuthPoint` с токеном обновления для перевыпуска токена доступа;
3. `MyAuth.OAuthPoint` проверяет актуальность токена доступа, формирует и выдаёт новый токен доступа и токен обновления;
4. При успешном ответе, клиент сохраняет токены;
5. Если не удалось перевыпустить токен доступа, клиент перенаправляет пользователя на страницу авторизации. Это может быть редирект на целевую страницу из веб-приложения или, в случае с desktop приложением - переход на целевую страницу в окне встроенного браузера. В запросе передаётся `URL`, на который следует вернуться после получения параметров для дальнейшей авторизации. В случает с `desktop/mobile`-приложением - это фиксированный фейковый URL для отлавливания этого перехода приложением;
6. Пользователь вводит данные авторизации и отправляет их в `Login service`;
7.  `Login service` идентифицирует пользователя и отправляет в `MyAuth.OAuthPoint` информацию о факте успешного логина пользователя - данные, которые должны быть включены в токен доступа;
8. `MyAuth.OAuthPoint` сохраняет данные о факте успешного логина и выдаёт `код авторизации`;
9. `Login service` переводит браузер на `URL`, указанный на шаге 5 и снабжает его `кодом авторизации`;
10. Браузер загружает клиент или `desktop/mobile`-приложение перехватывает переход на указанный `URL`. Таким образом приложение получает `код авторизации`;
11. Клиент передаёт полученный `код авторизации` в `MyAuth.OAuthPoint`
12. `MyAuth.OAuthPoint` по `коду авторизации` определяет данные пользователя, формирует и выдаёт токен доступа и токен обновления;
13. Запрос к защищённому ресурсу с токеном доступа;
14. Успешное выполнение операции.

## API

### POST /login

Получает информацию о пользователе для дальнейшего выпуска токенов. Фактически получает информацию о пользователе, прошедшем успешную авторизацию в `Login service`. Ответ - строка - код авторизации.

Пример запроса:

```json
{
  "ClientId": "android-default",
  "RedirectUri": "http://host1.com/auth-code",
  "Subject": "user-id",
  "CodeChallenge": "2FeO34RYzgb7xbt2pYxcpA==",
  "CodeChallengeMethod": "md5",
  "Roles": [
    "admin"
  ],
  "Audience": [
    "host1.com",
    "host2.com"
  ],
  "Claims": [
    {
      "Name": "name",
      "Value": "Jhon"
    },
    {
      "Name": "surname",
      "Value": "Ololoevich"
    }
  ]
}

```

* **ClientId** - идентификатор клиента - приложения;
* **RedirectUri** - адрес для перехода после успешного входа. Туда будет передан `код авторизации`
* **Subject** - идентификатор субъекта (пользователя);
* **CodeChallenge** - хэш заготовленной сервисом входа последовательности для проверки достоверности кода авторизации;
* **CodeChallengeMethod** - хэш-функция для формирования `CodeChallenge `. Поддерживается только `md5`;
* **Roles** - массив ролей субъекта (пользователя). Попадает в токен в виде утверждений (`Claims`) с именами http://schemas.microsoft.com/ws/2008/06/identity/claims/role;
* **Audience** - список хостов, куда разрешён доступ субъекту (пользователю);
* **Claims** - прочие утверждения.

### POST /token

Получает запросы выпуска и перевыпуска токена доступа.

#### Запрос выпуска

Пример запроса:

```json
{
  "GrantType": "authorization_code",
  "CodeVerifier": "qwerty",
  "AuthCode": "5b28afded2a149bc8436f70f2ecc4c05",
  "ClientId": "android-default"
}
```

* **GrantType** - тип запроса. `authorization_code` - запрос токена по коду авторизации;
* **CodeVerifier** - оригинальная последовательность, хэш которой был получен сервисом ранее, при вызове `/login`, и которая была передана клиенту в результате успешной авторизации;
* **AuthCode** - код авторизации, полученный клиентом от `Login service` в результате успешной авторизации;
* **ClientId** - идентификатор клиента - приложения.

#### Запрос первыпуска

Пример запроса:

```json

{
  "GrantType": "refresh_token",
  "CodeVerifier": "qwerty",
  "RefreshToken": "YmE4ZGU2YzRjY2Y4NDU5NTlhMmQwMWY3NGVmZjIzMjY",
  "ClientId": "android-default"
}

```

* **GrantType** - тип запроса. `refresh_token`- запрос токена по токену обновления;
* **CodeVerifier** - оригинальная последовательность, хэш которой был получен сервисом ранее, при вызове `/login`, и которая была передана клиенту в результате успешной авторизации;
* **RefreshToken**- токен обновления, полученный при одном из предвдущих выпусков токена доступа;
* **ClientId** - идентификатор клиента - приложения.

#### Ответ

Пример успешного ответа:

```json
{
  "access_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1cm46dGVzdC1jbGFpbXM6Zm9vIjoiZm9vIiwidXJuOnRlc3QtY2xhaW1zOmJhciI6ImJhciIsIm5iZiI6MTU4MzMxNTcyNywiZXhwIjoxNTgzNDAyMTI3LCJpc3MiOiJpc3N1ZXIiLCJhdWQiOiJhdWRpZW5jZSJ9.MmiFj0dTd5e0JsKOd8KzIKuY_e4iop2wMjDoYyaiUaE",
  "token_type": "bearer",
  "expires_in": 60,
  "refresh_token": "YmE4ZGU2YzRjY2Y4NDU5NTlhMmQwMWY3NGVmZjIzMjY"
}
```

* **access_token** - токен доступа
* **token_type** - тип выпущенного токена
* **expires_in** - время жизни токена доступа в секундах
* **refresh_token** - токен обновления

Пример ошибки:

```json
{
  "error": "invalid_client",
  "error_description": "Client not found"
}
```

* error - код ошибки
* error_description - свободное текстовое описание ошибки

#### Коды ошибок

Выдержка из [RFC](https://tools.ietf.org/html/rfc6749#section-4.1.2.1):

```
         invalid_request
               The request is missing a required parameter, includes an
               invalid parameter value, includes a parameter more than
               once, or is otherwise malformed.
         unauthorized_client
               The client is not authorized to request an authorization
               code using this method.

         access_denied
               The resource owner or authorization server denied the
               request.

         unsupported_response_type
               The authorization server does not support obtaining an
               authorization code using this method.

         invalid_scope
               The requested scope is invalid, unknown, or malformed.

         server_error
               The authorization server encountered an unexpected
               condition that prevented it from fulfilling the request.
               (This error code is needed because a 500 Internal Server
               Error HTTP status code cannot be returned to the client
               via an HTTP redirect.)

         temporarily_unavailable
               The authorization server is currently unable to handle
               the request due to a temporary overloading or maintenance
               of the server.  (This error code is needed because a 503
               Service Unavailable HTTP status code cannot be returned
               to the client via an HTTP redirect.)
```

### DELETE /refresh-tokens/{token}

Отзывает токен обновления.

**token** - токен обновления.

### DELETE /subjects/{subject}/refresh-tokens

Отзывает все токены обновления указанного субъекта (пользователя).

**subject** - субъект (идентификатор пользователя).

## Настройка

### Настройки приложения

Конфигурационный файл `appsettings.json` имеет следующее содержание:

```json
{
  "Redis": {
    "Host": "127.0.0.1",
    "Port": "6379",
    "Password": "secret-1",
    "DbIndex": "0",
    "Encoding": "utf-8"
  },
  "TokenIssuing": {
    "Issuer": "MyAuth.OAuthPoint",
    "AccessTokenLifeTimeMin": "10",
    "RefreshTokenLifeTimeDays": "10",
    "Secret": "secret-2"
  }
}
```

* **Redis** - раздел, определяющий конфигурацию подключения к `Redis`:
  * **Host** - IP или имя хоста;
  * **Port** (опциональный) - порт, 6379 - по умолчанию;
  * **Password (опциональный)** - пароль, пустой - по умолчанию;
  * **DbIndex** (опциональный)  - индекс базы данных, 0 - по умолчанию;
  * **Encoding** (опциональный)  - кодировка, по умолчанию - установленная для приложения по умолчанию;
* **TokenIssuing** - раздел, определяющий конфигурацию выпуска токенов:
  * **Issuer** - имя издателя, `MyAuth.OAuthPoint` - по умолчанию;
  * **AccessTokenLifeTimeMin** - время жизни токена доступа в минутах;
  * **RefreshTokenLifeTimeDays** - время жизни токена обновления в днях;
  * **Secret** - ключ для создания подписи токена.

### Список клиентов

Список клиентов - файл с содержанием в формате `JSON`. Содержание - массив сущностей, описывающих параметры, связанные с клиентами.

Пример содержимого файла-списка клиентов:

```json
[
  {
    "Id": "test-app1",
    "Verification": "false"
  },
  {
    "Id": "test-app2",
    "Verification": "true"
  },
  {
    "Id": "test-app3",
    "Verification": "true",
    "AllowUris": [
      "http://dev.host.ru",
      "http://test.host.ru",
      "http://prod.host.ru"
    ]
  },
  {
    "Id": "test-app4",
    "Verification": "true",
    "AllowUris": [
      "http://test.host2.ru"
    ]
  }
]
```

* **Id** - идентификатор клиента, передаваемый при авторизации;
* **Verification** - обязательность проверки по `CodeChallenge`;
* **AllowUris** - массив `URI`, куда разрешена передача кода авторизации для данного клиента.

## Развёртывание

Сервис может быть развёрнут на базе `docker`. [Проект в docker-hub](https://hub.docker.com/r/ozzyext/myauth-oauth-point).

```bash
docker pull ozzyext/myauth-oauth-point:latest

docker run --rm -p 8080:80 -v ./host-path/app.config:/app/app.config -v ./host-path/clients.json:/app/clients.json ozzyext/myauth-oauth-point:latest
```

