SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for cache
-- ----------------------------
CREATE TABLE `cache` (
  `key` varchar(1000) NOT NULL,
  `data` text NOT NULL,
  PRIMARY KEY (`key`),
  FULLTEXT KEY `key` (`key`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for keyword
-- ----------------------------
CREATE TABLE `keyword` (
  `key` char(255) NOT NULL,
  `data` char(255) NOT NULL,
  PRIMARY KEY (`key`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
