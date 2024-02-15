DROP TABLE IF EXISTS `devices_sensors`;

DROP TABLE IF EXISTS `devices_dimensions`;

DROP TABLE IF EXISTS `devices_cameras`;

DROP TABLE IF EXISTS `devices_networks`;

DROP TABLE IF EXISTS `devices_technologies`;

DROP TABLE IF EXISTS `devices_memories`;

DROP TABLE IF EXISTS `devices`;

DROP TABLE IF EXISTS `brands`;

DROP TABLE IF EXISTS `networks`;

DROP TABLE IF EXISTS `unit_of_memories`;

DROP TABLE IF EXISTS `operation_systems`;

DROP TABLE IF EXISTS `technologies`;

DROP TABLE IF EXISTS `sensors`;

CREATE TABLE `brands`
(
    `id`   INTEGER AUTO_INCREMENT NOT NULL,
    `name` NVARCHAR(200)          NULL,
    PRIMARY KEY (`id`)
)
    CHARACTER SET `utf8mb4`
    COLLATE `utf8mb4_general_ci`;

CREATE TABLE `sensors`
(
    `id`   INTEGER AUTO_INCREMENT NOT NULL,
    `name` NVARCHAR(200)          NULL,
    PRIMARY KEY (`id`)
)
    CHARACTER SET `utf8mb4`
    COLLATE `utf8mb4_general_ci`;

CREATE TABLE `networks`
(
    `id`   INTEGER AUTO_INCREMENT NOT NULL,
    `name` NVARCHAR(200)          NULL,
    PRIMARY KEY (`id`)
)
    CHARACTER SET `utf8mb4`
    COLLATE `utf8mb4_general_ci`;

CREATE TABLE `unit_of_memories`
(
    `id`        INTEGER AUTO_INCREMENT NOT NULL,
    `name`      NVARCHAR(200)          NULL,
    `parent_id` INTEGER                NULL,
    `exchange`  INTEGER                NULL,
    PRIMARY KEY (`id`),
    FOREIGN KEY (`parent_id`) REFERENCES `unit_of_memories` (`id`)
)
    CHARACTER SET `utf8mb4`
    COLLATE `utf8mb4_general_ci`;

CREATE TABLE `technologies`
(
    `id`   INTEGER AUTO_INCREMENT NOT NULL,
    `name` NVARCHAR(200)          NULL,
    PRIMARY KEY (`id`)
)
    CHARACTER SET `utf8mb4`
    COLLATE `utf8mb4_general_ci`;


CREATE TABLE `operation_systems`
(
    `id`   INTEGER AUTO_INCREMENT NOT NULL,
    `name` NVARCHAR(200)          NULL,
    PRIMARY KEY (`id`)
)
    CHARACTER SET `utf8mb4`
    COLLATE `utf8mb4_general_ci`;

CREATE TABLE `devices`
(
    `id`                       INTEGER AUTO_INCREMENT          NOT NULL,
    `url`                      NVARCHAR(200)                   NULL,
    `name`                     NVARCHAR(200)                   NULL,
    `brand_id`                 INTEGER                         NULL,
    `type`                     ENUM ('PHONE','WATCH','TABLET') NULL,
    `display_ratio`            DECIMAL                         NULL,
    `display_size`             DECIMAL                         NULL,
    `weight`                   DECIMAL                         NULL,
    `battery_capacity`         INTEGER UNSIGNED                NULL,
    `pixel_per_inches`         INTEGER UNSIGNED                NULL,
    `processor_model`          NVARCHAR(200)                   NULL,
    `count_of_thread`          NVARCHAR(200)                   NULL,
    `price`                    DECIMAL                         NULL,
    `year_of_release`          DATE                            NULL,
    `operation_id`             INTEGER                         NULL,
    `operation_system_version` NVARCHAR(200)                   NULL,
    PRIMARY KEY (`id`),
    FOREIGN KEY (`brand_id`) REFERENCES `brands` (`id`),
    FOREIGN KEY (`operation_id`) REFERENCES `operation_systems` (`id`)
)
    CHARACTER SET `utf8mb4`
    COLLATE `utf8mb4_general_ci`;

