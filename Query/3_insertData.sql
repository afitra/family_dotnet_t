-- Insert data ke tabel "Employee" password abcd1234
INSERT INTO "Employees" ("Nama", "Nik", "Email", "Password")
VALUES ('John Doe', 'JD12345', 'a@mail.co', 'l3PNy5XMxeK6aYDHI0viKeRTi05FqUL4+ROAOadkZLE='),
       ('Jane Smith', 'JS67890', 'b@mail.co', 'l3PNy5XMxeK6aYDHI0viKeRTi05FqUL4+ROAOadkZLE=');

-- Insert data ke tabel "Absen"
-- Karyawan 1 (John Doe)
INSERT INTO "Absens" ("Employee_id", "Tanggal", "Geotagging")
VALUES (1, '2024-01-22', '-6.176209,106.839732'),
       (1, '2024-01-23', '-6.176209,106.839732'),
       (1, '2024-01-24', '-6.176209,106.839732'),
       (1, '2024-01-25', '-6.176209,106.839732'),
       (1, '2024-01-26', '-6.176209,106.839732');

-- Karyawan 2 (Jane Smith)
INSERT INTO "Absens" ("Employee_id", "Tanggal", "Geotagging")
VALUES (2, '2024-01-22', '-6.176209,106.839732'),
       (2, '2024-01-23', '-6.176209,106.839732'),
       (2, '2024-01-24', '-6.176209,106.839732'),
       (2, '2024-01-25', '-6.176209,106.839732'),
       (2, '2024-01-26', '-6.176209,106.839732');
