DROP TABLE IF EXISTS `areas`;
CREATE TABLE `areas` (
  `id` int(11) NOT NULL,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
LOCK TABLES `areas` WRITE;
INSERT INTO `areas` VALUES (0,'Los Santos'),(1,'Külváros'),(2,'San Fierro'),(3,'Angel Pine és környéke'),(4,'Mt. Chilliad'),(5,'Bayside');
UNLOCK TABLES;
DROP TABLE IF EXISTS `calls`;
CREATE TABLE `calls` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `userID` int(11) NOT NULL,
  `date` datetime NOT NULL,
  `price` int(11) NOT NULL,
  `thisWeek` tinyint(1) NOT NULL,
  `type` smallint(6) NOT NULL,
  `previousWeek` tinyint(1) NOT NULL,
  `currentTopList` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `userID` (`userID`),
  CONSTRAINT `calls_ibfk_1` FOREIGN KEY (`userID`) REFERENCES `users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
DROP TABLE IF EXISTS `cars`;
CREATE TABLE `cars` (
  `id` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
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
  `workType` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `keyHolder1` (`keyHolder1`),
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
  CONSTRAINT `cars_ibfk_3` FOREIGN KEY (`engine`) REFERENCES `tuningLevels` (`level`),
  CONSTRAINT `cars_ibfk_4` FOREIGN KEY (`ecu`) REFERENCES `tuningLevels` (`level`),
  CONSTRAINT `cars_ibfk_5` FOREIGN KEY (`brakes`) REFERENCES `tuningLevels` (`level`),
  CONSTRAINT `cars_ibfk_6` FOREIGN KEY (`gearbox`) REFERENCES `tuningLevels` (`level`),
  CONSTRAINT `cars_ibfk_7` FOREIGN KEY (`tyres`) REFERENCES `tuningLevels` (`level`),
  CONSTRAINT `cars_ibfk_8` FOREIGN KEY (`turbo`) REFERENCES `tuningLevels` (`level`),
  CONSTRAINT `cars_ibfk_9` FOREIGN KEY (`suspension`) REFERENCES `tuningLevels` (`level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
DROP TABLE IF EXISTS `holidays`;
CREATE TABLE `holidays` (
  `startDate` date NOT NULL,
  `endDate` date NOT NULL,
  `everyYear` tinyint(1) NOT NULL,
  PRIMARY KEY (`startDate`,`endDate`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
DROP TABLE IF EXISTS `images`;
CREATE TABLE `images` (
  `userid` int(11) NOT NULL,
  `name` varchar(40) COLLATE utf8mb4_unicode_ci NOT NULL,
  `uploadDate` datetime NOT NULL,
  PRIMARY KEY (`name`),
  KEY `userid` (`userid`),
  CONSTRAINT `images_ibfk_1` FOREIGN KEY (`userid`) REFERENCES `users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
DROP TABLE IF EXISTS `nameChanges`;
CREATE TABLE `nameChanges` (
  `userID` int(11) NOT NULL,
  `newName` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `pending` tinyint(1) NOT NULL,
  `approved` tinyint(1) NOT NULL,
  PRIMARY KEY (`userID`,`newName`),
  CONSTRAINT `nameChanges_ibfk_1` FOREIGN KEY (`userID`) REFERENCES `users` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
DROP TABLE IF EXISTS `operatingHours`;
CREATE TABLE `operatingHours` (
  `dayOfWeek` smallint(6) NOT NULL,
  `opening` time NOT NULL,
  `closing` time NOT NULL,
  PRIMARY KEY (`dayOfWeek`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
LOCK TABLES `operatingHours` WRITE;
INSERT INTO `operatingHours` VALUES (0,'15:00:00','20:00:00'),(1,'15:00:00','21:00:00'),(2,'15:00:00','21:00:00'),(3,'15:00:00','21:00:00'),(4,'15:00:00','21:00:00'),(5,'15:00:00','21:00:00'),(6,'15:00:00','20:00:00');
UNLOCK TABLES;
DROP TABLE IF EXISTS `roles`;
CREATE TABLE `roles` (
  `id` int(11) NOT NULL,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'NULL',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
LOCK TABLES `roles` WRITE;
INSERT INTO `roles` VALUES (0,'Beszállító'),(1,'Próbaidős sofőr'),(2,'Gyakornok sofőr'),(3,'Sofőr'),(4,'Haladó sofőr'),(5,'Profi sofőr'),(6,'Veterán sofőr'),(7,'Vontatós gyakornok'),(8,'Vontatós'),(9,'Haladó vontatós'),(10,'Telephelyvezető gyakornok'),(11,'Telephelyvezető'),(12,'Műszaki igazgató'),(13,'Igazgató-helyettes'),(14,'Igazgató');
UNLOCK TABLES;
DROP TABLE IF EXISTS `serviceFees`;
CREATE TABLE `serviceFees` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `userID` int(11) NOT NULL,
  `amount` int(11) NOT NULL,
  `paid` tinyint(1) NOT NULL,
  `date` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `userID` (`userID`),
  CONSTRAINT `serviceFees_ibfk_1` FOREIGN KEY (`userID`) REFERENCES `users` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
DROP TABLE IF EXISTS `sessions`;
CREATE TABLE `sessions` (
  `id` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `userID` int(11) NOT NULL,
  `expiry` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `userID` (`userID`),
  CONSTRAINT `sessions_ibfk_1` FOREIGN KEY (`userID`) REFERENCES `users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
DROP TABLE IF EXISTS `taxiPrices`;
CREATE TABLE `taxiPrices` (
  `source` int(11) NOT NULL,
  `destination` int(11) NOT NULL,
  `price` int(11) NOT NULL,
  PRIMARY KEY (`source`,`destination`),
  KEY `destination` (`destination`),
  CONSTRAINT `taxiPrices_ibfk_1` FOREIGN KEY (`source`) REFERENCES `areas` (`id`),
  CONSTRAINT `taxiPrices_ibfk_2` FOREIGN KEY (`destination`) REFERENCES `areas` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
LOCK TABLES `taxiPrices` WRITE;
INSERT INTO `taxiPrices` VALUES (0,0,700),(0,1,1200),(0,2,1500),(0,3,1300),(0,4,2000),(0,5,2500),(1,1,800),(1,3,1100),(1,4,2000),(1,5,1100),(2,1,1200),(2,2,800),(2,4,2000),(2,5,1200),(3,2,1200),(3,3,1000),(3,4,2000),(3,5,1200),(4,4,2000),(5,4,2000);
UNLOCK TABLES;
DROP TABLE IF EXISTS `towGarages`;
CREATE TABLE `towGarages` (
  `id` int(11) NOT NULL,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
LOCK TABLES `towGarages` WRITE;
INSERT INTO `towGarages` VALUES (0,'BMS'),(1,'Fix'),(2,'Junkyard');
UNLOCK TABLES;
DROP TABLE IF EXISTS `towPrices`;
CREATE TABLE `towPrices` (
  `source` int(11) NOT NULL,
  `destination` int(11) NOT NULL,
  `price` int(11) NOT NULL,
  PRIMARY KEY (`source`,`destination`),
  KEY `destination` (`destination`),
  CONSTRAINT `towPrices_ibfk_1` FOREIGN KEY (`source`) REFERENCES `areas` (`id`),
  CONSTRAINT `towPrices_ibfk_2` FOREIGN KEY (`destination`) REFERENCES `towGarages` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
LOCK TABLES `towPrices` WRITE;
INSERT INTO `towPrices` VALUES (0,0,4000),(0,1,5000),(1,0,4500),(1,1,4500),(2,0,5000),(2,1,4000),(3,0,4500),(3,1,4500),(4,0,6500),(4,1,6500),(5,0,6000),(5,1,4500);
UNLOCK TABLES;
DROP TABLE IF EXISTS `tuningLevels`;
CREATE TABLE `tuningLevels` (
  `level` int(11) NOT NULL,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'NULL',
  PRIMARY KEY (`level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
LOCK TABLES `tuningLevels` WRITE;
INSERT INTO `tuningLevels` VALUES (0,'Gyári'),(1,'Alap'),(2,'Profi'),(3,'Verseny'),(4,'Venom');
UNLOCK TABLES;
DROP TABLE IF EXISTS `users`;
CREATE TABLE `users` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `role` int(11) NOT NULL,
  `password` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `accepted` tinyint(1) NOT NULL,
  `accountID` int(11) NOT NULL,
  `email` varchar(128) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'NULL',
  `deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
DROP TABLE IF EXISTS inactivity;
CREATE TABLE inactivity(
  userid int NOT NULL,
  beginning DATETIME NOT NULL,
  ending DATETIME NOT NULL,
  reason VARCHAR(2048) NOT NULL,
  accepted BOOLEAN NOT NULL,
  pending BOOLEAN NOT NULL,
  PRIMARY KEY(userid,beginning,ending),
  FOREIGN KEY (userid)
  REFERENCES users(id)
  );
DROP TABLE IF EXISTS permissions;
CREATE TABLE `permissions` (
  `roleId` int(11) NOT NULL,
  `permissions` int(11) unsigned NOT NULL,
  PRIMARY KEY (`roleId`),
  CONSTRAINT `permissions_ibfk_1` FOREIGN KEY (`roleId`) REFERENCES `roles` (`id`)
  );
INSERT INTO `permissions` (`permissions`, `roleId`) VALUES (1, 0);
INSERT INTO `permissions` (`permissions`, `roleId`) VALUES (4, 1);
INSERT INTO `permissions` (`permissions`, `roleId`) VALUES (4, 2);
INSERT INTO `permissions` (`permissions`, `roleId`) VALUES (4, 3);
INSERT INTO `permissions` (`permissions`, `roleId`) VALUES (4, 4);
INSERT INTO `permissions` (`permissions`, `roleId`) VALUES (4, 5);
INSERT INTO `permissions` (`permissions`, `roleId`) VALUES (4, 6);
INSERT INTO `permissions` (`permissions`, `roleId`) VALUES (2, 7);
INSERT INTO `permissions` (`permissions`, `roleId`) VALUES (2, 8);
INSERT INTO `permissions` (`permissions`, `roleId`) VALUES (2, 9);
INSERT INTO `permissions` (`permissions`, `roleId`) VALUES (143, 10);
INSERT INTO `permissions` (`permissions`, `roleId`) VALUES (159, 11);
INSERT INTO `permissions` (`permissions`, `roleId`) VALUES (4095, 12);
INSERT INTO `permissions` (`permissions`, `roleId`) VALUES (4095, 13);
INSERT INTO `permissions` (`permissions`, `roleId`) VALUES (4095, 14);
DROP TABLE IF EXISTS blacklist;
CREATE TABLE `blacklist` (
  `accountID` int(11) NOT NULL,
  `roleID` int(11) NOT NULL DEFAULT 0,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `reason` text COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT ' ',
  `canReturn` tinyint(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (`accountID`),
  KEY `roleID` (`roleID`),
  CONSTRAINT `blacklist_ibfk_1` FOREIGN KEY (`roleID`) REFERENCES `roles` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
DROP TABLE IF EXISTS `requiredCalls`;
CREATE TABLE `requiredCalls` (
  `roleId` int(11) NOT NULL,
  `calls` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`roleId`),
  CONSTRAINT `requiredCalls_ibfk_1` FOREIGN KEY (`roleId`) REFERENCES `roles` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
LOCK TABLES `requiredCalls` WRITE;
INSERT INTO `requiredCalls` VALUES (0,0),(1,25),(2,25),(3,20),(4,20),(5,15),(6,15),(7,15),(8,10),(9,10),(10,5),(11,0),(12,0),(13,0),(14,0);