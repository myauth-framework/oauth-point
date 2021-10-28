-- --------------------------------------------------------
-- Хост:                         127.0.0.1
-- Версия сервера:               8.0.26 - MySQL Community Server - GPL
-- Операционная система:         Linux
-- HeidiSQL Версия:              11.0.0.5919
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;


-- Дамп структуры базы данных myauth-sso
-- CREATE DATABASE IF NOT EXISTS `myauth-sso` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
-- USE `myauth-sso`;

-- Дамп структуры для таблица myauth-sso.clients
CREATE TABLE IF NOT EXISTS `clients` (
  `id` char(32) NOT NULL COMMENT 'Client identifier (GUID)',
  `name` varchar(50) NOT NULL COMMENT 'Name',
  `password_hash` char(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT 'HEX HMAC-MD5 hash with salt',
  `enabled` enum('Y','N') CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL DEFAULT 'Y' COMMENT 'Indicated client enabled',
  `enabled_dt` datetime DEFAULT NULL COMMENT '''enabled'' flag actuallity date time',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='Registered clients';

-- Экспортируемые данные не выделены.

-- Дамп структуры для таблица myauth-sso.client_available_audiences
CREATE TABLE IF NOT EXISTS `client_available_audiences` (
  `id` int NOT NULL AUTO_INCREMENT COMMENT 'Row identifier',
  `uri` varchar(250) NOT NULL COMMENT 'URI',
  `client_id` char(32) NOT NULL COMMENT 'Client identifier',
  PRIMARY KEY (`id`),
  KEY `ClientAvailableAudienceToClient` (`client_id`),
  CONSTRAINT `ClientAvailableAudienceToClient` FOREIGN KEY (`client_id`) REFERENCES `clients` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='Available audiences for client';

-- Экспортируемые данные не выделены.

-- Дамп структуры для таблица myauth-sso.client_available_scopes
CREATE TABLE IF NOT EXISTS `client_available_scopes` (
  `id` int NOT NULL AUTO_INCREMENT COMMENT 'Row identifier',
  `name` varchar(50) NOT NULL COMMENT 'Scope name',
  `client_id` char(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Client identifier',
  PRIMARY KEY (`id`),
  KEY `ClientAvailableScopesToClient` (`client_id`),
  CONSTRAINT `ClientAvailableScopesToClient` FOREIGN KEY (`client_id`) REFERENCES `clients` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='Client available scopes';

-- Экспортируемые данные не выделены.

-- Дамп структуры для таблица myauth-sso.client_available_uri
CREATE TABLE IF NOT EXISTS `client_available_uri` (
  `id` int NOT NULL AUTO_INCREMENT COMMENT 'Row identifier',
  `uri` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT 'URI',
  `client_id` char(32) NOT NULL COMMENT 'Client identifier',
  PRIMARY KEY (`id`),
  KEY `ClientAvailableUriToClient` (`client_id`),
  CONSTRAINT `ClientAvailableUriToClient` FOREIGN KEY (`client_id`) REFERENCES `clients` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='Available redirect URI for client';

-- Экспортируемые данные не выделены.

-- Дамп структуры для таблица myauth-sso.login_sessions
CREATE TABLE IF NOT EXISTS `login_sessions` (
  `id` char(32) NOT NULL COMMENT 'Login session identifier (GUID)',
  `client_id` char(32) NOT NULL COMMENT 'Client identifier',
  `redirect_uri` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Request ''redirect_uri''',
  `scope` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Request ''scope''',
  `state` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL COMMENT 'Request ''state''',
  `create_dt` datetime NOT NULL COMMENT 'Createion date time',
  `error_code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL COMMENT 'Authorization error string code',
  `error_desc` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL COMMENT 'Authorization error description',
  `subject_id` varchar(50) DEFAULT NULL COMMENT 'Authorized subject identifier',
  `expiry` datetime NOT NULL COMMENT 'Session expirration date time',
  `auth_code` char(32) DEFAULT NULL COMMENT 'Authorization code',
  `auth_code_expiry` datetime DEFAULT NULL COMMENT 'Authorization code expiration date time',
  `auth_code_used` enum('Y','N') CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL COMMENT 'Indicates that authorixation code already used',
  `login_expiry` datetime NOT NULL COMMENT 'Login completion expiration date time',
  `completed` enum('Y','N') CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL COMMENT 'Indicates that session logon or error',
  `completed_dt` datetime DEFAULT NULL COMMENT '''completed'' field actuallity date time',
  `revoked` enum('Y','N') DEFAULT NULL COMMENT 'Indicates that the session is revoked',
  PRIMARY KEY (`id`),
  KEY `LoginSessionToClient` (`client_id`),
  KEY `LoginSessionToSubject` (`subject_id`),
  CONSTRAINT `LoginSessionToClient` FOREIGN KEY (`client_id`) REFERENCES `clients` (`id`),
  CONSTRAINT `LoginSessionToSubject` FOREIGN KEY (`subject_id`) REFERENCES `subjects` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='Login sessions';

-- Экспортируемые данные не выделены.

-- Дамп структуры для таблица myauth-sso.subjects
CREATE TABLE IF NOT EXISTS `subjects` (
  `id` varchar(50) NOT NULL COMMENT 'String identitifer',
  `enabled` enum('Y','N') CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Indicates is subject enabled',
  `enabled_dt` datetime DEFAULT NULL COMMENT '''enabled'' flag actiallity datetime',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='Subjects';

-- Экспортируемые данные не выделены.

-- Дамп структуры для таблица myauth-sso.subject_access_claims
CREATE TABLE IF NOT EXISTS `subject_access_claims` (
  `id` int NOT NULL AUTO_INCREMENT COMMENT 'Row id',
  `name` varchar(50) NOT NULL COMMENT 'Name',
  `value` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Value',
  `subject_id` varchar(50) NOT NULL COMMENT 'Subject identifier',
  PRIMARY KEY (`id`),
  KEY `SubjectAccessClaimsToSubject` (`subject_id`),
  CONSTRAINT `SubjectAccessClaimsToSubject` FOREIGN KEY (`subject_id`) REFERENCES `subjects` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='Additional subject claims for access token';

-- Экспортируемые данные не выделены.

-- Дамп структуры для таблица myauth-sso.subject_available_scopes
CREATE TABLE IF NOT EXISTS `subject_available_scopes` (
  `id` int NOT NULL AUTO_INCREMENT COMMENT 'Row identifier',
  `name` varchar(50) NOT NULL COMMENT 'Scope name',
  `subject_id` varchar(50) NOT NULL COMMENT 'Subject identifier',
  PRIMARY KEY (`id`),
  KEY `SubjectAvailabeScopesToSubject` (`subject_id`),
  CONSTRAINT `SubjectAvailabeScopesToSubject` FOREIGN KEY (`subject_id`) REFERENCES `subjects` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='Subject available scopes';

-- Экспортируемые данные не выделены.

-- Дамп структуры для таблица myauth-sso.subject_identity_claims
CREATE TABLE IF NOT EXISTS `subject_identity_claims` (
  `id` int NOT NULL AUTO_INCREMENT COMMENT 'Row id',
  `name` varchar(50) NOT NULL COMMENT 'Name',
  `value` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Value',
  `scope_id` varchar(50) NOT NULL COMMENT 'Scope string identifier',
  `subject_id` varchar(50) NOT NULL COMMENT 'Subject id',
  PRIMARY KEY (`id`),
  KEY `SubjectIdentityClaimsToSubject` (`subject_id`),
  CONSTRAINT `SubjectIdentityClaimsToSubject` FOREIGN KEY (`subject_id`) REFERENCES `subjects` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='Subject claims for identity';

-- Экспортируемые данные не выделены.

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
