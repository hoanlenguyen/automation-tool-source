-- MySQL dump 10.13  Distrib 5.5.62, for Win64 (AMD64)
--
-- Host: mysqltestdb22.mysql.database.azure.com    Database: intranet_testdb
-- ------------------------------------------------------
-- Server version	5.6.47.0

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__efmigrationshistory`
--

LOCK TABLES `__efmigrationshistory` WRITE;
/*!40000 ALTER TABLE `__efmigrationshistory` DISABLE KEYS */;
INSERT INTO `__efmigrationshistory` VALUES ('20220822033251_initialize_DB','6.0.6'),('20220823012306_StaffRecord_add_OtherDepartment','6.0.6'),('20220824095701_StaffRecord_add_NumberOfDays_NumberOfHours','6.0.6'),('20220825042925_StaffRecord_add_LateAmount_Department_WorkingHours','6.0.6'),('20220826020122_StaffRecord_add_Fine','6.0.6'),('20220831075252_StaffRecord_add_CalculationAmount','6.0.6'),('20220908091156_Users_add_BankAccountName','6.0.6'),('20220909072139_add_Currencies','6.0.6'),('20220909081639_Currencies_update_nullable_CurrencyCode','6.0.6'),('20220914063454_Roles_add_IsSuperAddmin','6.0.6'),('20220914071705_Brands_add_IsAllBrand','6.0.6'),('20220914101428_Ranks_add_Level','6.0.6'),('20220923062532_Users_update_BirthDate_Nullable','6.0.6'),('20220926042257_Role_add_Departments','6.0.6');
/*!40000 ALTER TABLE `__efmigrationshistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetuserclaims`
--

DROP TABLE IF EXISTS `aspnetuserclaims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `aspnetuserclaims` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` int(11) NOT NULL,
  `ClaimType` longtext,
  `ClaimValue` longtext,
  PRIMARY KEY (`Id`),
  KEY `IX_AspNetUserClaims_UserId` (`UserId`),
  CONSTRAINT `FK_AspNetUserClaims_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetuserclaims`
--

LOCK TABLES `aspnetuserclaims` WRITE;
/*!40000 ALTER TABLE `aspnetuserclaims` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetuserclaims` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetuserlogins`
--

DROP TABLE IF EXISTS `aspnetuserlogins`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `aspnetuserlogins` (
  `LoginProvider` varchar(255) NOT NULL,
  `ProviderKey` varchar(255) NOT NULL,
  `ProviderDisplayName` longtext,
  `UserId` int(11) NOT NULL,
  PRIMARY KEY (`LoginProvider`,`ProviderKey`),
  KEY `IX_AspNetUserLogins_UserId` (`UserId`),
  CONSTRAINT `FK_AspNetUserLogins_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetuserlogins`
--

LOCK TABLES `aspnetuserlogins` WRITE;
/*!40000 ALTER TABLE `aspnetuserlogins` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetuserlogins` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetusertokens`
--

DROP TABLE IF EXISTS `aspnetusertokens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `aspnetusertokens` (
  `UserId` int(11) NOT NULL,
  `LoginProvider` varchar(255) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Value` longtext,
  PRIMARY KEY (`UserId`,`LoginProvider`,`Name`),
  CONSTRAINT `FK_AspNetUserTokens_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetusertokens`
--

LOCK TABLES `aspnetusertokens` WRITE;
/*!40000 ALTER TABLE `aspnetusertokens` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetusertokens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `banks`
--

DROP TABLE IF EXISTS `banks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `banks` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(150) NOT NULL,
  `CreationTime` datetime(6) NOT NULL,
  `CreatorUserId` int(11) DEFAULT NULL,
  `LastModificationTime` datetime(6) DEFAULT NULL,
  `LastModifierUserId` int(11) DEFAULT NULL,
  `Status` tinyint(1) NOT NULL,
  `IsDeleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `banks`
--

LOCK TABLES `banks` WRITE;
/*!40000 ALTER TABLE `banks` DISABLE KEYS */;
INSERT INTO `banks` VALUES (1,'Mock','2022-01-01 00:00:00.000000',NULL,NULL,NULL,0,1),(2,'MyBank','2022-08-22 11:11:21.728396',1,'2022-09-08 04:29:18.591335',3,0,0),(3,'ABC','2022-08-22 11:11:32.379326',1,NULL,NULL,1,0),(4,'KBANK','2022-08-26 08:15:30.577096',1,NULL,NULL,1,0),(5,'BBL','2022-08-26 08:15:35.286748',1,NULL,NULL,1,0),(6,'TTB','2022-08-30 05:18:38.902062',3,NULL,NULL,1,0),(7,'SCB','2022-08-30 05:31:03.119383',3,NULL,NULL,1,0),(8,'TCB','2022-08-30 06:37:06.045816',3,'2022-09-09 06:49:53.130072',1,1,0),(9,'Test Bank','2022-09-12 04:17:42.961939',3,NULL,NULL,1,0);
/*!40000 ALTER TABLE `banks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `brandemployees`
--

DROP TABLE IF EXISTS `brandemployees`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `brandemployees` (
  `BrandId` int(11) NOT NULL,
  `EmployeeId` int(11) NOT NULL,
  PRIMARY KEY (`BrandId`,`EmployeeId`),
  KEY `IX_BrandEmployees_EmployeeId` (`EmployeeId`),
  CONSTRAINT `FK_BrandEmployees_Brands_BrandId` FOREIGN KEY (`BrandId`) REFERENCES `brands` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BrandEmployees_Users_EmployeeId` FOREIGN KEY (`EmployeeId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `brandemployees`
--

LOCK TABLES `brandemployees` WRITE;
/*!40000 ALTER TABLE `brandemployees` DISABLE KEYS */;
INSERT INTO `brandemployees` VALUES (2,1),(3,1),(4,1),(7,3),(3,4),(3,5),(4,5),(4,6),(3,7),(4,7),(10,7),(13,7),(3,8),(4,8),(6,9),(9,10),(7,11),(6,12),(5,13),(5,14),(7,15),(7,16),(5,17),(8,17),(5,18),(6,19),(8,23),(6,24),(8,26),(9,26),(10,26),(11,26),(12,26),(14,26),(4,27),(4,28),(3,29),(8,30),(9,30),(10,30),(11,30),(12,30),(14,30),(6,31),(4,32),(7,33),(4,34),(7,35);
/*!40000 ALTER TABLE `brandemployees` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `brands`
--

DROP TABLE IF EXISTS `brands`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `brands` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(150) NOT NULL,
  `CreationTime` datetime(6) NOT NULL,
  `CreatorUserId` int(11) DEFAULT NULL,
  `LastModificationTime` datetime(6) DEFAULT NULL,
  `LastModifierUserId` int(11) DEFAULT NULL,
  `Status` tinyint(1) NOT NULL,
  `IsDeleted` tinyint(1) NOT NULL,
  `IsAllBrand` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=23 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `brands`
--

LOCK TABLES `brands` WRITE;
/*!40000 ALTER TABLE `brands` DISABLE KEYS */;
INSERT INTO `brands` VALUES (1,'Mock','2022-01-01 00:00:00.000000',NULL,NULL,NULL,0,1,0),(2,'BrandA','2022-08-22 11:11:53.522725',1,'2022-09-09 03:50:52.227631',3,1,1,0),(3,'BrandB','2022-08-22 11:12:00.542486',1,NULL,NULL,1,0,0),(4,'BrandC','2022-08-22 11:12:07.486768',1,NULL,NULL,1,0,0),(5,'T99','2022-08-26 08:14:33.563325',1,NULL,NULL,1,0,0),(6,'-','2022-08-26 08:14:59.064840',1,'2022-09-12 10:35:29.851935',3,1,0,0),(7,'All Brand','2022-08-26 08:15:16.858601',1,'2022-09-27 04:47:59.121129',1,1,0,1),(8,'R99','2022-08-29 06:55:35.474616',3,NULL,NULL,1,0,0),(9,'W99','2022-08-29 06:55:49.714029',3,NULL,NULL,1,0,0),(10,'GW99','2022-08-30 06:20:09.044098',3,NULL,NULL,1,0,0),(11,'PGW55','2022-08-30 06:20:16.903288',3,NULL,NULL,1,0,0),(12,'B666','2022-08-30 06:28:53.222781',3,NULL,NULL,1,0,0),(13,'U99','2022-08-30 06:29:02.091984',3,'2022-08-31 04:00:30.943034',1,1,0,0),(14,'PGV555','2022-08-30 06:29:48.089722',3,'2022-09-14 10:12:14.667144',3,1,0,0),(15,'BrandAB','2022-09-09 09:11:23.939820',3,'2022-09-09 09:11:54.748340',3,1,1,0),(16,'Test12','2022-09-12 04:50:08.302771',3,'2022-09-12 04:50:55.443004',3,0,1,0),(17,'New','2022-09-13 03:25:01.617467',3,'2022-09-13 03:25:50.086152',3,1,1,0),(18,'Delete Brand','2022-09-14 03:24:45.620805',3,'2022-09-14 03:26:44.275832',3,1,1,0),(19,'To delete','2022-09-14 03:36:35.389777',3,'2022-09-14 03:37:43.069021',3,1,1,0),(20,'DeletedBrand','2022-09-14 03:56:00.524434',1,'2022-09-14 03:56:32.012345',1,1,1,0),(21,'to be deleted','2022-09-14 05:41:04.027510',3,'2022-09-14 05:41:10.607781',3,1,1,0),(22,'to be deleted','2022-09-14 05:41:04.750577',3,'2022-09-14 05:42:48.086475',3,1,1,0);
/*!40000 ALTER TABLE `brands` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `currencies`
--

DROP TABLE IF EXISTS `currencies`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `currencies` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(150) NOT NULL,
  `CurrencyCode` varchar(10) DEFAULT NULL,
  `CurrencySymbol` varchar(10) NOT NULL,
  `CreationTime` datetime(6) NOT NULL,
  `CreatorUserId` int(11) DEFAULT NULL,
  `LastModificationTime` datetime(6) DEFAULT NULL,
  `LastModifierUserId` int(11) DEFAULT NULL,
  `Status` tinyint(1) NOT NULL,
  `IsDeleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `currencies`
--

LOCK TABLES `currencies` WRITE;
/*!40000 ALTER TABLE `currencies` DISABLE KEYS */;
INSERT INTO `currencies` VALUES (1,'United States','USD','$','2022-09-09 08:42:46.092857',NULL,'2022-09-12 03:18:27.867628',3,1,0),(2,'Thailand','THB','฿','2022-09-09 08:43:08.004281',NULL,'2022-09-09 08:44:56.822060',1,1,0),(3,'Vietnam','VND','₫','2022-09-09 08:43:36.787170',NULL,NULL,NULL,1,0),(4,'Singapore','SGD','S$','2022-09-09 08:44:15.738913',NULL,NULL,NULL,1,0),(5,'Malaysia','MYR','RM','2022-09-09 09:07:23.126118',NULL,NULL,NULL,1,0),(6,'Philippines','PHP','₱','2022-09-12 10:15:28.415062',NULL,NULL,NULL,1,0);
/*!40000 ALTER TABLE `currencies` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `departments`
--

DROP TABLE IF EXISTS `departments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `departments` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(150) NOT NULL,
  `CreationTime` datetime(6) NOT NULL,
  `CreatorUserId` int(11) DEFAULT NULL,
  `LastModificationTime` datetime(6) DEFAULT NULL,
  `LastModifierUserId` int(11) DEFAULT NULL,
  `Status` tinyint(1) NOT NULL,
  `IsDeleted` tinyint(1) NOT NULL,
  `WorkingHours` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `departments`
--

LOCK TABLES `departments` WRITE;
/*!40000 ALTER TABLE `departments` DISABLE KEYS */;
INSERT INTO `departments` VALUES (1,'Mock','2022-01-01 00:00:00.000000',NULL,NULL,NULL,0,1,0),(2,'Marketing','2022-08-22 11:12:30.113235',1,'2022-09-21 10:01:30.955200',32,0,0,9),(3,'Sales','2022-08-22 11:12:34.965836',1,'2022-09-08 03:16:56.984506',3,1,0,9),(4,'CS','2022-08-24 06:57:11.768796',1,'2022-09-14 03:38:05.866189',3,1,1,0),(5,'Design','2022-08-25 07:46:07.445578',1,NULL,NULL,1,0,0),(6,'HR','2022-08-26 08:13:47.564884',1,NULL,NULL,1,0,0),(7,'Logistics','2022-08-26 08:13:57.206608',1,NULL,NULL,1,0,0),(8,'OTP Team','2022-08-26 08:14:08.052993',1,NULL,NULL,1,0,0),(9,'-','2022-08-26 08:14:52.519736',1,'2022-09-12 10:33:08.629823',3,1,0,0),(10,'Risk','2022-08-29 06:55:15.065019',3,'2022-09-09 06:52:22.822006',1,1,0,0),(11,'Move','2022-09-12 04:06:56.960155',3,'2022-09-13 03:22:56.774888',3,1,1,0),(12,'Del','2022-09-14 03:24:56.280854',3,'2022-09-14 03:26:34.295403',3,1,1,0),(13,'CS','2022-09-14 03:39:41.963125',3,NULL,NULL,1,0,0),(14,'IT','2022-09-19 09:47:55.832839',3,'2022-09-21 10:02:06.628967',32,1,0,0),(15,'Transaction','2022-09-22 08:17:22.259032',3,NULL,NULL,1,0,0),(16,'Line Team','2022-09-22 08:17:32.052396',3,NULL,NULL,1,0,0),(17,'Finance','2022-09-22 08:17:47.286105',3,NULL,NULL,1,0,0);
/*!40000 ALTER TABLE `departments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `employeeimporthistories`
--

DROP TABLE IF EXISTS `employeeimporthistories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `employeeimporthistories` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `FileName` longtext NOT NULL,
  `ImportTime` datetime(6) NOT NULL,
  `TotalRows` int(11) NOT NULL,
  `TotalErrorRows` int(11) NOT NULL,
  `ImportByUserId` int(11) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `employeeimporthistories`
--

LOCK TABLES `employeeimporthistories` WRITE;
/*!40000 ALTER TABLE `employeeimporthistories` DISABLE KEYS */;
/*!40000 ALTER TABLE `employeeimporthistories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ranks`
--

DROP TABLE IF EXISTS `ranks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ranks` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(150) NOT NULL,
  `CreationTime` datetime(6) NOT NULL,
  `CreatorUserId` int(11) DEFAULT NULL,
  `LastModificationTime` datetime(6) DEFAULT NULL,
  `LastModifierUserId` int(11) DEFAULT NULL,
  `Status` tinyint(1) NOT NULL,
  `IsDeleted` tinyint(1) NOT NULL,
  `Level` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=34 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ranks`
--

LOCK TABLES `ranks` WRITE;
/*!40000 ALTER TABLE `ranks` DISABLE KEYS */;
INSERT INTO `ranks` VALUES (1,'Mock','2022-01-01 00:00:00.000000',NULL,NULL,NULL,0,1,0),(2,'Executive','2022-08-22 11:12:56.470980',1,NULL,NULL,1,0,0),(3,'Officer','2022-08-22 11:13:45.783973',1,NULL,NULL,1,0,0),(4,'Director','2022-08-22 11:13:52.304676',1,'2022-09-20 09:05:47.742405',1,1,0,100),(5,'-','2022-08-26 08:13:18.275007',1,'2022-09-12 10:17:55.091027',3,1,0,0),(6,'M1','2022-08-26 08:13:30.542857',1,'2022-09-21 08:00:22.862401',3,1,0,21),(7,'M3','2022-08-26 08:13:35.851094',1,'2022-09-21 08:00:29.865095',3,1,0,23),(8,'Probation','2022-08-29 06:54:23.825646',3,NULL,NULL,1,0,0),(9,'M10','2022-08-30 06:37:17.865076',3,'2022-09-09 06:50:19.764566',1,1,0,0),(10,'Training','2022-09-13 03:23:34.425160',3,'2022-09-13 03:24:32.557827',3,1,1,0),(11,'D1','2022-09-14 03:25:06.668953',3,'2022-09-14 03:26:56.341984',3,1,1,0),(12,'S1','2022-09-21 07:59:12.384939',3,'2022-09-21 07:59:36.147551',3,1,0,11),(13,'S2','2022-09-21 07:59:20.772946',3,'2022-09-21 07:59:45.446702',3,1,0,12),(14,'S3','2022-09-21 08:00:14.888112',3,NULL,NULL,1,0,13),(15,'M2','2022-09-22 07:02:53.190769',3,NULL,NULL,1,0,22),(16,'SM1','2022-09-22 07:03:17.944371',3,NULL,NULL,1,0,31),(17,'SM2','2022-09-22 07:03:24.313589',3,NULL,NULL,1,0,32),(18,'SSM1','2022-09-22 07:03:34.397975',3,NULL,NULL,1,0,41),(19,'SSM2','2022-09-22 07:03:41.900534',3,NULL,NULL,1,0,42),(20,'S4','2022-09-22 08:14:05.252697',3,NULL,NULL,1,0,14),(21,'S5','2022-09-22 08:14:15.361638',3,NULL,NULL,1,0,15),(22,'S6','2022-09-22 08:14:21.677076',3,NULL,NULL,1,0,16),(23,'S7','2022-09-22 08:14:31.847910',3,NULL,NULL,1,0,17),(24,'S8','2022-09-22 08:14:39.037214',3,NULL,NULL,1,0,18),(25,'S9','2022-09-22 08:14:46.220020',3,NULL,NULL,1,0,19),(26,'M4','2022-09-22 08:14:59.561829',3,NULL,NULL,1,0,24),(27,'M5','2022-09-22 08:15:12.486950',3,NULL,NULL,1,0,25),(28,'SM3','2022-09-22 08:15:35.841907',3,NULL,NULL,1,0,33),(29,'SM4','2022-09-22 08:15:47.125664',3,NULL,NULL,1,0,34),(30,'SM5','2022-09-22 08:15:56.820006',3,NULL,NULL,1,0,35),(31,'SSM3','2022-09-22 08:16:21.038785',3,NULL,NULL,1,0,43),(32,'SSM4','2022-09-22 08:16:43.535267',3,NULL,NULL,1,0,44),(33,'SSM5','2022-09-22 08:16:52.411668',3,NULL,NULL,1,0,45);
/*!40000 ALTER TABLE `ranks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `roleclaims`
--

DROP TABLE IF EXISTS `roleclaims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `roleclaims` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ClaimType` varchar(50) NOT NULL,
  `ClaimValue` varchar(100) NOT NULL,
  `RoleId` int(11) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_RoleClaims_RoleId` (`RoleId`),
  CONSTRAINT `FK_RoleClaims_Roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=387 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `roleclaims`
--

LOCK TABLES `roleclaims` WRITE;
/*!40000 ALTER TABLE `roleclaims` DISABLE KEYS */;
INSERT INTO `roleclaims` VALUES (133,'Permission','Employee.View',5),(134,'Permission','Employee.Create',5),(135,'Permission','Employee.Update',5),(136,'Permission','Employee.Delete',5),(137,'Permission','StaffRecord.View',5),(138,'Permission','StaffRecord.Create',5),(139,'Permission','StaffRecord.Update',5),(140,'Permission','StaffRecord.Delete',5),(141,'Permission','Department.View',5),(142,'Permission','Department.Create',5),(143,'Permission','Department.Update',5),(144,'Permission','Department.Delete',5),(145,'Permission','Rank.View',5),(146,'Permission','Rank.Create',5),(147,'Permission','Rank.Update',5),(148,'Permission','Rank.Delete',5),(149,'Permission','Brand.View',5),(150,'Permission','Brand.Create',5),(151,'Permission','Brand.Update',5),(152,'Permission','Brand.Delete',5),(153,'Permission','Bank.View',5),(154,'Permission','Bank.Create',5),(155,'Permission','Bank.Update',5),(156,'Permission','Bank.Delete',5),(157,'Permission','Currency.View',5),(158,'Permission','Currency.Create',5),(159,'Permission','Currency.Update',5),(160,'Permission','Currency.Delete',5),(259,'Permission','Bank.View',1),(260,'Permission','Bank.Create',1),(261,'Permission','Bank.Update',1),(262,'Permission','Bank.Delete',1),(263,'Permission','Brand.View',1),(264,'Permission','Brand.Create',1),(265,'Permission','Brand.Update',1),(266,'Permission','Brand.Delete',1),(267,'Permission','Rank.View',1),(268,'Permission','Rank.Create',1),(269,'Permission','Rank.Update',1),(270,'Permission','Rank.Delete',1),(271,'Permission','Department.View',1),(272,'Permission','Department.Create',1),(273,'Permission','Department.Update',1),(274,'Permission','Department.Delete',1),(275,'Permission','Employee.View',1),(276,'Permission','Employee.Create',1),(277,'Permission','Employee.Update',1),(278,'Permission','Employee.Delete',1),(279,'Permission','Role.View',1),(280,'Permission','Role.Create',1),(281,'Permission','Role.Update',1),(282,'Permission','Role.Delete',1),(283,'Permission','StaffRecord.View',1),(284,'Permission','StaffRecord.Create',1),(285,'Permission','StaffRecord.Update',1),(286,'Permission','StaffRecord.Delete',1),(287,'Permission','LeaveHistory.View',1),(288,'Permission','Currency.View',1),(289,'Permission','Currency.Create',1),(290,'Permission','Currency.Update',1),(291,'Permission','Currency.Delete',1),(358,'Permission','StaffRecord.View',8),(359,'Permission','StaffRecord.Create',8),(360,'Permission','StaffRecord.Update',8),(361,'Permission','StaffRecord.Delete',8),(362,'Permission','LeaveHistory.View',8),(369,'Permission','StaffRecord.View',7),(370,'Permission','StaffRecord.Create',7),(371,'Permission','LeaveHistory.View',7),(376,'Permission','Employee.View',6),(377,'Permission','Employee.Update',6),(378,'Permission','Employee.Create',6),(379,'Permission','Employee.Delete',6),(380,'Permission','LeaveHistory.View',6),(384,'Permission','StaffRecord.View',4),(385,'Permission','StaffRecord.Create',4),(386,'Permission','LeaveHistory.View',4);
/*!40000 ALTER TABLE `roleclaims` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `roledepartments`
--

DROP TABLE IF EXISTS `roledepartments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `roledepartments` (
  `RoleId` int(11) NOT NULL,
  `DepartmentId` int(11) NOT NULL,
  PRIMARY KEY (`RoleId`,`DepartmentId`),
  KEY `IX_RoleDepartments_DepartmentId` (`DepartmentId`),
  CONSTRAINT `FK_RoleDepartments_Departments_DepartmentId` FOREIGN KEY (`DepartmentId`) REFERENCES `departments` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_RoleDepartments_Roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `roledepartments`
--

LOCK TABLES `roledepartments` WRITE;
/*!40000 ALTER TABLE `roledepartments` DISABLE KEYS */;
INSERT INTO `roledepartments` VALUES (4,2),(6,2),(7,2),(4,3),(6,3),(7,3),(4,5),(6,5),(6,6),(6,7),(6,8),(6,9),(6,10),(6,13),(6,14),(6,15),(6,16),(6,17);
/*!40000 ALTER TABLE `roledepartments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `roles`
--

DROP TABLE IF EXISTS `roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `roles` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `CreationTime` datetime(6) NOT NULL,
  `CreatorUserId` int(11) DEFAULT NULL,
  `Name` varchar(256) NOT NULL,
  `NormalizedName` varchar(256) NOT NULL,
  `ConcurrencyStamp` varchar(50) NOT NULL,
  `LastModificationTime` datetime(6) DEFAULT NULL,
  `LastModifierUserId` int(11) DEFAULT NULL,
  `Status` tinyint(1) NOT NULL,
  `IsDeleted` tinyint(1) NOT NULL,
  `IsSuperAddmin` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `RoleNameIndex` (`NormalizedName`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `roles`
--

LOCK TABLES `roles` WRITE;
/*!40000 ALTER TABLE `roles` DISABLE KEYS */;
INSERT INTO `roles` VALUES (1,'2022-01-01 00:00:00.000000',1,'SuperAdmin','SUPERADMIN','a15e8c7f-d70d-4245-a4b9-4055fffd55bd','2022-09-14 11:08:27.181960',3,1,0,1),(2,'2022-08-22 11:07:54.868455',1,'CEO','CEO','88036983-e0ba-4a9a-94b4-d58fa121a178','2022-09-23 05:32:19.833382',3,1,0,0),(3,'2022-08-22 11:14:21.270081',1,'Staff','STAFF','32328fca-64df-4f5b-b927-899424de6710','2022-09-23 05:31:27.126792',3,1,0,0),(4,'2022-08-22 13:55:53.976194',1,'Manager','MANAGER','7c94f499-5d07-45ed-a968-979c9bf7bed4','2022-09-28 09:18:55.777804',1,1,0,0),(5,'2022-09-14 09:21:51.240355',3,'HR Staff','HR STAFF','00a087c0-410f-4ab1-84d5-f8ba9bf6c13c',NULL,NULL,1,0,0),(6,'2022-09-14 09:37:04.625244',3,'HR Manager','HR MANAGER','4854c068-4baf-4d80-8862-3e53dfbafbdf','2022-09-27 04:16:42.658148',3,1,0,0),(7,'2022-09-22 08:18:43.910387',3,'Senior Manager','SENIOR MANAGER','d07ea233-9002-4372-8ac2-1e7658cf0302','2022-09-26 08:26:11.328679',1,1,0,0),(8,'2022-09-22 08:21:54.135405',3,'Super Senior Manager','SUPER SENIOR MANAGER','e0390e0f-1079-40d2-9175-09c0a73e6352',NULL,NULL,1,0,0);
/*!40000 ALTER TABLE `roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `staffrecorddocuments`
--

DROP TABLE IF EXISTS `staffrecorddocuments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `staffrecorddocuments` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `StaffRecordId` int(11) NOT NULL,
  `FileUrl` longtext NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_StaffRecordDocuments_StaffRecordId` (`StaffRecordId`),
  CONSTRAINT `FK_StaffRecordDocuments_StaffRecords_StaffRecordId` FOREIGN KEY (`StaffRecordId`) REFERENCES `staffrecords` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `staffrecorddocuments`
--

LOCK TABLES `staffrecorddocuments` WRITE;
/*!40000 ALTER TABLE `staffrecorddocuments` DISABLE KEYS */;
INSERT INTO `staffrecorddocuments` VALUES (17,5,'import Employee Sample - v2.xlsx'),(18,5,'import Employee Sample.xlsx'),(19,20,'image.png');
/*!40000 ALTER TABLE `staffrecorddocuments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `staffrecords`
--

DROP TABLE IF EXISTS `staffrecords`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `staffrecords` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `EmployeeId` int(11) NOT NULL,
  `DepartmentId` int(11) NOT NULL,
  `RecordType` int(11) NOT NULL,
  `Reason` varchar(500) NOT NULL,
  `StartDate` datetime(6) NOT NULL,
  `EndDate` datetime(6) NOT NULL,
  `Remarks` varchar(200) DEFAULT NULL,
  `CreationTime` datetime(6) NOT NULL,
  `CreatorUserId` int(11) DEFAULT NULL,
  `LastModificationTime` datetime(6) DEFAULT NULL,
  `LastModifierUserId` int(11) DEFAULT NULL,
  `Status` tinyint(1) NOT NULL,
  `IsDeleted` tinyint(1) NOT NULL,
  `OtherDepartment` varchar(200) DEFAULT NULL,
  `NumberOfDays` int(11) NOT NULL DEFAULT '0',
  `NumberOfHours` int(11) NOT NULL DEFAULT '0',
  `LateAmount` int(11) NOT NULL DEFAULT '0',
  `RecordDetailType` int(11) NOT NULL DEFAULT '0',
  `Fine` decimal(18,2) NOT NULL DEFAULT '0.00',
  `CalculationAmount` decimal(18,2) NOT NULL DEFAULT '0.00',
  PRIMARY KEY (`Id`),
  KEY `IX_StaffRecords_EmployeeId` (`EmployeeId`),
  CONSTRAINT `FK_StaffRecords_Users_EmployeeId` FOREIGN KEY (`EmployeeId`) REFERENCES `users` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `staffrecords`
--

LOCK TABLES `staffrecords` WRITE;
/*!40000 ALTER TABLE `staffrecords` DISABLE KEYS */;
INSERT INTO `staffrecords` VALUES (1,5,3,3,'day off','2022-08-04 00:00:00.000000','2022-08-06 00:00:00.000000','Remarks....','2022-08-23 09:38:43.123245',1,'2022-08-23 13:28:26.904462',1,1,0,NULL,0,0,0,0,0.00,0.00),(2,3,3,2,'paid off test reason','2022-08-07 00:00:00.000000','2022-08-12 00:00:00.000000','....','2022-08-23 16:51:25.644803',1,'2022-08-24 11:21:26.904425',1,1,0,NULL,0,0,0,0,0.00,0.00),(3,4,2,3,'reason test','2022-08-25 00:00:00.000000','2022-08-27 00:00:00.000000','...','2022-08-24 09:12:34.376823',1,'2022-08-24 11:21:19.268903',1,1,0,NULL,0,0,0,0,0.00,0.00),(4,9,7,0,'r test','2022-08-08 01:01:01.000000','2022-08-12 01:01:01.000000','..','2022-08-24 09:14:55.849629',1,'2022-09-20 09:56:15.908337',1,1,0,'IT dept',0,96,0,0,0.00,0.00),(5,5,3,1,'test','2022-08-07 00:00:00.000000','2022-08-06 00:00:00.000000','.....','2022-08-24 11:18:24.614594',1,'2022-08-24 06:22:40.660562',1,1,0,NULL,0,0,0,0,0.00,0.00),(6,3,4,0,'testt','2022-08-02 00:00:00.000000','2022-08-04 00:00:00.000000','testing','2022-08-24 09:04:46.062249',1,NULL,NULL,1,0,NULL,0,0,0,0,0.00,0.00),(7,4,2,2,'test2','2022-08-01 00:00:00.000000','2022-08-03 00:00:00.000000','test2','2022-08-24 10:34:51.849917',1,NULL,NULL,1,0,NULL,0,0,0,0,0.00,0.00),(8,3,7,0,'','2022-08-01 13:01:01.000000','2022-08-01 15:01:01.000000',NULL,'2022-08-29 06:28:18.792245',3,NULL,NULL,1,0,NULL,0,2,0,0,0.00,0.00),(9,8,6,1,'late for 2 hours','2022-08-02 01:01:01.000000','2022-08-02 11:01:01.000000',NULL,'2022-08-29 06:31:20.791163',3,NULL,NULL,1,0,NULL,0,10,300,4,0.00,0.00),(10,3,2,2,'holiday','2022-08-03 01:01:01.000000','2022-08-05 01:01:01.000000',NULL,'2022-08-29 06:47:28.353282',3,NULL,NULL,1,0,NULL,3,0,0,16,0.00,0.00),(11,3,3,1,'late 2h','2022-09-09 09:01:01.000000','2022-09-09 11:01:01.000000',NULL,'2022-09-09 04:08:19.316538',3,NULL,NULL,1,0,NULL,0,2,2500,4,0.00,0.00),(12,3,2,2,'2 days leave','2022-09-19 01:01:01.000000','2022-09-20 01:01:01.000000',NULL,'2022-09-12 04:53:13.250693',3,'2022-09-12 04:54:23.893935',3,1,0,NULL,2,0,0,16,0.00,0.00),(13,8,2,3,'sick','2022-09-05 01:01:01.000000','2022-09-06 01:01:01.000000',NULL,'2022-09-12 10:11:53.004382',3,NULL,NULL,1,0,NULL,2,0,0,32,0.00,0.00),(14,30,10,3,'sick','2022-09-13 01:01:01.000000','2022-09-14 01:01:01.000000',NULL,'2022-09-14 08:09:55.154451',3,NULL,NULL,1,0,NULL,2,0,0,32,0.00,0.00),(15,7,2,0,'ot','2022-09-20 16:28:01.000000','2022-09-20 19:28:01.000000',NULL,'2022-09-14 08:27:37.722852',3,NULL,NULL,1,0,NULL,0,3,0,0,0.00,63.00),(16,6,2,2,'gg','2022-08-29 01:01:01.000000','2022-09-02 01:01:01.000000',NULL,'2022-09-21 08:50:07.602789',3,NULL,NULL,1,0,NULL,5,0,0,16,0.00,0.00),(17,4,2,3,'sick','2022-09-22 01:01:01.000000','2022-09-22 01:01:01.000000',NULL,'2022-09-22 07:16:30.244622',3,NULL,NULL,1,0,NULL,1,0,0,32,0.00,0.00),(18,27,2,2,'leave','2022-09-21 01:01:01.000000','2022-09-21 01:01:01.000000',NULL,'2022-09-22 07:16:48.741107',3,NULL,NULL,1,0,NULL,1,0,0,16,0.00,0.00),(19,28,2,0,'1hour','2022-09-20 11:01:01.000000','2022-09-20 12:01:01.000000',NULL,'2022-09-22 07:17:24.106843',3,NULL,NULL,1,0,NULL,0,1,0,0,0.00,9.00),(20,33,3,1,'late 30 mins','2022-09-22 01:01:01.000000','2022-09-22 01:01:01.000000',NULL,'2022-09-22 07:18:04.852571',3,'2022-09-22 07:23:53.308120',3,1,0,NULL,0,0,300,4,0.00,0.00),(21,16,6,0,'....','2022-09-29 01:01:01.000000','2022-09-29 21:01:01.000000',NULL,'2022-09-28 09:10:01.025290',1,NULL,NULL,1,0,NULL,0,20,0,0,0.00,0.00);
/*!40000 ALTER TABLE `staffrecords` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `userroles`
--

DROP TABLE IF EXISTS `userroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `userroles` (
  `UserId` int(11) NOT NULL,
  `RoleId` int(11) NOT NULL,
  PRIMARY KEY (`UserId`,`RoleId`),
  KEY `IX_UserRoles_RoleId` (`RoleId`),
  CONSTRAINT `FK_UserRoles_Roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_UserRoles_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `userroles`
--

LOCK TABLES `userroles` WRITE;
/*!40000 ALTER TABLE `userroles` DISABLE KEYS */;
INSERT INTO `userroles` VALUES (1,1),(3,1),(32,1),(6,2),(19,2),(31,2),(4,3),(5,3),(9,3),(10,3),(11,3),(12,3),(13,3),(14,3),(16,3),(23,3),(25,3),(27,3),(29,3),(7,4),(8,4),(15,4),(17,4),(18,4),(24,4),(26,4),(30,4),(34,4),(28,5),(33,6),(35,7);
/*!40000 ALTER TABLE `userroles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `users` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(150) NOT NULL,
  `UserType` int(11) NOT NULL,
  `IsDeleted` tinyint(1) NOT NULL,
  `IsFirstTimeLogin` tinyint(1) NOT NULL,
  `CreationTime` datetime(6) NOT NULL,
  `CreatorUserId` int(11) DEFAULT NULL,
  `LastModificationTime` datetime(6) DEFAULT NULL,
  `LastModifierUserId` int(11) DEFAULT NULL,
  `Status` tinyint(1) NOT NULL,
  `EmployeeCode` varchar(50) NOT NULL,
  `RoleId` int(11) NOT NULL,
  `RankId` int(11) NOT NULL,
  `DeptId` int(11) NOT NULL,
  `BankId` int(11) NOT NULL,
  `BankAccountNumber` varchar(20) DEFAULT NULL,
  `StartDate` datetime(6) DEFAULT NULL,
  `BirthDate` datetime(6) DEFAULT NULL,
  `IdNumber` varchar(20) DEFAULT NULL,
  `BackendUser` varchar(20) DEFAULT NULL,
  `BackendPass` varchar(20) DEFAULT NULL,
  `Salary` int(11) NOT NULL,
  `Note` varchar(150) DEFAULT NULL,
  `IntranetPassword` varchar(20) NOT NULL,
  `Country` varchar(80) DEFAULT NULL,
  `UserName` varchar(256) DEFAULT NULL,
  `NormalizedUserName` varchar(256) DEFAULT NULL,
  `Email` varchar(256) DEFAULT NULL,
  `NormalizedEmail` varchar(256) DEFAULT NULL,
  `EmailConfirmed` tinyint(1) NOT NULL,
  `PasswordHash` longtext,
  `SecurityStamp` longtext,
  `ConcurrencyStamp` longtext,
  `PhoneNumber` longtext,
  `PhoneNumberConfirmed` tinyint(1) NOT NULL,
  `TwoFactorEnabled` tinyint(1) NOT NULL,
  `LockoutEnd` datetime(6) DEFAULT NULL,
  `LockoutEnabled` tinyint(1) NOT NULL,
  `AccessFailedCount` int(11) NOT NULL,
  `BankAccountName` varchar(150) NOT NULL DEFAULT '',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Users_EmployeeCode` (`EmployeeCode`),
  UNIQUE KEY `UserNameIndex` (`NormalizedUserName`),
  KEY `EmailIndex` (`NormalizedEmail`)
) ENGINE=InnoDB AUTO_INCREMENT=36 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'hoan le',10,0,0,'2022-08-22 10:35:52.287121',NULL,NULL,NULL,1,'lenguyenhanhoan',1,1,1,1,NULL,NULL,'2022-08-22 10:35:52.287123',NULL,NULL,NULL,0,NULL,'123qwe!@#QWE',NULL,'lenguyenhanhoan','LENGUYENHANHOAN','lenguyenhanhoan@gmail.com','LENGUYENHANHOAN@GMAIL.COM',0,'AQAAAAEAACcQAAAAEICMuS2G+w+XnHA3fwvQebps9rEkVXwckhHp/TjVscXd5HBtn/1S+flvl+s7rLhWSQ==','QAO37NDN6CG3RR7E4WFEOYYB2RTHH5ZS','9be4bfe0-ca80-4bbd-ba72-efc53da419e4',NULL,0,0,NULL,1,0,''),(3,'Glory',0,0,0,'2022-08-22 13:16:46.286828',1,'2022-09-14 07:54:35.824141',1,1,'Glory080',1,2,3,2,'321312323','2022-08-08 00:00:00.000000','2022-08-01 00:00:00.000000','2323123131',NULL,NULL,232000,NULL,'Glory123!','Singapore','Glory080','GLORY080',NULL,NULL,0,'AQAAAAEAACcQAAAAEAeCN10vTVEkrfTadN4d1GjJASXZKzkzZXLujNAo/iR3A6H9Uuk7qtKjct7x/Mmgug==','C4LGLVN4JWR3EQXFRAHNWZOVJWP5AKLJ','517cc2ef-437b-413e-b33c-3bdfe2453752',NULL,0,0,NULL,1,0,''),(4,'HoanLe',0,0,1,'2022-08-22 13:48:14.142796',1,'2022-09-22 07:10:17.842302',3,1,'HoanLe081',3,12,2,3,'127777777','2022-08-10 00:00:00.000000','2022-08-02 00:00:00.000000','2112122122',NULL,NULL,20000000,NULL,'123qwe!@#QWE','Vietnam','HoanLe081','HOANLE081',NULL,NULL,0,'AQAAAAEAACcQAAAAEFFUkv6pqcZOI2+mzdsYSdUUUxRb2IdztxzzOKE6gSR1NnpZN9fZlOXDyQcUyqnogA==','IXT6UUXLYOQQAUUDX65U2LV2GZRYUJ7C','ba8e1656-d780-4915-9e6e-1c064498a759',NULL,0,0,NULL,1,0,''),(5,'Hoan Le V5',0,0,1,'2022-08-22 13:54:01.667898',1,'2022-09-12 03:18:13.677246',3,1,'HoanLeCode05',3,3,2,3,'11121212121221','2022-08-10 00:00:00.000000','2001-06-02 00:00:00.000000','33333111',NULL,NULL,3000000,NULL,'123qwe!@#QWE','United States','HoanLeCode05','HOANLECODE05',NULL,NULL,0,'AQAAAAEAACcQAAAAEGZWZ2VQTRUs8J6rKeBjiz1bftHhtCx0Nkac3yYHESc67FJg3k5dM0EXaenx72pBEA==','PNK62HEZF3QREJLZXIFCH6AKJZ6OGCET','c3cad9df-b755-4227-af2a-8136062ebdea',NULL,0,0,NULL,1,0,''),(6,'HoanLe Test07',0,0,1,'2022-08-29 02:40:48.877970',1,'2022-09-22 07:11:34.298735',3,1,'HoanLeTest07',2,6,2,3,'123456678','2021-11-10 00:00:00.000000','1990-05-24 00:00:00.000000','33145622200','HoanLeV6','123qwe!@#QWE',70000,'employee note….','123qwe!@#QWE','Singapore','HoanLeTest07','HOANLETEST07','HoanLeTest07@intranet.com','HOANLETEST07@INTRANET.COM',0,'AQAAAAEAACcQAAAAEA2WNJeETgEII0qSI8BJ91euzY1hd9tMQcnJfnz1qI2nXWTBSlcke4Jd9VRSjX9MEg==','3CINMBVM7DP5OEL7QDEPFLGE7DCG2GOV','c530b102-c611-48d2-9317-ee5ea6b83de0',NULL,0,0,NULL,1,0,''),(7,'HoanLe Test008',0,0,0,'2022-08-29 02:40:48.877970',1,'2022-09-28 09:17:25.732043',1,1,'HoanLeTest08',4,12,3,9,'123456678','2021-11-10 01:01:01.000000','1990-05-24 01:01:01.000000','33145622200','HoanLeV6','123qwe!@#QWE',70000,'','123qwe!@#QWE1','VietNam','HoanLeTest08','HOANLETEST08','HoanLeTest08@intranet.com','HOANLETEST08@INTRANET.COM',0,'AQAAAAEAACcQAAAAEIpsz3EK25SsNJJ0bpR1A3E+48o8DnfvZhH61cYHGdt9MD8hi32PBysWB4nRRt2/OQ==','JF3K6JHILJGMELBRRGQ7FX6DUOF6RGB6','a4e56c51-66f2-46cd-91ed-041c0eefb98c',NULL,0,0,NULL,1,0,''),(8,'HoanLe Test009',0,0,1,'2022-08-29 03:10:50.792720',1,'2022-09-14 03:19:53.430626',1,1,'HoanLeTest09',4,3,3,3,'123456678','2021-11-10 00:00:00.000000','1990-05-24 00:00:00.000000','33145622200','HoanLeV06','123qwe!@#QWE',70000,'employee note….','123qwe!@#QWE','Singapore','HoanLeTest09','HOANLETEST09','HoanLeTest09@intranet.com','HOANLETEST09@INTRANET.COM',0,'AQAAAAEAACcQAAAAENf6usIkv50As1fdfLaI64JZAHGXQTRwU07h6Ubwdyzf0dawfI6Inbl2g1guDziE4Q==','GU57I2TYFNLQJQKYQPXGSYEQA5LCREHS','6ab11d56-94f0-4a3e-99b0-009920265fe8',NULL,0,0,NULL,1,0,''),(9,'P.Ann',0,0,1,'2022-08-30 03:33:54.005709',1,'2022-09-21 08:31:09.826291',32,1,'P.Ann1084',3,5,7,4,'3832420505','2022-04-16 00:00:00.000000','1977-03-02 00:00:00.000000','','','',20000,'employee note….','123qwe!@#QWE','Thailand','P.Ann1084','P.ANN1084','P.Ann1084@intranet.com','P.ANN1084@INTRANET.COM',0,'AQAAAAEAACcQAAAAEDCOxJG9tRnj7oMxIu6o6Xcf7Clau6E6cGgYWg3+hiIswLPmjDrA+EU2SBVwTfHcWg==','MS7DW3GEKCFP4LYJT5BVDURW55UXW2CK','b9af3ca0-5398-4240-a072-e419f333c4e6',NULL,0,0,NULL,1,0,'Sarunya Thomasorn'),(10,'Birth',0,0,1,'2022-08-30 05:28:42.180781',3,NULL,NULL,0,'Birth1074',3,8,4,6,'4772334035','2022-04-20 00:00:00.000000','1989-05-28 00:00:00.000000','','cs24','wowcs24',15000,'Deactivated','123qwe!@#QWE','Thailand','Birth1074','BIRTH1074','Birth1074@intranet.com','BIRTH1074@INTRANET.COM',0,'AQAAAAEAACcQAAAAELIVRw7uA4Q7TDD5h0h+PUlictGn3qGq/pZnG+o9j0dWMNbX3kfTYyaDJyiL7P+FSw==','RPC5Y6GXBA4MN2AU5JWEQQCZ4IOCHAG4','8f9b0067-24aa-46f8-812c-27ba8ccebbb0',NULL,0,0,NULL,1,0,''),(11,'GTest',0,1,1,'2022-08-30 06:02:27.844480',3,'2022-08-30 06:14:17.850589',3,1,'GTest',3,5,7,4,'123-4-56789-0','2022-08-01 00:00:00.000000','1999-08-01 00:00:00.000000','Taken',NULL,NULL,50000,NULL,'123qwe!@#QWE','Thailand','GTest','GTEST',NULL,NULL,0,'AQAAAAEAACcQAAAAEGD1mlHrUpGvaPz6wiXo3b+pskhtV0jFfDXWmIDawFgHaJmMI0+SDsMhy1RlLQijng==','MDFRQAIUNZVSMHTSFQIPBZRD6PWWNCWH','933cddae-8a6b-4614-9fae-5d73a9647fdd',NULL,0,0,NULL,1,0,''),(12,'Richard',0,0,1,'2022-08-30 06:03:27.651632',3,'2022-09-12 03:54:03.604535',3,1,'Richard1107',3,5,7,5,'423-0-83427-9','2022-05-01 00:00:00.000000','1901-01-01 00:00:00.000000','','','',75000,'','123qwe!@#QWE','Thailand','Richard1107','RICHARD1107','Richard1107@intranet.com','RICHARD1107@INTRANET.COM',0,'AQAAAAEAACcQAAAAEASvbdG0Lj+wV0CKYonrR6V3ZedoczvrPHEMxvuWkYrsGTxRRg/e49YRE4fUfZpqsg==','MMCFGSRSLABIW57HBJ6T53IWJU4JIUV7','d304714e-f7c3-41a7-acd8-a3b8cac48ccb',NULL,0,0,NULL,1,0,'Mr Lee Eng Ann'),(13,'Long',0,0,1,'2022-08-30 06:04:36.814245',3,'2022-09-12 03:54:13.258266',3,1,'Long1117',3,5,8,4,'457-2-53264-5','2022-06-01 00:00:00.000000','1901-01-01 00:00:00.000000','','','',25000,'','123qwe!@#QWE','Thailand','Long1117','LONG1117','Long1117@intranet.com','LONG1117@INTRANET.COM',0,'AQAAAAEAACcQAAAAEPgBWX4+q/iG92q6CTY/fh43FYFzLD2tUGvX2pTprrU3WPT0Hacx6BZwWKPr7dOfGQ==','IXPWEH5SYUATIMK443ZC3LM3WZF6IJBP','596cb924-27fc-49c5-b034-3d9ed37ba902',NULL,0,0,NULL,1,0,'Test 1'),(14,'Apple',0,0,1,'2022-08-30 06:04:36.814245',3,'2022-09-14 03:40:52.993422',3,1,'Applee1007',3,6,13,4,'0331344001','2022-08-15 00:00:00.000000','1992-01-14 00:00:00.000000','','t99st7','1111ccdd',25000,'','123qwe!@#QWE','Thailand','Applee1007','APPLEE1007','Applee1007@intranet.com','APPLEE1007@INTRANET.COM',0,'AQAAAAEAACcQAAAAECdI+Dnwm8/JSaN9e1u9LeyITLy4mgLq0MTo6s3RSspDY35ksJhkurEU12Lx1XjDrg==','H4JO4EVMPKC424KNE2K2KMJHTQ7IFIOD','d9aa3f4b-d287-464d-a7eb-0393548d298e',NULL,0,0,NULL,1,0,''),(15,'Feel',0,0,1,'2022-08-30 06:10:44.162843',3,'2022-09-27 07:38:00.049850',3,1,'Feel1020',4,6,4,4,'0348115758',NULL,'1996-01-16 01:01:01.000000','ID Taken','t99st12/cs8','3232eeff/wowcs8',24500,'','123qwe!@#QWE','Thailand','Feel1020','FEEL1020','Feel1020@intranet.com','FEEL1020@INTRANET.COM',0,'AQAAAAEAACcQAAAAEIgSghYYBnMsiVIsvNmvXszpsIlA8iNZeY8IltfZjxMgmMlfFDTMPR9J92Zx5Kf+cQ==','QTOK7EYX3HVKQVAV677UIPA76FAAQ3FS','05ca6550-f048-4b54-a9a8-c9b5a7392fa2',NULL,0,0,NULL,1,0,'FEEL TEST'),(16,'Top',0,0,1,'2022-09-09 10:25:10.333261',3,'2022-09-21 08:32:28.396964',3,1,'Top1014',3,6,6,4,'0671610342','2021-02-01 00:00:00.000000','2002-02-18 00:00:00.000000','123123951','','',25500,'','123qwe!@#QWE','Thailand','Top1014','TOP1014','Top1014@intranet.com','TOP1014@INTRANET.COM',0,'AQAAAAEAACcQAAAAENOW3hFa7e7NXPmJB5U+yldTUHetz7zxim6/+orfLdRflE2oEayczxeQyVCsON+dHQ==','RDMCBX4XAMVXGXBEHMUGLQTE2CQMCVPV','808cac47-5a13-4117-8de9-03222d260284',NULL,0,0,NULL,1,0,'Rattanaphat Jongton'),(17,'San',0,0,1,'2022-09-09 10:25:10.333261',3,'2022-09-27 04:32:20.689937',3,1,'Sann1019',4,6,4,7,'8522140627','2021-05-26 01:01:01.000000','1996-08-15 01:01:01.000000','123123952','','',24500,'','123qwe!@#QWE','Thailand','Sann1019','SANN1019','Sann1019@intranet.com','SANN1019@INTRANET.COM',0,'AQAAAAEAACcQAAAAEKOljf0cgX4RyVagkwLymbpHVROIJG9kUVHtn8i8fmlSZyFmq0pGqT5MzrvjQE3LOw==','FFXR7AAM6IEKTA5JGTB7DREIDELX7PHA','438d8a2f-f09d-4f44-b9ee-e4d571dc082c',NULL,0,0,NULL,1,0,'Subpachai Chaikaewmay'),(18,'Kate',0,0,1,'2022-09-12 03:39:26.168890',3,NULL,NULL,1,'Katee1027',4,6,6,4,'1093555372','2021-10-01 00:00:00.000000','1982-11-12 00:00:00.000000',NULL,NULL,NULL,29500,NULL,'123qwe!@#QWE','Thailand','Katee1027','KATEE1027',NULL,NULL,0,'AQAAAAEAACcQAAAAEKB0unCR1Gcf06sKEEdMqRpk30Ip8Vgah9JSjY9i9z+v/z+mnu0ki85Qx4HxUUP0Ag==','P7VN2LEYFBTZITSVYM2LO4T4S6PO53N2','92dc0f62-030c-4ed1-9b96-c1bdc0e87e2e',NULL,0,0,NULL,1,0,'Virunpatch Srikham'),(19,'Mai',0,0,1,'2022-09-12 03:46:37.338204',3,'2022-09-27 07:36:02.371873',3,1,'Mai1058',2,6,9,7,'4311152285',NULL,NULL,NULL,NULL,NULL,12000,NULL,'123qwe!@#QWE','Thailand','Mai1058','MAI1058',NULL,NULL,0,'AQAAAAEAACcQAAAAEN6xFaq2PsDFLL24cI164VG+NOMUIODlNwDjCth48cuwf5VFHmPmcId7liW9VxsbWg==','LQPBRUV7UGWAA26MAYUTHU27UPIO4AR4','feb1fe5c-faab-4333-8496-95d3309dae9b',NULL,0,0,NULL,1,0,'Myo Moon Mai'),(23,'Snow',0,0,1,'2022-09-12 03:52:24.016772',3,'2022-09-27 06:22:19.055485',1,1,'Snow1068',3,8,4,4,'0218379508','2022-04-13 01:01:01.000000','1996-11-20 01:01:01.000000','','cs22','wowcs22',15000,'','123qwe!@#QWE','Thailand','Snow1068','SNOW1068','Snow1068@intranet.com','SNOW1068@INTRANET.COM',0,'AQAAAAEAACcQAAAAEB4fURGuGJ2YlQDlV5hGckEBROPxtFH5nEhz32noHUtcRz757HFd7MBxgX7LbYm1CQ==','QF7QVETL6T5KCWXRDSGDTH6EZILJD5K7','ecbf4bcc-2e6b-4bba-80f5-efaa60857b99',NULL,0,0,NULL,1,0,'Montharika Jandam'),(24,'Eddie',0,0,1,'2022-09-12 03:55:38.596666',3,NULL,NULL,1,'Eddie1029',4,5,2,3,'','2021-11-01 00:00:00.000000',NULL,NULL,NULL,NULL,50000,NULL,'123qwe!@#QWE','Thailand','Eddie1029','EDDIE1029',NULL,NULL,0,'AQAAAAEAACcQAAAAEDsrzyTNC79NeHK2OjVk6e3W5Z+hAzCIq7RrRjSLKjJM/eeOrZiv0Xl9ED5RtdmFVg==','JWPQ4GQR6DSAPND2WT3AMR7O345T4GSU','014e6f81-ded7-4f46-ad06-c7f14b99afe0',NULL,0,0,NULL,1,0,'Trf to J'),(25,'K-lin',0,0,1,'2022-09-12 10:34:43.746745',3,NULL,NULL,1,'K-lin1028',3,5,2,7,'816-267148-6','2021-11-01 00:00:00.000000',NULL,NULL,NULL,NULL,50000,NULL,'123qwe!@#QWE','Thailand','K-lin1028','K-LIN1028',NULL,NULL,0,'AQAAAAEAACcQAAAAEAAUpblr0+gLrOJb0Y83HfibRkVWb8amdzTz7isXKSDC3O1Kzaziwky+Mp7aLxQvSg==','VRBVPDH4VDNY6WCPLYDUCCYKDBY4Z7NT','b847bd2f-c5e4-4145-910b-d27420d33e21',NULL,0,0,NULL,1,0,'Lim Kay-Lin Kathlene'),(26,'Goy',0,0,1,'2022-09-12 10:41:14.055369',3,'2022-09-27 06:19:04.516614',1,1,'Goy1010',4,7,10,4,'0872102604','2022-09-05 01:01:01.000000','1996-08-10 01:01:01.000000','','fin3','wowfin3',32500,'123qwe!@#QWE','123qwe!@#QWE','Thailand','Goy1010','GOY1010','Goy1010@intranet.com','GOY1010@INTRANET.COM',0,'AQAAAAEAACcQAAAAEIDR/BqoyxcMG2mm3Uaq6g/m+ZC4CqD+pPZFeQR5lof94qoDCJyayWc62TdAiJBexA==','QABIGSNLBSV5CTZTHZYLRM7SHFGOK26D','e8b76845-9e7f-4016-a15d-a95ce295d859',NULL,0,0,NULL,1,0,'Jansri Panapipatsook'),(27,'Del',0,0,1,'2022-09-14 03:26:21.485436',3,'2022-09-22 07:13:30.851749',3,1,'Del',3,15,2,9,'11223330008','2022-09-14 00:00:00.000000','1990-09-01 00:00:00.000000',NULL,NULL,NULL,20000,NULL,'123qwe!@#QWE','Thailand','Del','DEL',NULL,NULL,0,'AQAAAAEAACcQAAAAEJ2wD5FZeUI1JVZuteE9LIJ/xtnrn/NBJmoOCFXFysNwSAIiYORb0Jgfp8jaBprWIw==','5K3BWRPJYE2XLD5X4QIWDAIM2CIRMODC','854d2ff2-0ca5-4cc6-b06e-9e7ba2fc9ac1',NULL,0,0,NULL,1,0,'Test'),(28,'test2',0,0,0,'2022-09-14 03:37:31.957587',3,'2022-09-22 07:13:00.404671',3,1,'test2',5,6,2,3,'1231233775','2022-09-07 00:00:00.000000','2015-09-08 00:00:00.000000','23412332',NULL,NULL,30000,NULL,'QWE123qwe!@#','thailand','test2','TEST2',NULL,NULL,0,'AQAAAAEAACcQAAAAEEFWBmLOL2yvW/aaOS4JcSWxGRZqHu7WAVVQWJlg1F++XOO+nKc669EUmU6KFLDyKQ==','LHT6JCMBW5LZT6FXG22IRSAJKK6IHAIE','977f3b4b-f59d-4e0e-b6b4-9a3942e09cbd',NULL,0,0,NULL,1,0,'test 23'),(29,'Richard Triumph',0,0,1,'2022-09-14 04:25:51.821911',1,'2022-09-22 03:56:18.170543',1,1,'RichardTriumph1108',3,9,3,2,'423-0-83427-9','2022-05-01 00:00:00.000000','1999-04-24 00:00:00.000000','123123951','','',75000,'','123qwe!@#QWE','Thailand','RichardTriumph1108','RICHARDTRIUMPH1108','RichardTriumph1108@intranet.com','RICHARDTRIUMPH1108@INTRANET.COM',0,'AQAAAAEAACcQAAAAEMkZx/UF39EByspd29Lsaqdt7LWFJpgPVZl18X44T7wibMGMU29eEFdS6zIhd7/+Sw==','6P2Q7JAXFKSR3OGKKZJ2B4CEIJPKPUX5','482c5a28-d9e6-49fc-9fd4-736a02b7d6a6',NULL,0,0,NULL,1,0,'Richard Triumph'),(30,'Fern',0,0,1,'2022-09-14 06:16:10.312447',1,'2022-09-22 03:50:17.029232',3,1,'Fern1015',4,7,10,7,'4081082231','2020-01-01 00:00:00.000000','1999-04-24 00:00:00.000000','','fin2','wowfin2',26500,'123qwe!@#QWE','123qwe!@#QWE','Thailand','Fern1015','FERN1015','Fern1015@intranet.com','FERN1015@INTRANET.COM',0,'AQAAAAEAACcQAAAAEKS6HtDY+fgBmERConfDWOSDrPy96lMMw+OEOc3G4EB+oigTvMTpRezroEYUkT3HlQ==','ITS2AMGQ6XIK4U76RKNFWKZDUCYOWM3P','67daeef8-a75a-4265-a663-dc897aa9fe25',NULL,0,0,NULL,1,0,'Preyapron Keawwijit'),(31,'Alice',0,0,1,'2022-09-14 06:52:38.012290',3,'2022-09-22 04:18:26.375608',3,1,'Alice1043',2,6,2,6,'3392710335','2022-01-04 00:00:00.000000','1991-09-02 00:00:00.000000','ID Taken','','',58000,'','123qwe!@#QWE','Thailand','Alice1043','ALICE1043','Alice1043@intranet.com','ALICE1043@INTRANET.COM',0,'AQAAAAEAACcQAAAAEFmqonsYmLUX5JSN8fUch10BcnhhevrIc7fSU1JQtiHAic+Obnp3Hqg81forJSTmIw==','BQXYYM7TLQCXCJQMXETYGE6A4AMLE5P3','78c357c0-04c8-4531-b6d9-7c420914bafc',NULL,0,0,NULL,1,0,'Kanyapat Buakhiow'),(32,'Thomas',0,0,0,'2022-09-19 09:44:15.173160',3,'2022-09-23 07:58:34.564003',3,1,'tl4896',1,5,3,9,NULL,'2022-10-02 01:01:01.000000','2022-10-02 01:01:01.000000',NULL,NULL,NULL,50000,NULL,'Abcd1234.','Malaysia','tl4896','TL4896',NULL,NULL,0,'AQAAAAEAACcQAAAAEFPbINb+Pe+3TBvJJaRhqEJ3y8h/MxjzN+inu20ofHQkHARTxFwKWdq/gmxx7mlibQ==','AXRIX2XLU5ZHP6UPH6AKFYIDTLDMIRTL','a840ffbb-152b-45e3-81ca-58b1a4c78760',NULL,0,0,NULL,1,0,'123'),(33,'Test1',0,0,0,'2022-09-20 03:22:26.494352',3,'2022-09-27 04:18:49.889774',3,1,'Test1',6,6,6,2,NULL,'2022-09-01 01:01:01.000000','2002-02-02 01:01:01.000000',NULL,NULL,NULL,50000,NULL,'!@#QWE123qwe','Malaysia','Test1','TEST1',NULL,NULL,0,'AQAAAAEAACcQAAAAEF1Rq5wF696wvXiU3dlwAG9jYi/i46CzybSWPiBRwREx5l5dsR4UuiW+cMlx/90nNA==','5ARYAROG3KWP5C7KUUNXAZTLFI6TFAYW','2065f636-7d0e-4680-a77a-13de7f02f1db',NULL,0,0,NULL,1,0,'Testing'),(34,'orange',0,0,0,'2022-09-22 07:14:32.506757',3,NULL,NULL,1,'Orange01',4,6,2,3,NULL,'2022-09-05 00:00:00.000000','1990-01-01 00:00:00.000000',NULL,NULL,NULL,50000,NULL,'!@#QWE123qwe','singapore','Orange01','ORANGE01',NULL,NULL,0,'AQAAAAEAACcQAAAAEKHdpJHupZKfl4ZvyC/Gkq8VsAAGr82Ggjpij01zfVf0Db4OR7i82E6IuPZ2YKDCUg==','NYUPI2W2FSUPUXXF23BBKRCSV5FJBEL5','4cf462ab-9ebc-4729-a468-36ee59524ddb',NULL,0,0,NULL,1,0,'test'),(35,'Pear',0,0,0,'2022-09-23 08:23:33.227520',3,'2022-09-27 14:28:46.825372',1,1,'Pear01',7,16,2,3,NULL,'2022-09-17 01:01:01.000000','1990-09-04 01:01:01.000000',NULL,NULL,NULL,90000,NULL,'!@#QWE123qwe','Singapore','Pear01','PEAR01',NULL,NULL,0,'AQAAAAEAACcQAAAAEG3O6ZF94Nap+WcacBvVHu6YG586dNc+ChaIqNdb4k5I/iOECwlv7OBfVwKJIeXCOw==','37ZUZ426NXFMU2CBZKN66ATJBOJLXJPX','96657e11-fb3f-4795-9363-5bc24beb6103',NULL,0,0,NULL,1,0,'TestPear');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping routines for database 'intranet_testdb'
--
/*!50003 DROP PROCEDURE IF EXISTS `SP_Filter_Leave_History` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES' */ ;
DELIMITER ;;
CREATE DEFINER=`samwee`@`%` PROCEDURE `SP_Filter_Leave_History`(
in currentUserId int,
in inputBrandId int,
in fromTime DATETIME,
in toTime DATETIME)
begin
    declare deptId, rankLevel, isAllBrandCheck int default 0;
   	declare brandIds, employeeIds varchar(200) default '';
   
	select u.DeptId , r.`Level` , group_concat(b.BrandId)
	into 
		deptId,		rankLevel,		brandIds
	from users u 
	left join ranks r 
	on u.RankId = r.Id  
	left join brandemployees b 
	on b.EmployeeId = u.Id 
	where u.Id = currentUserId
	group by u.Id;
	 
	
	select  exists 		
		(select * from brands where IsAllBrand = 1 and (if(inputBrandId is null, FIND_IN_SET(Id, brandIds)>0, Id = inputBrandId)))		
	into isAllBrandCheck;	
	
	if (isAllBrandCheck = 0) then 
		select group_concat(distinct (EmployeeId)) 
		into employeeIds 
		from brandemployees where if(inputBrandId is null, FIND_IN_SET(BrandId, brandIds)>0, BrandId = inputBrandId);
	end if;
	
	select 
		s.EmployeeId,  u.Name as 'EmployeeName', u.EmployeeCode  as 'EmployeeCode', u.DeptId as 'DepartmentId', u.RankId ,
		s.RecordType, s.RecordDetailType, s.LateAmount, s.StartDate, s.EndDate,
		s.NumberOfDays, s.NumberOfHours, s.Fine , s.CalculationAmount 
	from staffrecords s
	inner join Users u  
	on s.EmployeeId = u.Id	
	left join ranks r 
	on u.RankId = r.Id
	where 
		u.DeptId  = deptId 
		and r.`Level` <= rankLevel 
		and (isAllBrandCheck >0 or FIND_IN_SET(s.EmployeeId , employeeIds)>0)
		and (if(fromTime is null, 1, s.CreationTime  >= fromTime))
		and (if(toTime is null, 1, s.CreationTime  <= toTime))
		and s.IsDeleted = 0
		;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `SP_Filter_Time_Off` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES' */ ;
DELIMITER ;;
CREATE DEFINER=`samwee`@`%` PROCEDURE `SP_Filter_Time_Off`(
in currentUserId INT,
in fromTime DATETIME,
in toTime DATETIME,
in exportLimit INT,
in exportOffset INT)
begin
    declare deptId, rankLevel, isAllBrandCheck, totalCount INT default 0;
   	declare brandIds, employeeIds varchar(200) default '';
   
	select u.DeptId , r.`Level` , group_concat(b.BrandId)
	into 
		deptId,		rankLevel,		brandIds
	from users u 
	left join ranks r 
	on u.RankId = r.Id  
	left join brandemployees b 
	on b.EmployeeId = u.Id 
	where u.Id = currentUserId
	group by u.Id;
	 
	
	select  exists 
		(select * from brands b  where IsAllBrand = 1 and Id in (brandIds))
	into isAllBrandCheck;

	if (isAllBrandCheck = 0) then 
		select group_concat(distinct (EmployeeId)) 
		into employeeIds 
		from brandemployees where FIND_IN_SET(BrandId, brandIds)>0;
	end if;
	
	select count(1)
	into totalCount
	from staffrecords s
	inner join Users u  
	on s.EmployeeId = u.Id 
	left join ranks r 
	on u.RankId = r.Id
	where 
		u.DeptId  = deptId 
		and r.`Level` <= rankLevel 
		and (isAllBrandCheck = 1 or FIND_IN_SET(u.Id, employeeIds)>0)
		and (if(fromTime is null, 1, s.CreationTime  >= fromTime))
		and (if(toTime is null, 1, s.CreationTime  <= toTime))
		and s.IsDeleted =0;
	
	
	select s.Id, s.EmployeeId,  u.Name as 'EmployeeName',u.EmployeeCode  as 'EmployeeCode', 
		s.StartDate , s.EndDate , s.CreationTime,
		s.RecordType ,s.RecordDetailType, u.DeptId as 'DepartmentId', u.RankId ,
		r.`Level`, u.CreatorUserId as 'CreatorUserId', totalCount
	from staffrecords s
	inner join Users u  
	on s.EmployeeId = u.Id
	left join ranks r 
	on u.RankId = r.Id
	where 
		u.DeptId  = deptId 
		and r.`Level` <= rankLevel 
		and (isAllBrandCheck = 1 or FIND_IN_SET(u.Id, employeeIds)>0)
		and (if(fromTime is null, 1, s.CreationTime  >= fromTime))
		and (if(toTime is null, 1, s.CreationTime  <= toTime))
		and s.IsDeleted = 0
   	LIMIT exportOffset, exportLimit;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `SP_Get_Employees_By_Current_User` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES' */ ;
DELIMITER ;;
CREATE DEFINER=`samwee`@`%` PROCEDURE `SP_Get_Employees_By_Current_User`(
in currentUserId INT
)
begin
    declare deptId,  isAllBrandCheck int default 0;
   	declare brandIds, deptIds, employeeIds varchar(200) default '';
   
	select u.DeptId , group_concat(b.BrandId)
	into 
		deptId, brandIds
	from users u  
	left join brandemployees b 
	on b.EmployeeId = u.Id 	
	where u.Id = currentUserId
	group by u.Id;

	
	select  group_concat(rd.DepartmentId)
	into 
		deptIds
	from roles r  
	inner join userroles ur 
	on ur.RoleId = r.Id
	inner join roledepartments rd
	on rd.RoleId = r.Id 
	where ur.UserId = currentUserId
	group by ur.UserId;

	set deptIds = concat(deptIds,',',  deptId);
	 
	 
	
	select  exists 
		(select * from brands b  where IsAllBrand = 1 and Id in (brandIds))
	into isAllBrandCheck;
	
	if (isAllBrandCheck = 0) then
		select group_concat(distinct (EmployeeId)) 
		into employeeIds 
		from brandemployees where FIND_IN_SET(BrandId, brandIds)>0;
	end if;
	 
	select u.Id, u.EmployeeCode , u.Name , u.DeptId as 'DepartmentId'
	from Users u 
	inner join brandemployees be
	on be.EmployeeId = u.Id		 
	where 
		    (isAllBrandCheck = 1 or FIND_IN_SET(u.Id  , employeeIds)>0)
		and FIND_IN_SET(u.DeptId  , deptIds)>0
		and u.IsDeleted = 0
	-- group by u.Id, u.EmployeeCode , u.Name , u.DeptId as 'DepartmentId'
;
 
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2022-09-28  9:33:06
