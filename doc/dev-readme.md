# MyAuth.OAuthPoint - Руководство Разработчика

## Акторы

* `SSO` - Single Sign-On - точка единого входа на базе `MyAuth.OAuthPoint`;
* `Client` - информационная система, пользующаяся `SSO`, т.е. являющаяся клиентом для `SSO`;
* `LoginService` - кастомная часть SSO, реализующая вход пользователей. Включает визуальную часть;
* `UserAgent` - приложение, с которым работает конечный пользователь. Является клиентом для информационной системы-потребителя (`Client`). 

## Процесс авторизации

### Части процесса

* `LoginSession` - состояние sso на время входа пользователя. Cодержит следующие реквизиты (в терминах БД):
  * `id` - идентификатор сессии (GUID)
  * `expiry` - дата и время истечения жизни сессии
  * `client_id` - идентификатор клиента
  * `status` - состояние сессии:
    * `waiting` - ожидает входа пользователя
    * `failed` - произошла ошибка. предусмотренная [oidc-v1#AuthError](https://openid.net/specs/openid-connect-core-1_0.html#AuthError)
    * `success` - был осуществлён успешный вход
  * `error_code` - код ошибки в случае завершения с ошибкой [oidc-v1#AuthError](https://openid.net/specs/openid-connect-core-1_0.html#AuthError)
  * `error_desc`- текстовое описание ошибки в случае завершения с ошибкой
  * `state` - кастомное состояние, которое определяет `LoginSerice`
* `AuthorizationSession` - состояние на время, когда пользователь считается авторизованным. Содержит реквизиты (в терминах БД):
  * `id` - идентификатор сессии (GUID)
  * `login_id` - идентификатор сессии логина, которая породила данную сессию
  * `expiry` - дата и время истечения жизни сессии
  * `subject_id` - идентификатор залогиненного пользователя
  * `revoked` - сессия была закрыта
* `TokenSession` - состояние, связанное с выпуском токенов.  Содержит реквизиты (в терминах БД):
  * `id` - идентификатор сессии (GUID)
  * `auth_id` - идентификатор сессии авторизации
  * `expiry` - дата и время истечения жизни сессии
  * `client_id` - идентификатор клиента
  * `scope` - из запроса авторизации [Authentication Request](https://openid.net/specs/openid-connect-core-1_0.html#AuthRequest)
  * `redirect_uri` -  из запроса авторизации [Authentication Request](https://openid.net/specs/openid-connect-core-1_0.html#AuthRequest)
  * `state` - из запроса авторизации [Authentication Request](https://openid.net/specs/openid-connect-core-1_0.html#AuthRequest)
  * `auth_code` - код авторизации, применяемый в [Code Flow](https://openid.net/specs/openid-connect-core-1_0.html#CodeFlowSteps)
  * `auth_code_expiry` - дата и время истечения жизни кода авторизации
* `AuthCode` - код авторизации, применяемый в [Code Flow](https://openid.net/specs/openid-connect-core-1_0.html#CodeFlowSteps)Содержит реквизиты (в терминах БД):
  * `id` - значение кода (GUID)
  * `token_sess_id` - идентиикатор сессии токена
  * `expiry` - дата и время истечения срока действия