CREATE TABLE `devices_dimensions`
(
    `id`        INTEGER AUTO_INCREMENT NOT NULL,
    `device_id` INTEGER                NOT NULL,
    `height`    DECIMAL UNSIGNED       NULL,
    `width`     DECIMAL UNSIGNED       NULL,
    `depth`     DECIMAL UNSIGNED       NULL,
    PRIMARY KEY (`id`),
    FOREIGN KEY (`device_id`) REFERENCES `devices` (`id`)
)
    CHARACTER SET `utf8mb4`
    COLLATE `utf8mb4_general_ci`;

CREATE TABLE `devices_cameras`
(
    `id`        INTEGER AUTO_INCREMENT NOT NULL,
    `pixel`     DECIMAL                NULL,
    `position`  ENUM ('BACK', 'FRONT') NULL,
    `type`      NVARCHAR(200)          NULL,
    `device_id` INTEGER                NOT NULL,
    PRIMARY KEY (`id`),
    FOREIGN KEY (`device_id`) REFERENCES `devices` (`id`)
)
    CHARACTER SET `utf8mb4`
    COLLATE `utf8mb4_general_ci`;

CREATE TABLE `devices_networks`
(
    `id`         INTEGER AUTO_INCREMENT NOT NULL,
    `device_id`  INTEGER                NOT NULL,
    `network_id` INTEGER                NOT NULL,
    PRIMARY KEY (`id`),
    FOREIGN KEY (`device_id`) REFERENCES `devices` (`id`),
    FOREIGN KEY (`network_id`) REFERENCES `networks` (`id`)
)
    CHARACTER SET `utf8mb4`
    COLLATE `utf8mb4_general_ci`;

CREATE TABLE `devices_technologies`
(
    `id`            INTEGER AUTO_INCREMENT NOT NULL,
    `device_id`     INTEGER                NOT NULL,
    `technology_id` INTEGER                NOT NULL,
    PRIMARY KEY (`id`),
    FOREIGN KEY (`device_id`) REFERENCES `devices` (`id`),
    FOREIGN KEY (`technology_id`) REFERENCES `technologies` (`id`)
)
    CHARACTER SET `utf8mb4`
    COLLATE `utf8mb4_general_ci`;

CREATE TABLE `devices_sensors`
(
    `id`            INTEGER AUTO_INCREMENT NOT NULL,
    `device_id`     INTEGER                NOT NULL,
    `sensor_id` INTEGER                NOT NULL,
    PRIMARY KEY (`id`),
    FOREIGN KEY (`device_id`) REFERENCES `devices` (`id`),
    FOREIGN KEY (`sensor_id`) REFERENCES `sensors` (`id`)
)
    CHARACTER SET `utf8mb4`
    COLLATE `utf8mb4_general_ci`;

CREATE TABLE `devices_memories`
(
    `id`             INTEGER AUTO_INCREMENT NOT NULL,
    `device_id`      INTEGER                NOT NULL,
    `memory_size`    DECIMAL                NULL,
    `memory_unit_id` INTEGER                NULL,
    `ram_size`       DECIMAL                NULL,
    `ram_unit_id`    INTEGER                NULL,
    PRIMARY KEY (`id`),
    FOREIGN KEY (`device_id`) REFERENCES `devices` (`id`),
    FOREIGN KEY (`memory_unit_id`) REFERENCES `unit_of_memories` (`id`),
    FOREIGN KEY (`ram_unit_id`) REFERENCES `unit_of_memories` (`id`)
)
    CHARACTER SET `utf8mb4`
    COLLATE `utf8mb4_general_ci`;

INSERT INTO `unit_of_memories`(`name`) VALUE ('MB');
INSERT INTO `unit_of_memories`(`name`, `parent_id`, `exchange`) SELECT 'GB', (SELECT MAX(`id`) FROM `unit_of_memories`), 1024;
INSERT INTO `unit_of_memories`(`name`, `parent_id`, `exchange`) SELECT 'TB', (SELECT MAX(`id`) FROM `unit_of_memories`), 1024;