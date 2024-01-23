--  START STORE PROSEDURE REGISTER EMPLOYEEE
CREATE
OR REPLACE PROCEDURE sp_RegisterEmployee(
    p_Nama VARCHAR(255),
    p_Nik VARCHAR(20),
    p_Email VARCHAR(255),
    p_Password VARCHAR(255),
    p_Salt VARCHAR(255)
)
LANGUAGE plpgsql
AS $$
DECLARE
v_HashedPassword VARCHAR(255);
BEGIN
    -- Hashing password dengan salt
    v_HashedPassword
:= ENCODE(DIGEST(p_Salt || p_Password, 'sha256'), 'base64');

    -- Insert employee
BEGIN
INSERT INTO "Employees" ("Nama", "Nik", "Email", "Password")
VALUES (p_Nama, p_Nik, p_Email, v_HashedPassword);

RETURN;
EXCEPTION
        WHEN unique_violation THEN
            -- jka duplikat return err
            RETURN;
END;
END;
$$;


CALL sp_RegisterEmployee('NamaKaryawan', 'NIK123', 'email@example.com', 'password123', 'sembilanKucing');

--  END STORE PROSEDURE REGISTER EMPLOYEEE


--  START STORE PROSEDURE GET ABSEN EMPLOYEEE

CREATE
OR REPLACE FUNCTION GetAbsenByEmployeeAndMonthFunction(
        IN employeeId INT,
        IN target_month INT,
        IN target_year INT
    )
    RETURNS SETOF "Absens"
    LANGUAGE SQL
    AS $$
SELECT *
FROM "Absens"
WHERE "Employee_id" = employeeId
  AND EXTRACT(MONTH FROM "Tanggal") = target_month
  AND EXTRACT(YEAR FROM "Tanggal") = target_year;
$$;


SELECT *
FROM GetAbsenByEmployeeAndMonthFunction(1, 1, 2024);

--  END STORE PROSEDURE GET ABSEN EMPLOYEEE
