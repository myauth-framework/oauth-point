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
CREATE DATABASE IF NOT EXISTS `myauth-sso` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `myauth-sso`;

-- Дамп структуры для таблица myauth-sso.claims
CREATE TABLE IF NOT EXISTS `claims` (
  `id` int NOT NULL AUTO_INCREMENT COMMENT 'Claim identifier',
  `session_scope` int NOT NULL,
  `name` varchar(50) NOT NULL COMMENT 'Claim name',
  `value` varchar(1024) NOT NULL COMMENT 'CLaim value',
  PRIMARY KEY (`id`),
  KEY `FK_Claim_To_SessionScope` (`session_scope`),
  CONSTRAINT `FK_Claim_To_SessionScope` FOREIGN KEY (`session_scope`) REFERENCES `session_scopes` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='Session scope claims';

-- Экспортируемые данные не выделены.

-- Дамп структуры для таблица myauth-sso.login_sessions
CREATE TABLE IF NOT EXISTS `login_sessions` (
  `id` char(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Session identifier (GUID)',
  `expiry` datetime NOT NULL COMMENT 'Expiration time',
  `client_id` char(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Client identifier (GUID)',
  `login_dt` datetime DEFAULT NULL COMMENT 'When user login successfully',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='Contains login sessions';

-- Экспортируемые данные не выделены.

-- Дамп структуры для таблица myauth-sso.session_initiations
CREATE TABLE IF NOT EXISTS `session_initiations` (
  `session_id` char(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT 'Session identifier (GUID)',
  `redirect_uri` varchar(2048) NOT NULL COMMENT 'Redirect URI from request',
  `state` varchar(512) DEFAULT NULL COMMENT 'Statefrom request',
  `authorization_code` char(32) DEFAULT NULL COMMENT 'Issued authorization code (GUID)',
  `error_code` varchar(50) DEFAULT NULL COMMENT 'Error code (from specification)',
  `erro_desription` varchar(1024) DEFAULT NULL COMMENT 'Error description',
  `complete_dt` datetime DEFAULT NULL COMMENT 'When initiation was completed',
  PRIMARY KEY (`session_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='Info about session initiation';

-- Экспортируемые данные не выделены.

-- Дамп структуры для таблица myauth-sso.session_scopes
CREATE TABLE IF NOT EXISTS `session_scopes` (
  `id` int NOT NULL AUTO_INCREMENT,
  `session_id` char(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Session identifier (GUID)',
  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Scope name',
  `requierd` enum('Y','N') NOT NULL COMMENT '''Y'' if scope contains in required scope list. Else - auth server send it but will be ignored',
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE KEY `UK_SessionScope` (`session_id`,`name`),
  CONSTRAINT `FK_SessScope_To_Session` FOREIGN KEY (`session_id`) REFERENCES `login_sessions` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='Scopes which related to sessions';

-- Экспортируемые данные не выделены.

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
