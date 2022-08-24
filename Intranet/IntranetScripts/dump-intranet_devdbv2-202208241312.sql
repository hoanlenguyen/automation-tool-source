-- MySQL dump 10.13  Distrib 5.5.62, for Win64 (AMD64)
--
-- Host: mysqltestdb22.mysql.database.azure.com    Database: intranet_devdbv2
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
INSERT INTO `__efmigrationshistory` VALUES ('20220822033251_initialize_DB','6.0.6'),('20220823012306_StaffRecord_add_OtherDepartment','6.0.6');
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
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `banks`
--

LOCK TABLES `banks` WRITE;
/*!40000 ALTER TABLE `banks` DISABLE KEYS */;
INSERT INTO `banks` VALUES (1,'Mock','2022-01-01 00:00:00.000000',NULL,NULL,NULL,0,1),(2,'MyBank','2022-08-22 11:11:21.728396',1,NULL,NULL,1,0),(3,'ABC','2022-08-22 11:11:32.379326',1,NULL,NULL,1,0);
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
INSERT INTO `brandemployees` VALUES (2,1),(3,1),(4,1),(2,3),(4,3),(2,4),(3,4),(4,4),(3,5),(4,5);
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
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `brands`
--

LOCK TABLES `brands` WRITE;
/*!40000 ALTER TABLE `brands` DISABLE KEYS */;
INSERT INTO `brands` VALUES (1,'Mock','2022-01-01 00:00:00.000000',NULL,NULL,NULL,0,1),(2,'BrandA','2022-08-22 11:11:53.522725',1,NULL,NULL,1,0),(3,'BrandB','2022-08-22 11:12:00.542486',1,NULL,NULL,1,0),(4,'BrandC','2022-08-22 11:12:07.486768',1,NULL,NULL,1,0);
/*!40000 ALTER TABLE `brands` ENABLE KEYS */;
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
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `departments`
--

