-- CREATE
-- EXTENSION IF NOT EXISTS postgis;
-- CREATE
-- EXTENSION IF NOT EXISTS postgis_topology;


-- Tabel Employee
CREATE TABLE "Employees"
(
    "Id"       SERIAL PRIMARY KEY,
    "Nama"     VARCHAR(255) NOT NULL,
    "Nik"      VARCHAR(20)  NOT NULL UNIQUE,
    "Email"    VARCHAR(255) NOT NULL UNIQUE,
    "Password" VARCHAR(255) NOT NULL
);

-- Tabel Absen dengan Geotagging bertipe data geometry
CREATE TABLE "Absens"
(
    "Id"          SERIAL PRIMARY KEY,
    "Employee_id" INT REFERENCES "Employees" ("Id") ON DELETE CASCADE,
    "Tanggal"     DATE         NOT NULL,
    "Geotagging"  VARCHAR(255) NOT NULL,
    CONSTRAINT Absen_Employee FOREIGN KEY ("Employee_id") REFERENCES "Employees" ("Id")
);