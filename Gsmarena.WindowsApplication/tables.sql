
CREATE TABLE `brands`
(
    `id`   INTEGER AUTO_INCREMENT NOT NULL,
    `name` NVARCHAR(200)          NOT NULL,
    PRIMARY KEY (`id`)
)
    CHARACTER SET `utf8mb4`
    COLLATE `utf8mb4_general_ci`;

CREATE TABLE `networks`
(
    `id`   INTEGER AUTO_INCREMENT NOT NULL,
    `name` NVARCHAR(200)          NOT NULL,
    PRIMARY KEY (`id`)
)
    CHARACTER SET `utf8mb4`
    COLLATE `utf8mb4_general_ci`;

CREATE TABLE `technologies`
(
    `id`   INTEGER AUTO_INCREMENT NOT NULL,
    `name` NVARCHAR(200)          NOT NULL,
    PRIMARY KEY (`id`)
)
    CHARACTER SET `utf8mb4`
    COLLATE `utf8mb4_general_ci`;


CREATE TABLE `operation_systems`
(
    `id`   INTEGER AUTO_INCREMENT NOT NULL,
    `name` NVARCHAR(200)          NOT NULL,
    PRIMARY KEY (`id`)
)
    CHARACTER SET `utf8mb4`
    COLLATE `utf8mb4_general_ci`;

CREATE TABLE `devices`
(
    `id`                       INTEGER AUTO_INCREMENT          NOT NULL,
    `url`                      NVARCHAR(200)                   NOT NULL,
    `name`                     NVARCHAR(200)                   NOT NULL,
    `brand_id`                 INTEGER                         NOT NULL,
    `type`                     ENUM ('PHONE','WATCH','TABLET') NOT NULL,
    `display_ratio`            DECIMAL                         NOT NULL,
    `display_size`             DECIMAL                         NOT NULL,
    `weight`                   DECIMAL                         NOT NULL,
    `battery_capacity`         INTEGER UNSIGNED                NOT NULL,
    `pixel_per_inches`         INTEGER UNSIGNED                NOT NULL,
    `processor_model`          NVARCHAR(200)                   NOT NULL,
    `count_of_thread`          NVARCHAR(200)                   NOT NULL,
    `price`                    DECIMAL                         NOT NULL,
    `year_of_release`          DATE                            NOT NULL,
    `operation_id`             INTEGER                         NOT NULL,
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
    `height`    DECIMAL UNSIGNED       NOT NULL,
    `width`     DECIMAL UNSIGNED       NOT NULL,
    `depth`     DECIMAL UNSIGNED       NOT NULL,
    PRIMARY KEY (`id`),
    FOREIGN KEY (`device_id`) REFERENCES `devices` (`id`)
)
    CHARACTER SET `utf8mb4`
    COLLATE `utf8mb4_general_ci`;

CREATE TABLE `devices_cameras`
(
    `id`        INTEGER AUTO_INCREMENT NOT NULL,
    `pixel`     DECIMAL                NOT NULL,
    `position`  ENUM ('back', 'front') NOT NULL,
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