LOCK TABLES `departments` WRITE;
/*!40000 ALTER TABLE `departments` DISABLE KEYS */;
INSERT INTO `departments` VALUES (1,'Mock','2022-01-01 00:00:00.000000',NULL,NULL,NULL,0,1),(2,'Marketing','2022-08-22 11:12:30.113235',1,NULL,NULL,1,0),(3,'Sales','2022-08-22 11:12:34.965836',1,NULL,NULL,1,0);
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
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ranks`
--

LOCK TABLES `ranks` WRITE;
/*!40000 ALTER TABLE `ranks` DISABLE KEYS */;
INSERT INTO `ranks` VALUES (1,'Mock','2022-01-01 00:00:00.000000',NULL,NULL,NULL,0,1),(2,'Executive','2022-08-22 11:12:56.470980',1,NULL,NULL,1,0),(3,'Officer','2022-08-22 11:13:45.783973',1,NULL,NULL,1,0),(4,'Director','2022-08-22 11:13:52.304676',1,NULL,NULL,1,0);
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
) ENGINE=InnoDB AUTO_INCREMENT=68 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `roleclaims`
--

LOCK TABLES `roleclaims` WRITE;
/*!40000 ALTER TABLE `roleclaims` DISABLE KEYS */;
INSERT INTO `roleclaims` VALUES (1,'Permission','Bank.Create',1),(2,'Permission','Bank.View',1),(3,'Permission','Bank.Update',1),(4,'Permission','Bank.Delete',1),(5,'Permission','Brand.Create',1),(6,'Permission','Brand.View',1),(7,'Permission','Brand.Update',1),(8,'Permission','Brand.Delete',1),(9,'Permission','Rank.Create',1),(10,'Permission','Rank.View',1),(11,'Permission','Rank.Update',1),(12,'Permission','Rank.Delete',1),(13,'Permission','Department.Create',1),(14,'Permission','Department.View',1),(15,'Permission','Department.Update',1),(16,'Permission','Department.Delete',1),(17,'Permission','Role.Create',1),(18,'Permission','Role.View',1),(19,'Permission','Role.Update',1),(20,'Permission','Role.Delete',1),(21,'Permission','Employee.Create',1),(22,'Permission','Employee.View',1),(23,'Permission','Employee.Update',1),(24,'Permission','Employee.Delete',1),(25,'Permission','StaffRecord.Create',1),(26,'Permission','StaffRecord.View',1),(27,'Permission','StaffRecord.Update',1),(28,'Permission','StaffRecord.Delete',1),(29,'Permission','Employee.View',2),(30,'Permission','Employee.Create',2),(31,'Permission','Employee.Update',2),(32,'Permission','Employee.Delete',2),(33,'Permission','Bank.View',3),(34,'Permission','Brand.View',3),(35,'Permission','Rank.View',3),(36,'Permission','Department.View',3),(37,'Permission','Employee.View',3),(38,'Permission','Role.View',3),(39,'Permission','StaffRecord.View',3),(40,'Permission','Bank.View',4),(41,'Permission','Bank.Create',4),(42,'Permission','Bank.Update',4),(43,'Permission','Bank.Delete',4),(44,'Permission','Brand.View',4),(45,'Permission','Brand.Create',4),(46,'Permission','Brand.Update',4),(47,'Permission','Brand.Delete',4),(48,'Permission','Rank.View',4),(49,'Permission','Rank.Create',4),(50,'Permission','Rank.Update',4),(51,'Permission','Rank.Delete',4),(52,'Permission','Department.View',4),(53,'Permission','Department.Create',4),(54,'Permission','Department.Update',4),(55,'Permission','Department.Delete',4),(56,'Permission','Employee.View',4),(57,'Permission','Employee.Create',4),(58,'Permission','Employee.Update',4),(59,'Permission','Employee.Delete',4),(60,'Permission','Role.View',4),(61,'Permission','Role.Create',4),(62,'Permission','Role.Update',4),(63,'Permission','Role.Delete',4),(64,'Permission','StaffRecord.View',4),(65,'Permission','StaffRecord.Create',4),(66,'Permission','StaffRecord.Update',4),(67,'Permission','StaffRecord.Delete',4);
/*!40000 ALTER TABLE `roleclaims` ENABLE KEYS */;
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
  PRIMARY KEY (`Id`),
  UNIQUE KEY `RoleNameIndex` (`NormalizedName`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `roles`
--

LOCK TABLES `roles` WRITE;
/*!40000 ALTER TABLE `roles` DISABLE KEYS */;
INSERT INTO `roles` VALUES (1,'2022-01-01 00:00:00.000000',1,'SuperAdmin','SUPERADMIN','88ba3b9c-84d9-489e-90c1-dce3cfbbf182',NULL,NULL,1,0),(2,'2022-08-22 11:07:54.868455',1,'CEO','CEO','88036983-e0ba-4a9a-94b4-d58fa121a178',NULL,NULL,1,0),(3,'2022-08-22 11:14:21.270081',1,'Staff','STAFF','32328fca-64df-4f5b-b927-899424de6710',NULL,NULL,1,0),(4,'2022-08-22 13:55:53.976194',1,'Manager','MANAGER','7c94f499-5d07-45ed-a968-979c9bf7bed4',NULL,NULL,1,0);
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
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `staffrecorddocuments`
--

LOCK TABLES `staffrecorddocuments` WRITE;
/*!40000 ALTER TABLE `staffrecorddocuments` DISABLE KEYS */;
INSERT INTO `staffrecorddocuments` VALUES (13,5,'import Employee Sample - v2.xlsx'),(14,5,'import Employee Sample.xlsx');
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
  PRIMARY KEY (`Id`),
  KEY `IX_StaffRecords_EmployeeId` (`EmployeeId`),
  CONSTRAINT `FK_StaffRecords_Users_EmployeeId` FOREIGN KEY (`EmployeeId`) REFERENCES `users` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `staffrecords`
--

LOCK TABLES `staffrecords` WRITE;
/*!40000 ALTER TABLE `staffrecords` DISABLE KEYS */;
INSERT INTO `staffrecords` VALUES (1,5,3,3,'day off','2022-08-04 00:00:00.000000','2022-08-06 00:00:00.000000','Remarks....','2022-08-23 09:38:43.123245',0,'2022-08-23 13:28:26.904462',1,1,0,NULL),(2,3,3,2,'paid off test reason','2022-08-07 00:00:00.000000','2022-08-12 00:00:00.000000','....','2022-08-23 16:51:25.644803',1,'2022-08-24 11:21:26.904425',1,1,0,NULL),(3,4,2,3,'reason test','2022-08-25 00:00:00.000000','2022-08-27 00:00:00.000000','...','2022-08-24 09:12:34.376823',1,'2022-08-24 11:21:19.268903',1,1,0,NULL),(4,1,0,0,'r test','2022-08-08 00:00:00.000000','2022-08-12 00:00:00.000000','..','2022-08-24 09:14:55.849629',1,'2022-08-24 11:21:08.924498',1,1,0,'IT dept'),(5,5,3,1,'test','2022-08-07 00:00:00.000000','2022-08-06 00:00:00.000000','.....','2022-08-24 11:18:24.614594',1,'2022-08-24 04:44:37.982280',1,1,0,NULL);
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
INSERT INTO `userroles` VALUES (1,1),(4,3),(5,3),(3,4);
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
  `BirthDate` datetime(6) NOT NULL,
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
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Users_EmployeeCode` (`EmployeeCode`),
  UNIQUE KEY `UserNameIndex` (`NormalizedUserName`),
  KEY `EmailIndex` (`NormalizedEmail`),
  KEY `IX_Users_BankId` (`BankId`),
  KEY `IX_Users_DeptId` (`DeptId`),
  KEY `IX_Users_RankId` (`RankId`),
  KEY `IX_Users_RoleId` (`RoleId`),
  CONSTRAINT `FK_Users_Banks_BankId` FOREIGN KEY (`BankId`) REFERENCES `banks` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Users_Departments_DeptId` FOREIGN KEY (`DeptId`) REFERENCES `departments` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Users_Ranks_RankId` FOREIGN KEY (`RankId`) REFERENCES `ranks` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Users_Roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'hoan le',10,0,1,'2022-08-22 10:35:52.287121',NULL,NULL,NULL,1,'lenguyenhanhoan',1,1,1,1,NULL,NULL,'2022-08-22 10:35:52.287123',NULL,NULL,NULL,0,NULL,'',NULL,'lenguyenhanhoan','LENGUYENHANHOAN','lenguyenhanhoan@gmail.com','LENGUYENHANHOAN@GMAIL.COM',0,'AQAAAAEAACcQAAAAEBXHkV9a99XUWr7CZUZBjTvu3xIzj88oq84yRa6cXd8yKXyL8BPHeQ59dAFpp5GuEw==','NC6E7ELKUOG5QQCLCQLCXFJI5QAPDG5C','02e032e3-7a6d-4a01-aefc-2f9b3eb94665',NULL,0,0,NULL,1,0),(3,'Glory',0,0,0,'2022-08-22 13:16:46.286828',1,'2022-08-24 02:51:17.301057',1,1,'Glory080',4,2,2,2,'321312323','2022-08-08 00:00:00.000000','2022-08-01 00:00:00.000000','2323123131',NULL,NULL,232000,NULL,'123qwe!@#QWE','Singapore','Glory080','GLORY80',NULL,NULL,0,'AQAAAAEAACcQAAAAEHzqc+7fZ3WpDKMzC3U0TI3TcNl+GSh4CeErGYSupkyea0fzjnskUnk+0Q7XPIGECA==','BUHGNTCEUPHZ6DYIVF4FWYNBW4H7NTH7','10c877b6-21b5-44b0-b7b6-96a27b7f7876',NULL,0,0,NULL,1,0),(4,'HoanLe',0,0,0,'2022-08-22 13:48:14.142796',1,'2022-08-24 02:52:23.438458',1,1,'HoanLe081',3,3,2,3,'127777777','2022-08-10 00:00:00.000000','2022-08-02 00:00:00.000000','2112122122',NULL,NULL,20000000,NULL,'123qwe!@#QWE','Vietnam','HoanLe081','HOANLE081',NULL,NULL,0,'AQAAAAEAACcQAAAAEFFUkv6pqcZOI2+mzdsYSdUUUxRb2IdztxzzOKE6gSR1NnpZN9fZlOXDyQcUyqnogA==','IXT6UUXLYOQQAUUDX65U2LV2GZRYUJ7C','ba8e1656-d780-4915-9e6e-1c064498a759',NULL,0,0,NULL,1,0),(5,'Hoan Le V5',0,0,0,'2022-08-22 13:54:01.667898',1,'2022-08-22 13:55:14.493457',1,1,'HoanLeCode05',3,3,2,3,'11121212121221','2022-08-10 00:00:00.000000','2001-06-02 00:00:00.000000','33333111',NULL,NULL,3000000,NULL,'123qwe!@#QWE',NULL,'HoanLeV5','HOANLEV5',NULL,NULL,0,'AQAAAAEAACcQAAAAEGZWZ2VQTRUs8J6rKeBjiz1bftHhtCx0Nkac3yYHESc67FJg3k5dM0EXaenx72pBEA==','PNK62HEZF3QREJLZXIFCH6AKJZ6OGCET','c3cad9df-b755-4227-af2a-8136062ebdea',NULL,0,0,NULL,1,0);
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping events for database 'intranet_devdbv2'
--

--
-- Dumping routines for database 'intranet_devdbv2'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2022-08-24 13:12:43
