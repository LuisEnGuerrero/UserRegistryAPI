CREATE OR REPLACE PROCEDURE sp_get_user_by_id(
    IN p_id INT,
    OUT p_nombre VARCHAR,
    OUT p_telefono VARCHAR,
    OUT p_direccion VARCHAR,
    OUT p_pais_id INT,
    OUT p_departamento_id INT,
    OUT p_municipio_id INT
)
LANGUAGE plpgsql AS $$
BEGIN
    SELECT nombre, telefono, direccion, pais_id, departamento_id, municipio_id
    INTO p_nombre, p_telefono, p_direccion, p_pais_id, p_departamento_id, p_municipio_id
    FROM usuario
    WHERE id = p_id;
END;
$$;
