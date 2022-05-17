-- MariaDB dump 10.19  Distrib 10.5.15-MariaDB, for Linux (x86_64)
--
-- Host: localhost    Database: camblogistics
-- ------------------------------------------------------
-- Server version	10.5.15-MariaDB

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `areas`
--

DROP TABLE IF EXISTS `areas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `areas` (
  `id` int(11) NOT NULL,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `areas`
--

LOCK TABLES `areas` WRITE;
/*!40000 ALTER TABLE `areas` DISABLE KEYS */;
INSERT INTO `areas` VALUES (0,'Los Santos'),(1,'Külváros'),(2,'San Fierro'),(3,'Angel Pine és környéke'),(4,'Mt. Chilliad'),(5,'Bayside');
/*!40000 ALTER TABLE `areas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `calls`
--

DROP TABLE IF EXISTS `calls`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `calls` (
  `id` int(11) NOT NULL,
  `userID` int(11) NOT NULL,
  `date` datetime NOT NULL,
  `price` int(11) NOT NULL,
  `thisWeek` tinyint(1) NOT NULL,
  `type` smallint(6) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `userID` (`userID`),
  CONSTRAINT `calls_ibfk_1` FOREIGN KEY (`userID`) REFERENCES `users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `calls`
--

LOCK TABLES `calls` WRITE;
/*!40000 ALTER TABLE `calls` DISABLE KEYS */;
/*!40000 ALTER TABLE `calls` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cars`
--

DROP TABLE IF EXISTS `cars`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cars` (
  `id` int(11) NOT NULL,
  `type` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `regNum` varchar(8) COLLATE utf8mb4_unicode_ci NOT NULL,
  `airRide` tinyint(1) NOT NULL,
  `gps` tinyint(1) NOT NULL,
  `ticket` tinyint(1) NOT NULL,
  `engine` int(11) NOT NULL,
  `ecu` int(11) NOT NULL,
  `brakes` int(11) NOT NULL,
  `gearbox` int(11) NOT NULL,
  `tyres` int(11) NOT NULL,
  `turbo` int(11) NOT NULL,
  `suspension` int(11) NOT NULL,
  `weightReduction` int(11) NOT NULL,
  `keyHolder1` int(11) DEFAULT NULL,
  `keyHolder2` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `keyHolder1` (`keyHolder1`),
  KEY `keyHolder2` (`keyHolder2`),
  KEY `engine` (`engine`),
  KEY `ecu` (`ecu`),
  KEY `brakes` (`brakes`),
  KEY `gearbox` (`gearbox`),
  KEY `tyres` (`tyres`),
  KEY `turbo` (`turbo`),
  KEY `suspension` (`suspension`),
  KEY `weightReduction` (`weightReduction`),
  CONSTRAINT `cars_ibfk_1` FOREIGN KEY (`keyHolder1`) REFERENCES `users` (`id`),
  CONSTRAINT `cars_ibfk_10` FOREIGN KEY (`weightReduction`) REFERENCES `tuningLevels` (`level`),
  CONSTRAINT `cars_ibfk_2` FOREIGN KEY (`keyHolder2`) REFERENCES `users` (`id`),
  CONSTRAINT `cars_ibfk_3` FOREIGN KEY (`engine`) REFERENCES `tuningLevels` (`level`),
  CONSTRAINT `cars_ibfk_4` FOREIGN KEY (`ecu`) REFERENCES `tuningLevels` (`level`),
  CONSTRAINT `cars_ibfk_5` FOREIGN KEY (`brakes`) REFERENCES `tuningLevels` (`level`),
  CONSTRAINT `cars_ibfk_6` FOREIGN KEY (`gearbox`) REFERENCES `tuningLevels` (`level`),
  CONSTRAINT `cars_ibfk_7` FOREIGN KEY (`tyres`) REFERENCES `tuningLevels` (`level`),
  CONSTRAINT `cars_ibfk_8` FOREIGN KEY (`turbo`) REFERENCES `tuningLevels` (`level`),
  CONSTRAINT `cars_ibfk_9` FOREIGN KEY (`suspension`) REFERENCES `tuningLevels` (`level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cars`
--

LOCK TABLES `cars` WRITE;
/*!40000 ALTER TABLE `cars` DISABLE KEYS */;
/*!40000 ALTER TABLE `cars` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `deliveryPrices`
--

DROP TABLE IF EXISTS `deliveryPrices`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `deliveryPrices` (
  `type` int(11) NOT NULL,
  `price` int(11) NOT NULL,
  PRIMARY KEY (`type`),
  CONSTRAINT `deliveryPrices_ibfk_1` FOREIGN KEY (`type`) REFERENCES `deliveryTypes` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `deliveryPrices`
--

LOCK TABLES `deliveryPrices` WRITE;
/*!40000 ALTER TABLE `deliveryPrices` DISABLE KEYS */;
INSERT INTO `deliveryPrices` VALUES (0,3000),(1,4000),(2,4500);
/*!40000 ALTER TABLE `deliveryPrices` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `deliveryTypes`
--

DROP TABLE IF EXISTS `deliveryTypes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `deliveryTypes` (
  `id` int(11) NOT NULL,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `deliveryTypes`
--

LOCK TABLES `deliveryTypes` WRITE;
/*!40000 ALTER TABLE `deliveryTypes` DISABLE KEYS */;
INSERT INTO `deliveryTypes` VALUES (0,'Farm beszállítás'),(1,'Műhely beszállítás'),(2,'Autószállítás műhelynél');
/*!40000 ALTER TABLE `deliveryTypes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `documents`
--

DROP TABLE IF EXISTS `documents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `documents` (
  `userID` int(11) NOT NULL,
  `personalID` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `driversLicense` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`personalID`,`driversLicense`),
  KEY `fk_userid` (`userID`),
  CONSTRAINT `fk_userid` FOREIGN KEY (`userID`) REFERENCES `users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `documents`
--

LOCK TABLES `documents` WRITE;
/*!40000 ALTER TABLE `documents` DISABLE KEYS */;
/*!40000 ALTER TABLE `documents` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `images`
--

DROP TABLE IF EXISTS `images`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `images` (
  `userid` int(11) NOT NULL,
  `name` varchar(40) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`name`),
  KEY `userid` (`userid`),
  CONSTRAINT `images_ibfk_1` FOREIGN KEY (`userid`) REFERENCES `users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `images`
--

LOCK TABLES `images` WRITE;
/*!40000 ALTER TABLE `images` DISABLE KEYS */;
/*!40000 ALTER TABLE `images` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `nameChanges`
--

DROP TABLE IF EXISTS `nameChanges`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `nameChanges` (
  `userID` int(11) NOT NULL,
  `newName` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `pending` tinyint(1) NOT NULL,
  `approved` tinyint(1) NOT NULL,
  PRIMARY KEY (`userID`,`newName`),
  CONSTRAINT `nameChanges_ibfk_1` FOREIGN KEY (`userID`) REFERENCES `users` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `nameChanges`
--

LOCK TABLES `nameChanges` WRITE;
/*!40000 ALTER TABLE `nameChanges` DISABLE KEYS */;
/*!40000 ALTER TABLE `nameChanges` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `roles`
--

DROP TABLE IF EXISTS `roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `roles` (
  `id` int(11) NOT NULL,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `roles`
--

LOCK TABLES `roles` WRITE;
/*!40000 ALTER TABLE `roles` DISABLE KEYS */;
INSERT INTO `roles` VALUES (0,'Beszállító'),(1,'Próbaidős sofőr'),(2,'Gyakornok sofőr'),(3,'Sofőr'),(4,'Haladó sofőr'),(5,'Profi sofőr'),(6,'Veterán sofőr'),(7,'Vontatós gyakornok'),(8,'Vontatós'),(9,'Haladó vontatós'),(10,'Telephelyvezető gyakornok'),(11,'Telephelyvezető'),(12,'Igazgató-helyettes'),(13,'Igazgató');
/*!40000 ALTER TABLE `roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sessions`
--

DROP TABLE IF EXISTS `sessions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sessions` (
  `id` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `userID` int(11) NOT NULL,
  `expiry` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `userID` (`userID`),
  CONSTRAINT `sessions_ibfk_1` FOREIGN KEY (`userID`) REFERENCES `users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sessions`
--

LOCK TABLES `sessions` WRITE;
/*!40000 ALTER TABLE `sessions` DISABLE KEYS */;
/*!40000 ALTER TABLE `sessions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `taxiPrices`
--

DROP TABLE IF EXISTS `taxiPrices`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `taxiPrices` (
  `source` int(11) NOT NULL,
  `destination` int(11) NOT NULL,
  `price` int(11) NOT NULL,
  PRIMARY KEY (`source`,`destination`),
  KEY `destination` (`destination`),
  CONSTRAINT `taxiPrices_ibfk_1` FOREIGN KEY (`source`) REFERENCES `areas` (`id`),
  CONSTRAINT `taxiPrices_ibfk_2` FOREIGN KEY (`destination`) REFERENCES `areas` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `taxiPrices`
--

LOCK TABLES `taxiPrices` WRITE;
/*!40000 ALTER TABLE `taxiPrices` DISABLE KEYS */;
INSERT INTO `taxiPrices` VALUES (0,0,700),(0,1,1200),(0,2,1500),(0,3,1300),(0,4,2000),(0,5,2500),(1,1,800),(1,3,1100),(1,4,2000),(1,5,1100),(2,1,1200),(2,2,800),(2,4,2000),(2,5,1200),(3,2,1200),(3,3,1000),(3,4,2000),(3,5,1200),(5,4,2000);
/*!40000 ALTER TABLE `taxiPrices` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `towGarages`
--

DROP TABLE IF EXISTS `towGarages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `towGarages` (
  `id` int(11) NOT NULL,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `towGarages`
--

LOCK TABLES `towGarages` WRITE;
/*!40000 ALTER TABLE `towGarages` DISABLE KEYS */;
INSERT INTO `towGarages` VALUES (0,'BMS'),(1,'Fix'),(2,'Junkyard');
/*!40000 ALTER TABLE `towGarages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `towPrices`
--

DROP TABLE IF EXISTS `towPrices`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `towPrices` (
  `source` int(11) NOT NULL,
  `destination` int(11) NOT NULL,
  `price` int(11) NOT NULL,
  PRIMARY KEY (`source`,`destination`),
  KEY `destination` (`destination`),
  CONSTRAINT `towPrices_ibfk_1` FOREIGN KEY (`source`) REFERENCES `areas` (`id`),
  CONSTRAINT `towPrices_ibfk_2` FOREIGN KEY (`destination`) REFERENCES `towGarages` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `towPrices`
--

LOCK TABLES `towPrices` WRITE;
/*!40000 ALTER TABLE `towPrices` DISABLE KEYS */;
INSERT INTO `towPrices` VALUES (0,0,4000),(0,1,5000),(1,0,4500),(1,1,4500),(2,0,5000),(2,1,4000),(3,0,4500),(3,1,4500),(4,0,6500),(4,1,6500),(5,0,6000),(5,1,4500);
/*!40000 ALTER TABLE `towPrices` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tuningLevels`
--

DROP TABLE IF EXISTS `tuningLevels`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tuningLevels` (
  `level` int(11) NOT NULL,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tuningLevels`
--

LOCK TABLES `tuningLevels` WRITE;
/*!40000 ALTER TABLE `tuningLevels` DISABLE KEYS */;
INSERT INTO `tuningLevels` VALUES (0,'Gyári'),(1,'Alap'),(2,'Profi'),(3,'Verseny'),(4,'Venom');
/*!40000 ALTER TABLE `tuningLevels` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `users` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `role` int(11) NOT NULL,
  `password` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `accepted` tinyint(1) NOT NULL,
  `accountID` int(11) NOT NULL,
  `email` varchar(128) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `warnings`
--

DROP TABLE IF EXISTS `warnings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `warnings` (
  `userID` int(11) NOT NULL,
  `date` datetime NOT NULL,
  PRIMARY KEY (`userID`,`date`),
  CONSTRAINT `warnings_ibfk_1` FOREIGN KEY (`userID`) REFERENCES `users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `warnings`
--

LOCK TABLES `warnings` WRITE;
/*!40000 ALTER TABLE `warnings` DISABLE KEYS */;
/*!40000 ALTER TABLE `warnings` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2022-05-17  9:43:34
